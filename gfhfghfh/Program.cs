using Arquetipo.Api.Handlers;
using Arquetipo.Api.Infrastructure;
using Arquetipo.Api.Infrastructure.Persistence;
using Arquetipo.Api.Controllers;
using Arquetipo.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Scalar.AspNetCore;
using Asp.Versioning.ApiExplorer;
using Arquetipo.Api.HttpHandlers;
using Arquetipo.Api.Services;
using System.Net.Http.Headers;
using Arquetipo.Api.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ArquetipoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteHandler, ClienteHandler>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<ForwardRequestIdDelegatingHandler>();
builder.Services.AddTransient<ITokenService, TokenService>();

string? operacionesApiHost = builder.Configuration["ApiOperaciones:Url"];
builder.Services.AddHttpClient<IApiOperacionesClient, OperacionesApiClient>(client =>
{
    // Configura la BaseAddress completa aquí
    client.BaseAddress = new Uri(operacionesApiHost);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<ForwardRequestIdDelegatingHandler>(); // Añade tu handler para propagar X-Request-ID

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirApiRequest", policy =>
        policy.WithOrigins("*") 
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .AllowAnyHeader());
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configuración de Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt"); //
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)), //
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"], //
        ValidateAudience = true, // Considera si necesitas esto y configúralo en appsettings
        ValidAudience = jwtSettings["Issuer"], // A menudo el mismo que el Issuer, o una audiencia específica
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>(); // Para logging

            // Obtener el identificador del usuario desde los claims del token
            // Puede ser ClaimTypes.NameIdentifier (si lo añadiste al token) o JwtRegisteredClaimNames.Sub (username)
            var userIdentifierClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier) ?? context.Principal.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdentifierClaim == null || string.IsNullOrEmpty(userIdentifierClaim.Value))
            {
                logger.LogWarning("Token validado pero no contiene un identificador de usuario (sub o nameidentifier).");
                context.Fail("Token inválido: Falta identificador de usuario.");
                return;
            }

            var userIdentifier = userIdentifierClaim.Value;
            Usuario usuario = null;

            // Decide si identificas por ID o por NombreUsuario
            if (int.TryParse(userIdentifier, out int userId)) // Si el identificador es el ID numérico
            {
                // Necesitarías un método GetUsuarioPorIdAsync en tu IUsuarioRepository
                // usuario = await userRepository.GetUsuarioPorIdAsync(userId); 
                // Por ahora, asumimos que el identificador es NombreUsuario si no es un ID numérico
                // O si siempre usas NombreUsuario como 'sub', entonces:
                usuario = await userRepository.GetUsuarioPorNombreAsync(userIdentifier);
            }
            else // Si el identificador es el NombreUsuario (del claim 'sub')
            {
                usuario = await userRepository.GetUsuarioPorNombreAsync(userIdentifier);
            }


            if (usuario == null || !usuario.EstaActivo)
            {
                logger.LogWarning("Autenticación fallida para el usuario (desde token): {UserIdentifier}. Usuario no encontrado o inactivo en la base de datos.", userIdentifier);
                context.Fail("Usuario no autorizado o inactivo."); // Esto resultará en un 401
            }
            else
            {
                logger.LogInformation("Token validado exitosamente contra la base de datos para el usuario: {UserIdentifier}", userIdentifier);
                // Opcional: Podrías refrescar claims aquí si fuera necesario, aunque es más complejo.
                // Por ejemplo, si los roles del usuario cambian frecuentemente y quieres que el ClaimsPrincipal actual
                // refleje los roles más recientes de la base de datos en lugar de solo los del token original.
                // var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                // // Remover claims de rol antiguos y añadir los nuevos desde usuario.Roles
            }
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var app = builder.Build();

app.UseRequestId();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestIdMiddleware>();

app.UseSwagger();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
List<string> versionStrings = apiVersionDescriptionProvider.ApiVersionDescriptions
    .Select(description => description.ApiVersion.ToString())
    .Distinct()
    .OrderBy(version => version) 
    .ToList();

app.MapScalarApiReference(options =>
{
    options
        .AddDocument("v1", "v1", "swagger/v1/swagger.json")
        .AddDocument("v2", "v2", "swagger/v2/swagger.json")
        .Theme = ScalarTheme.Default;
});

app.UseRouting();
app.UseHttpsRedirection();

app.UseCors("PermitirApiRequest");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();