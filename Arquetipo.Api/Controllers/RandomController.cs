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
public class RandomController : ControllerBase
{
    private readonly ILogger<RandomController> _logger;
    private readonly Random _random;
    private readonly IRandomHandler _randomHandler;

    public RandomController(ILogger<RandomController> logger,
                           IRandomHandler randomHandler)
    {
        _logger = logger;
        _random = new Random();
        _randomHandler = randomHandler;
    }

    /// <summary>Obtiene Numero Random</summary>
    /// <remarks>V1 Permite Obtener numnero aleatorio entre 1 y 100</remarks>
    [MapToApiVersion("1")]
    [HttpGet("number")]
    public IActionResult GetRandomNumber()
    {
        int randomNumber = _random.Next(1, 101); // Genera un número aleatorio entre 1 y 100
        return Ok(randomNumber);
    }

    /// <summary>Obtiene Numero Random Version 2</summary>
    /// <remarks>V2 Permite Obtener numnero aleatorio entre 1 y 100</remarks>
    [MapToApiVersion("2")]
    [HttpGet("number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetNowRandomNumber()
    {
        int randomNumberNew = _random.Next(1, 101); // Genera un número aleatorio entre 1 y 100
        return Ok(randomNumberNew);
    }


    /// <summary>Obtiene Response de ejemplo</summary>
    /// <remarks>Permite obtener una respuesta Json de tipo estandar </remarks>
    [HttpGet("exampleResponse")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDataExample))]
    public async Task<IActionResult> GetExampleresponse()
    {
        var response = await _randomHandler.GeDataRandomDammy();
        return Ok(response);
    }

    /// <summary>Obtiene Response Error aleatorio</summary>
    /// <remarks>Permite obtener una 200 o 500 de forma aleatoria</remarks>
    [MapToApiVersion("2")]
    [HttpGet("error")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetRandomError()
    {
        bool shouldGenerateError = _random.Next(0, 2) == 1; // Decide aleatoriamente si se debe generar un error

        if (shouldGenerateError)
        {
            _logger.LogError("Un error aleatorio ha sido generado.");
            return StatusCode(500, "Se generó un error aleatorio.");
        }
        else
        {
            return Ok("Todo salió bien esta vez.");
        }
    }

    /// <summary>Crea un Item de tipo Random</summary>
    /// <remarks>Permite simular un insert a base de datos</remarks>
    [MapToApiVersion("1")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult CreateRandomItem([FromBody]RandomItemRequest request)
    {
        int randomNumber = _random.Next(1, 101);
        return CreatedAtAction(nameof(GetRandomNumber), new { id = randomNumber }, randomNumber);
    }

    /// <summary>Actualiza un Item de tipo Random V2</summary>
    /// <remarks>Permite simular un update en base de datos</remarks>
    [MapToApiVersion("2")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateRandomItem(int id)
    {
        if (id < 1 || id > 100)
        {
            return BadRequest("El ID proporcionado no es válido. Debe estar entre 1 y 100.");
        }

        int randomNumber = _random.Next(1, 101);
        return Ok($"El elemento con ID {id} ha sido actualizado con el número aleatorio {randomNumber}.");
    }

    /// <summary>Elimina un Item de tipo Random</summary>
    /// <remarks>Permite simular un delete a base de datos</remarks>
    [MapToApiVersion("2")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteRandomItem(int id)
    {
        if (id < 1 || id > 100)
        {
            return BadRequest("El ID proporcionado no es válido. Debe estar entre 1 y 100.");
        }

        return Ok($"El elemento con ID {id} ha sido eliminado.");
    }
}