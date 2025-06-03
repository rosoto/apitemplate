using Arquetipo.Api.Infrastructure.Persistence;
using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Response; // O la entidad si es diferente
using Microsoft.EntityFrameworkCore;

namespace Arquetipo.Api.Infrastructure;

public class ClienteRepository : IClienteRepository
{
    private readonly ArquetipoDbContext _context;
    private readonly ILogger<ClienteRepository> _logger;

    public ClienteRepository(ArquetipoDbContext context, ILogger<ClienteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Cliente>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("Obteniendo clientes paginados desde EF Core. Página: {Page}, Tamaño: {PageSize}", page, pageSize);

        // Validación básica para evitar valores negativos o cero
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return await _context.Clientes
            .OrderBy(c => c.Id) // Ordena por Id para paginación consistente
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Cliente> GetByIdAsync(int? id)
    {
        _logger.LogInformation("Obteniendo cliente por ID {Id} desde EF Core", id);
        if (id == null) return null;
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<List<Cliente>> GetByAnyAsync(SetClienteAny clienteParams)
    {
        _logger.LogInformation("Buscando clientes desde EF Core con parámetros: {@ClienteParams}", clienteParams);
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(clienteParams.Nombre))
        {
            query = query.Where(c => c.Nombre.Contains(clienteParams.Nombre));
        }
        if (!string.IsNullOrWhiteSpace(clienteParams.Apellido))
        {
            query = query.Where(c => c.Apellido.Contains(clienteParams.Apellido));
        }
        if (!string.IsNullOrWhiteSpace(clienteParams.Email))
        {
            query = query.Where(c => c.Email.Contains(clienteParams.Email));
        }
        if (!string.IsNullOrWhiteSpace(clienteParams.Telefono))
        {
            query = query.Where(c => c.Telefono.Contains(clienteParams.Telefono));
        }

        return await query.Take(10).ToListAsync(); // Manteniendo la lógica del TOP 10
    }

    public async Task<bool> ExistsAsync(int? id)
    {
        if (id == null) return false;
        return await _context.Clientes.AnyAsync(c => c.Id == id);
    }

    public async Task AddClientesAsync(List<SetCliente> clientesDto)
    {
        if (clientesDto == null || !clientesDto.Any())
        {
            _logger.LogWarning("Intento de agregar una lista de clientes nula o vacía.");
            return;
        }

        var clientesEntities = new List<Cliente>();
        foreach (var dto in clientesDto)
        {
            // Mapeo manual de DTO a Entidad (o usa AutoMapper)
            clientesEntities.Add(new Cliente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Telefono = dto.Telefono
            });
        }

        await _context.Clientes.AddRangeAsync(clientesEntities);
        await _context.SaveChangesAsync();
        _logger.LogInformation("{Count} clientes agregados a través de EF Core.", clientesEntities.Count);
    }

    public async Task UpdateAsync(SetClienteId clienteDto)
    {
        if (clienteDto?.Id == null)
        {
            _logger.LogWarning("Intento de actualizar un cliente con DTO nulo o ID nulo.");
            // Considera lanzar una excepción si esto no debería ocurrir
            return;
        }

        var clienteEntity = await _context.Clientes.FindAsync(clienteDto.Id);

        if (clienteEntity == null)
        {
            _logger.LogWarning("No se encontró el cliente con ID {Id} para actualizar.", clienteDto.Id);
            // Considera lanzar una excepción o devolver un booleano indicando fallo
            return;
        }

        // Mapeo manual de DTO a Entidad (o usa AutoMapper)
        clienteEntity.Nombre = clienteDto.Nombre;
        clienteEntity.Apellido = clienteDto.Apellido;
        clienteEntity.Email = clienteDto.Email;
        clienteEntity.Telefono = clienteDto.Telefono;

        _context.Clientes.Update(clienteEntity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Cliente con ID {Id} actualizado a través de EF Core.", clienteDto.Id);
    }

    public async Task DeleteAsync(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Intento de eliminar un cliente con ID nulo.");
            return;
        }

        var clienteEntity = await _context.Clientes.FindAsync(id);

        if (clienteEntity == null)
        {
            _logger.LogWarning("No se encontró el cliente con ID {Id} para eliminar.", id);
            return;
        }

        _context.Clientes.Remove(clienteEntity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Cliente con ID {Id} eliminado a través de EF Core.", id);
    }
}