﻿using Asp.Versioning.ApiExplorer; // Necesario para IApiVersionDescriptionProvider con Asp.Versioning
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System; // Para Array.Empty<string>()
using System.IO; // Para Path.Combine
using System.Reflection; // Para Assembly.GetExecutingAssembly()

// Asegúrate de que este namespace coincida con la ubicación real del archivo
// y con lo que se registra en Program.cs
namespace Arquetipo.Api.Controllers; // Este es el namespace en tu archivo original

/// <summary>
/// Configura las opciones de SwaggerGen para la aplicación.
/// Esta clase es responsable de generar un documento Swagger por cada versión de API
/// y de configurar la seguridad JWT en la UI de la documentación.
/// </summary>
/// <remarks>
/// Permite que la versionado de API defina un documento Swagger por cada versión de API
/// después de que el servicio IApiVersionDescriptionProvider haya sido resuelto del contenedor de servicios.
/// </remarks>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider; //

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ConfigureSwaggerOptions"/>.
    /// </summary>
    /// <param name="provider">El <see cref="IApiVersionDescriptionProvider"/> utilizado para generar los documentos Swagger.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) //
    {
        _provider = provider; //
    }

    /// <summary>
    /// Configura la instancia de <see cref="SwaggerGenOptions"/>.
    /// </summary>
    /// <param name="options">Las <see cref="SwaggerGenOptions"/> a configurar.</param>
    public void Configure(SwaggerGenOptions options) //
    {
        // Añadir definición de seguridad para JWT Bearer
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //
        {
            Name = "Authorization", //
            Type = SecuritySchemeType.ApiKey, //
            Scheme = "Bearer", //
            BearerFormat = "JWT", //
            In = ParameterLocation.Header, //
            Description = "Encabezado de autorización JWT usando el esquema Bearer. \r\n\r\n " +
                          "Ingresa 'Bearer' [espacio] y luego tu token en la entrada de texto de abajo.\r\n\r\n" +
                          "Ejemplo: \"Bearer 12345abcdef\"", //
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement //
        {
            {
                new OpenApiSecurityScheme //
                {
                    Reference = new OpenApiReference //
                    {
                        Type = ReferenceType.SecurityScheme, //
                        Id = "Bearer" //
                    }
                },
                Array.Empty<string>() //
            }
        });

        // Añadir un documento swagger por cada versión de API descubierta
        foreach (var description in _provider.ApiVersionDescriptions) //
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description)); //
        }

        // Integrar comentarios XML
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); //
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath); //
        }
    }

    /// <summary>
    /// Crea la información de <see cref="OpenApiInfo"/> para una versión de API dada.
    /// </summary>
    /// <param name="description">La <see cref="ApiVersionDescription"/> de la API.</param>
    /// <returns>Una instancia de <see cref="OpenApiInfo"/>.</returns>
    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description) //
    {
        var info = new OpenApiInfo() //
        {
            Title = "Arquetipo API", //
            Version = description.ApiVersion.ToString(), //
            Description = $"Documentación para la API Arquetipo {description.ApiVersion}.",
            // Puedes añadir más detalles si lo deseas:
            // Contact = new OpenApiContact
            // {
            //     Name = "Equipo de Desarrollo",
            //     Email = "desarrollo@example.com",
            //     Url = new Uri("https://example.com/contact")
            // },
            // License = new OpenApiLicense
            // {
            //     Name = "Usar bajo Licencia XYZ",
            //     Url = new Uri("https://example.com/license")
            // }
        };

        if (description.IsDeprecated) //
        {
            info.Description += " Esta versión de API ha sido marcada como obsoleta."; //
        }

        return info; //
    }
}