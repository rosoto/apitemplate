using Arquetipo.Api.Infrastructure; //
using Arquetipo.Api.Models.Request; //
using Arquetipo.Api.Models.Response; //
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arquetipo.Api.Handlers; //

public class ClienteHandler : IClienteHandler
{
    private readonly IClienteRepository _clienteRepository; //
    private readonly ILogger<ClienteHandler> _logger; //
    public ClienteHandler(IClienteRepository clienteRepository,
                        ILogger<ClienteHandler> logger) //
    {
        _clienteRepository = clienteRepository; //
        _logger = logger; //
    }

    public async Task<DataCliente> GetClientes(int page, int pageSize) //
    {
        var result = new DataCliente { Data = new List<Cliente>() };
        var clientes = await _clienteRepository.GetAllAsync(page, pageSize);
        if (clientes != null && clientes.Any())
            result.Data.AddRange(clientes);
        return result;
    }

    public async Task<DataCliente> GetClienteId(int? Id) //
    {
        var result = new DataCliente { Data = new List<Cliente>() }; //
        var cliente = await _clienteRepository.GetByIdAsync(Id); //
        if (cliente is not null) result.Data.Add(cliente); //
        return result; //
    }

    public async Task<DataCliente> GetClientesAny(SetClienteAny cliente) //
    {
        var result = new DataCliente { Data = new List<Cliente>() }; //
        var response = await _clienteRepository.GetByAnyAsync(cliente); //
        if (response != null && response.Any()) result.Data.AddRange(response); //
        return result; //
    }

    public async Task PostClientes(List<SetCliente> clientes) //
    {
        await _clienteRepository.AddClientesAsync(clientes); //
    }

    public async Task<bool> UpdateCliente(SetClienteId cliente) //
    {
        if (!await _clienteRepository.ExistsAsync(cliente.Id)) return false; //
        await _clienteRepository.UpdateAsync(cliente); //
        _logger.LogInformation("respuesta true"); //
        return true; //
    }

    public async Task<bool> DeleteCliente(int? IdCliente) //
    {
        if (!await _clienteRepository.ExistsAsync(IdCliente)) return false; //
        await _clienteRepository.DeleteAsync(IdCliente); //
        return true; //
    }
}