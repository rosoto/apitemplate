using System.Security.Claims;
using Arquetipo.Api.Models.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Arquetipo.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableCors("PermitirApiRequest")]
public class UserJWTController : ControllerBase
{
    private readonly ILogger<UserJWTController> _logger;
    public UserJWTController(ILogger<UserJWTController> logger)
    {
        _logger = logger;
    }

    /// <summary>Obtiene Informacion del Usario del JWT</summary>
    /// <remarks>Permite Obtener Info del usriario del JWT</remarks>
    [MapToApiVersion("1")]
    [Authorize]
    [HttpGet("userInfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseUserJwt))]
    public async Task<IActionResult> GetUserInfoAsync()
    {
        _logger.LogInformation("Ejecucion service userInfo");
        await Task.Delay(1000);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // lo obtiene del claim del token, debe existir la propiedad en el token
        var userEmail = User.FindFirstValue(ClaimTypes.Email); //
        // ejemplo response con herencia de clase
        var response = new ResponseUserJwt { UsersJwt = new UserJwtData { Id = userId, Email = userEmail}  };
        return Ok(response);
    }

}