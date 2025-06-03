using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arquetipo.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string requestId = context.Items[RequestIdMiddleware.RequestIdHeaderName] as string ?? "N/A-From-ExceptionHandler";
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada (X-Request-ID: {RequestId}): {Message}", requestId, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response;
            if (_env.IsDevelopment())
            {
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message, // Mensaje detallado en desarrollo
                    InnerException = ex.InnerException?.Message, // Uso de operador de acceso condicional para evitar la desreferencia de una referencia NULL
                    StackTrace = ex.StackTrace?.ToString() // Uso de operador de acceso condicional para evitar la desreferencia de una referencia NULL
                };
            }
            else
            {
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Ocurrió un error interno en el servidor."
                };
            }
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}