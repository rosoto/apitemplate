using System.Text;
using Arquetipo.Api.Controllers; // Asumiendo que es necesario, si no, eliminar.
using Arquetipo.Api.Handlers;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer; // <-- ¡Ubicación correcta!
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options; // Necesario para IConfigureOptions<T>

namespace Arquetipo.Api;

// NOTA IMPORTANTE: En .NET 8, Program.cs suele manejar la configuración de inicio
// sin una clase Startup.cs separada. Si usas Startup.cs, deberás configurar
// Program.cs para que la use.

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // Este método es llamado por el tiempo de ejecución. Úsalo para añadir servicios al contenedor.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomMvc(Configuration)
                .AddHttpServices();
    }

    // Este método es llamado por el tiempo de ejecución. Úsalo para configurar el pipeline de solicitud HTTP.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        // Para desarrollo, normalmente quieres la UI de Swagger.
        // Puedes descomentar la comprobación env.IsDevelopment() si solo quieres Swagger en desarrollo.
        // if (env.IsDevelopment())
        // {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Esto asegura rutas correctas al desplegar en una sub-ruta, si es necesario.
            string swaggerBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"{swaggerBasePath}/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
        // }

        // app.UseHttpsRedirection(); // Descomenta si quieres forzar HTTPS

        app.UseRouting();

        app.UseCors("PermitirApiRequest"); // Aplica la política CORS nombrada (definida una sola vez)

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddScoped<IRandomHandler, RandomHandler>();
        // services.AddScoped<IClienteRepository, ClienteRespository>();
        // services.AddScoped<IClienteHandler, ClienteHandler>();
        // services.AddScoped<IRandomHandler3, RandomHandler3>();
        // services.AddScoped<IRandomHandler4, RandomHandler4>();

        services.AddControllers();

        // Configura la política CORS (solo una vez)
        services.AddCors(options =>
        {
            options.AddPolicy("PermitirApiRequest",
                builder => builder.WithOrigins("*").WithMethods("GET", "POST", "PUT").AllowAnyHeader());
        });

        // Configura el versionado de API (correctamente)
        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.ReportApiVersions = true;
            // Configura el explorador de API para integrarse con Swagger
            setup.ApiExplorer.GroupNameFormat = "'v'V";
            setup.SubstituteApiVersionInUrl = true; // Sustituye la versión de la API en la ruta de la URL
        })
        .AddApiExplorer(setup => // Llamada separada para añadir los servicios del explorador de API
        {
            // Configura el explorador de API para generar documentos Swagger separados para cada versión de API
            setup.GroupNameFormat = "'v'VVV"; // Ej., v1, v2.0
            setup.SubstituteApiVersionInUrl = true;
        });


        // Configura la autenticación JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Audience = configuration["Jwt:Issuer"]; // Considera `ValidAudience` en lugar de `Audience` para múltiples audiencias.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"], // Asegúrate de que esto coincida con `options.Audience` si el emisor es el mismo
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Issuer"],
                ValidateLifetime = true
            };
        });

        // Aprende más sobre la configuración de Swagger/OpenAPI en https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        // Esta línea asume que tienes una clase ConfigureSwaggerOptions en tu proyecto.
        // Se usa para configuraciones avanzadas de Swagger, especialmente con el versionado de API.
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }

    public static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        // services.AddHttpClient<IComisionApiClient, ComisionApiClient>();
        // services.AddHttpClient<IPagoComisionApiClient, PagoComisionApiClient>();
        return services;
    }
}