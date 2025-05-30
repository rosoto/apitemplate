using Arquetipo.Api.Infrastructure;
using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Response;

namespace Arquetipo.Api.Handlers;

public class ClienteHandler : IClienteHandler
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<ClienteHandler> _logger;
    public ClienteHandler(IClienteRepository clienteRepository,
                        ILogger<ClienteHandler> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
    }

    // Obtiene todos los clientes.
    public async Task<DataCliente> GetClientes()
    {
        var result = new DataCliente();
        result.Data = new List<Cliente>();
        var clientes = await _clienteRepository.GetAllAsync();
        if (clientes.Any()) result.Data.AddRange(clientes);
        return result;
    }

    // Obtiene cliente por Id.
    public async Task<DataCliente> GetClienteId(int? Id)
    {
        var result = new DataCliente();
        result.Data = new List<Cliente>();
        var cliente = await _clienteRepository.GetByIdAsync(Id);
        if (cliente is not null) result.Data.Add(cliente);
        return result;
    }

    // Obtiene cliente por busqueda.
    public async Task<DataCliente> GetClientesAny(SetClienteAny cliente)
    {
        var result = new DataCliente();
        result.Data = new List<Cliente>();
        var response = await _clienteRepository.GetByAnyAsync(cliente);
        if (response.Any()) result.Data.AddRange(response);
        return result;
    }

     // Agrega una lista de clientes.
    public async Task PostClientes(List<SetCliente> clientes)
    {
        await _clienteRepository.AddClientesAsync(clientes);
    }

    // Actualiza un cliente.
    public async Task<bool> UpdateCliente(SetClienteId cliente)
    {
        if (!await _clienteRepository.ExistsAsync(cliente.Id)) return false;

        await _clienteRepository.UpdateAsync(cliente);
        _logger.LogInformation("respueta true");
        return true;
    }

    // Elimina un cliente por su ID.
    public async Task<bool> DeleteCliente(int? IdCliente)
    {
        if (!await _clienteRepository.ExistsAsync(IdCliente)) return false;

        await _clienteRepository.DeleteAsync(IdCliente);
        return true;
    }
}