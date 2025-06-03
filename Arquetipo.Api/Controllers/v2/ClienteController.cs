using Arquetipo.Api.Handlers; //
using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Response;
using Asp.Versioning; // ACTUALIZADO
using Microsoft.AspNetCore.Mvc; //

namespace Arquetipo.Api.Controllers; //

[ApiController] //
[ApiVersion("2")] //
[Route("api/v{version:apiVersion}/[controller]")] //
public class ClienteController : ControllerBase //
{
    private readonly IClienteHandler _clienteHandler; //
    private readonly ILogger<ClienteController> _logger; //

    public ClienteController(IClienteHandler clienteHandler,
                            ILogger<ClienteController> logger) //
    {
        _clienteHandler = clienteHandler; //
        _logger = logger; //
    }

    // GET api/v1/cliente
    /// <summary>Obtiene Todos los Clientes</summary>
    /// <remarks>V1 Permite Obtener los Cliente con un limite de 10 registros</remarks>
    [MapToApiVersion("1")] //
    [HttpGet] //
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))] //
    [ProducesResponseType(StatusCodes.Status204NoContent)] //
    public async Task<IActionResult> GetAllAsync() //
    {
        var response = await _clienteHandler.GetClientes(); //
        if (response.Data == null || !response.Data.Any()) return NoContent(); //
        _logger.LogInformation("respuesta OK"); //
        return Ok(response); //
    }

    // GET api/v1/cliente/5
    /// <summary>Obtiene Cliente segun ID</summary>
    /// <remarks>V1 Permite Obtener un Cliente segun su ID</remarks>
    /// <param name="id" example="5">id cliente</param>
    [MapToApiVersion("1")] //
    [HttpGet("{id:int?}")] //
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))] //
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Para cuando no se encuentra por ID
    public async Task<IActionResult> GetByIdAsync(int? id) //
    {
        if (id == null) return BadRequest("El ID no puede ser nulo.");
        var response = await _clienteHandler.GetClienteId(id); //
        if (response.Data == null || !response.Data.Any()) return NoContent();
        return Ok(response); //
    }

    // GET api/v2/cliente?Nombre=Mauricio
    /// <summary>Obtiene Clientes segun busqueda</summary>
    /// <remarks>V2 Permite Obtener Clientes que hagan mach en la busqueda con un limite de 10 registros</remarks>
    [MapToApiVersion("2")] //
    [HttpGet] //
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataCliente))] //
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByAnyAsync([FromQuery] SetClienteAny cliente) //
    {
        var response = await _clienteHandler.GetClientesAny(cliente); //
        if (response.Data == null || !response.Data.Any()) return NoContent();
        return Ok(response); //
    }

    // POST api/v1/cliente
    /// <summary>Inserta Clientes</summary>
    /// <remarks>V1 Permite Insertar lista de Clientes</remarks>
    [MapToApiVersion("1")] //
    [HttpPost] //
    [ProducesResponseType(StatusCodes.Status201Created)] // Debería ser 201 Created
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] List<SetCliente> clientes) //
    {
        if (clientes == null || !clientes.Any()) return BadRequest("La lista de clientes no puede estar vacía.");
        // Aquí podrías añadir validación de cada cliente en la lista si es necesario
        await _clienteHandler.PostClientes(clientes); //
        // Idealmente, PostClientes debería devolver algo para generar una respuesta CreatedAtAction o similar.
        // Por ahora, devolvemos un Ok genérico o un Created sin ubicación.
        return StatusCode(StatusCodes.Status201Created, "Clientes creados exitosamente."); //
    }

    // PUT api/v1/cliente
    /// <summary>Actualiza Cliente</summary>
    /// <remarks>V1 Permite Actualizar un Cliente segun su Id</remarks>
    [MapToApiVersion("1")] //
    [HttpPut] // // Podría ser HttpPut("{id}") para pasar el ID en la ruta
    [ProducesResponseType(StatusCodes.Status200OK)] //
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para validaciones
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el cliente.Id no existe
    //ProducesResponseType(StatusCodes.Status409Conflict) no es estándar para PUT, NotFound es más común si no existe.
    public async Task<IActionResult> UpdateAsync([FromBody] SetClienteId cliente) //
    {
        if (cliente == null || cliente.Id == null) return BadRequest("Datos de cliente inválidos.");
        var result = await _clienteHandler.UpdateCliente(cliente); //
        if (!result) return NotFound($"Cliente con ID {cliente.Id} no encontrado."); //
        return Ok("Cliente actualizado exitosamente."); //
    }

    // DELETE api/v1/cliente/5
    /// <summary>Elimina Cliente</summary>
    /// <remarks>V1 Permite Eliminar un Cliente segun su Id</remarks>
    [MapToApiVersion("1")] //
    [HttpDelete("{id:int?}")] //
    [ProducesResponseType(StatusCodes.Status200OK)] // O Status204NoContent si no se devuelve contenido
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el ID no existe
    //ProducesResponseType(StatusCodes.Status409Conflict) no es estándar para DELETE.
    public async Task<IActionResult> DeleteAsync(int? id) //
    {
        if (id == null) return BadRequest("El ID no puede ser nulo.");
        var result = await _clienteHandler.DeleteCliente(id); //
        if (!result) return NotFound($"Cliente con ID {id} no encontrado."); //
        return Ok($"Cliente con ID {id} eliminado exitosamente."); //
    }
}