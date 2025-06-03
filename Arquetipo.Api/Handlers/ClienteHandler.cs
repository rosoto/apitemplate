using Arquetipo.Api.Infrastructure;
using Arquetipo.Api.Models.Request; 
using Arquetipo.Api.Handlers.Mapper;
using Arquetipo.Api.Models.Request.v2;
using Arquetipo.Api.Models.Response.v2;
using Arquetipo.Api.Models.Response.v1;
using Arquetipo.Api.Models.Request.v1;

namespace Arquetipo.Api.Handlers;

public class ClienteHandler(IClienteRepository clienteRepository, ILogger<ClienteHandler> logger) : IClienteHandler
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ILogger<ClienteHandler> _logger = logger;

    // --- V1 Implementations ---
    public async Task<DataClienteResponse> GetClientesV1Async(int page, int pageSize)
    {
        // Línea 16 antes (aproximadamente)
        var result = new DataClienteResponse { Data = new List<ClienteResponse>() };
        var repoClientes = await _clienteRepository.GetAllAsync(page, pageSize);
        if (repoClientes != null && repoClientes.Any())
        {
            result.Data.AddRange(repoClientes.Select(ClienteMapper.ToClienteResponseV1).Where(c => c != null));
        }
        return result;
    }

    public async Task<DataClienteResponse> GetClienteByIdV1Async(int? id)
    {
        // Línea 23 antes (aproximadamente)
        var result = new DataClienteResponse { Data = new List<ClienteResponse>() };
        var repoCliente = await _clienteRepository.GetByIdAsync(id);
        if (repoCliente is not null)
        {
            var clienteDto = ClienteMapper.ToClienteResponseV1(repoCliente);
            if (clienteDto != null) result.Data.Add(clienteDto);
        }
        return result;
    }

    public async Task<DataClienteResponse> GetClientesByAnyV1Async(BuscarClienteRequest queryV1) // Corregido el tipo
    {
        _logger.LogInformation("Handler V1: Buscando clientes con query: {@QueryV1}", queryV1);

        var response = new DataClienteResponse { Data = new List<ClienteResponse>() };

        if (queryV1 == null)
        {
            _logger.LogWarning("Handler V1: El objeto de consulta (BuscarClienteQueryV1) es nulo.");
            response.Status = 400;
            return response;
        }

        var setClienteAnyParaRepo = ClienteMapper.ToSetClienteAny(queryV1); // Usar Mapper

        if (setClienteAnyParaRepo == null)
        {
            _logger.LogWarning("Handler V1: No se pudo mapear BuscarClienteQueryV1 a SetClienteAny. Query: {@QueryV1}", queryV1);
            response.Status = 400;
            return response;
        }

        var repoClientes = await _clienteRepository.GetByAnyAsync(setClienteAnyParaRepo);

        if (repoClientes != null && repoClientes.Any())
        {
            response.Data.AddRange(repoClientes.Select(ClienteMapper.ToClienteResponseV1).Where(c => c != null));
            _logger.LogInformation("Handler V1: Se encontraron {Count} clientes para la consulta.", response.Data.Count);
        }
        else
        {
            _logger.LogInformation("Handler V1: No se encontraron clientes para la consulta.");
        }
        return response;
    }

    public async Task PostClientesV1Async(List<CrearClienteRequestV1> clientes)
    {
        // Línea 63 que mencionaste. Usar Mapper.
        var repoClientes = clientes.Select(ClienteMapper.ToSetCliente).Where(c => c != null).ToList();
        if (repoClientes.Any())
        {
            await _clienteRepository.AddClientesAsync(repoClientes);
        }
    }

    public async Task<bool> UpdateClienteV1Async(ActualizarClienteRequest cliente) // Corregido el tipo
    {
        if (cliente?.Id == null || !await _clienteRepository.ExistsAsync(cliente.Id))
        {
            _logger.LogWarning("Handler V1: Cliente no existe o ID es nulo para actualizar. ID: {ClienteId}", cliente?.Id);
            return false;
        }

        var repoUpdateDto = ClienteMapper.ToSetClienteId(cliente); // Usar Mapper
        if (repoUpdateDto == null)
        {
            _logger.LogError("Handler V1: Falla al mapear ActualizarClienteRequestV1 a SetClienteId para ID: {Id}", cliente.Id);
            return false;
        }
        await _clienteRepository.UpdateAsync(repoUpdateDto);
        return true;
    }

    public async Task<bool> DeleteClienteV1Async(int? idCliente)
    {
        if (!await _clienteRepository.ExistsAsync(idCliente)) return false;
        await _clienteRepository.DeleteAsync(idCliente);
        return true;
    }

    // --- V2 Implementations ---
    public async Task<DataClienteResponseV2> GetClientesV2Async(int page, int pageSize, bool? soloActivos)
    {
        _logger.LogInformation("Handler: GetClientesV2Async - soloActivos: {SoloActivos}", soloActivos);
        var result = new DataClienteResponseV2 { Data = new List<ClienteResponseV2>() };
        var repoClientes = await _clienteRepository.GetAllAsync(page, pageSize);

        // if(soloActivos.HasValue && soloActivos.Value) {
        //     repoClientes = repoClientes.Where(c => c.EstaActivo).ToList();
        // }

        if (repoClientes != null && repoClientes.Any())
        {
            result.Data.AddRange(repoClientes.Select(ClienteMapper.ToClienteResponseV2).Where(c => c != null));
        }
        return result;
    }

    public async Task<DataClienteResponseV2> GetClienteByIdV2Async(int? id)
    {
        var result = new DataClienteResponseV2 { Data = new List<ClienteResponseV2>() };
        var repoCliente = await _clienteRepository.GetByIdAsync(id);
        if (repoCliente is not null)
        {
            var clienteDto = ClienteMapper.ToClienteResponseV2(repoCliente);
            if (clienteDto != null) result.Data.Add(clienteDto);
        }
        return result;
    }

    public async Task<DataClienteResponseV2> GetClientesByAnyV2Async(BuscarClienteRequestV2 query) // Corregido tipo y nombre de parámetro
    {
        _logger.LogInformation("Handler V2: Buscando clientes con query: {@QueryV2}", query);
        var result = new DataClienteResponseV2 { Data = new List<ClienteResponseV2>() };

        if (query == null)
        {
            _logger.LogWarning("Handler V2: El objeto de consulta (BuscarClienteQueryV2) es nulo.");
            result.Status = 400;
            return result;
        }

        // Usar Mapper
        var setClienteAnyParaRepo = ClienteMapper.ToSetClienteAny(query);

        if (setClienteAnyParaRepo == null)
        {
            _logger.LogWarning("Handler V2: No se pudo mapear BuscarClienteQueryV2 a SetClienteAny. Query: {@QueryV2}", query);
            result.Status = 400;
            return result;
        }

        var repoClientes = await _clienteRepository.GetByAnyAsync(setClienteAnyParaRepo);

        if (repoClientes != null && repoClientes.Any())
        {
            result.Data.AddRange(repoClientes.Select(ClienteMapper.ToClienteResponseV2).Where(c => c != null));
            _logger.LogInformation("Handler V2: Se encontraron {Count} clientes para la consulta.", result.Data.Count);
        }
        else
        {
            _logger.LogInformation("Handler V2: No se encontraron clientes para la consulta.");
        }
        return result;
    }

    public async Task PostClientesV2Async(List<CrearClienteRequestV2> clientes)
    {
        // Línea 144 que mencionaste. Usar Mapper.
        var repoSetClienteList = new List<SetCliente>();
        foreach (var reqV2 in clientes)
        {
            var setCliente = ClienteMapper.ToSetCliente(reqV2); // Mapea CrearClienteRequestV2 a SetCliente
            if (setCliente != null)
            {
                // Lógica adicional si SetCliente necesita ser adaptado para campos de V2
                // que no existen directamente en SetCliente (ej. PreferenciaContacto)
                // Esto podría implicar que SetCliente necesite esos campos, o que la entidad
                // del repositorio los tenga y se actualicen de otra forma.
                repoSetClienteList.Add(setCliente);
            }
        }

        if (repoSetClienteList.Any())
        {
            _logger.LogInformation("Handler V2: Creando {Count} clientes.", repoSetClienteList.Count);
            await _clienteRepository.AddClientesAsync(repoSetClienteList);
        }
        else
        {
            _logger.LogWarning("Handler V2: No hay clientes válidos para crear después del mapeo.");
        }
    }

    public async Task<bool> UpdateClienteV2Async(ActualizarClienteRequestV2 clienteUpdateReq)
    {
        if (clienteUpdateReq?.Id == null)
        {
            _logger.LogWarning("Handler V2: Solicitud de actualización de cliente V2 inválida o ID nulo.");
            return false;
        }

        var entidadCliente = await _clienteRepository.GetByIdAsync(clienteUpdateReq.Id.Value);
        if (entidadCliente == null)
        {
            _logger.LogWarning("Handler V2: Cliente con ID {Id} no encontrado para actualizar.", clienteUpdateReq.Id);
            return false;
        }

        ClienteMapper.ApplyUpdate(entidadCliente, clienteUpdateReq);

        // Mapear la entidadCliente actualizada al SetClienteId que espera el repositorio
        var repoUpdateDto = new SetClienteId
        {
            Id = entidadCliente.Id,
            Nombre = entidadCliente.Nombre,
            Apellido = entidadCliente.Apellido,
            Email = entidadCliente.Email,
            Telefono = entidadCliente.Telefono
        };
        // Este mapeo también podría estar en ClienteMapper si se vuelve complejo:
        // var repoUpdateDto = ClienteMapper.ToSetClienteId(entidadCliente);

        await _clienteRepository.UpdateAsync(repoUpdateDto);
        return true;
    }
}