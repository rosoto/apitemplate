using Arquetipo.Api.Handlers;
using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Arquetipo.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[ApiVersion("2")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteHandler _clienteHandler;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(IClienteHandler clienteHandler,
                            ILogger<ClienteController> logger)
    {
        _clienteHandler = clienteHandler;
        _logger = logger;
    }

    /// <summary>Obtiene Todos los Clientes</summary>
    /// <remarks>V1 Permite Obtener los Cliente con un limite de 10 registros</remarks>
    [MapToApiVersion("1")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllAsync()
    {
        var response = await _clienteHandler.GetClientes();
        if(!response.Data.Any()) return NoContent();
        _logger.LogInformation("respuesta OK");
        return Ok(response);
    }

    /// <summary>Obtiene Cliente segun ID</summary>
    /// <remarks>V1 Permite Obtener un Cliente segun su ID</remarks>
    /// <param name="id" example="5">id cliente</param>
    [MapToApiVersion("1")]
    [HttpGet("{id:int?}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))]
    public async Task<IActionResult> GetByIdAsync(int? id)
    {
        var response = await _clienteHandler.GetClienteId(id);
        return Ok(response);
    }

    /// <summary>Obtiene Clientes segun busqueda</summary>
    /// <remarks>V2 Permite Obtener Clientes que hagan mach en la busqueda con un limite de 10 registros</remarks>
    [MapToApiVersion("2")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))]
    public async Task<IActionResult> GetByAnyAsync([FromQuery] SetClienteAny cliente)
    {
        var response = await _clienteHandler.GetClientesAny(cliente);
        return Ok(response);
    }

    /// <summary>Inserta Clientes</summary>
    /// <remarks>V1 Permite Insertar lista de Clientes</remarks>
    [MapToApiVersion("1")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAsync([FromBody]List<SetCliente> clientes)
    {
        await _clienteHandler.PostClientes(clientes);
        return Ok();
    }

    /// <summary>Actualiza Cliente</summary>
    /// <remarks>V1 Permite Actualizar un Cliente segun su Id</remarks>
    [MapToApiVersion("1")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync([FromBody]SetClienteId cliente)
    {
        var result = await _clienteHandler.UpdateCliente(cliente);
        if(!result) return Conflict();
        return Ok();
    }

    /// <summary>Elimina Cliente</summary>
    /// <remarks>V1 Permite Eliminar un Cliente segun su Id</remarks>
    [MapToApiVersion("1")]
    [HttpDelete("{id:int?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int? id)
    {
        var result = await _clienteHandler.DeleteCliente(id);
        if(!result) return Conflict();
        return Ok();
    }
}