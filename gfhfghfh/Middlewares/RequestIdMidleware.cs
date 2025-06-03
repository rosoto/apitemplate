using Microsoft.Extensions.Primitives;

namespace Arquetipo.Api.Middlewares
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestIdMiddleware> _logger; // Opcional, para loguear la generación del ID
        public const string RequestIdHeaderName = "X-Request-ID";

        public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestId;

            // 1. Verificar si el header X-Request-ID ya viene en la solicitud
            if (context.Request.Headers.TryGetValue(RequestIdHeaderName, out StringValues existingRequestId) &&
                !StringValues.IsNullOrEmpty(existingRequestId))
            {
                requestId = existingRequestId.ToString();
            }
            else
            {
                // 2. Si no existe, generar un nuevo Request ID
                requestId = Guid.NewGuid().ToString();
            }

            // Guardar el Request ID en HttpContext.Items para que otros componentes puedan acceder a él
            // durante el ciclo de vida de la solicitud.
            context.Items[RequestIdHeaderName] = requestId;

            // 3. Añadir el Request ID a las cabeceras de la respuesta
            // Esto se hace antes de que la respuesta se envíe al cliente.
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(RequestIdHeaderName))
                {
                    context.Response.Headers.Append(RequestIdHeaderName, requestId);
                }
                return Task.CompletedTask;
            });

            // Opcional: Usar ILogger Scopes para incluir el Request ID en todos los logs generados
            // durante esta solicitud. Esto es muy útil para la trazabilidad.
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [RequestIdHeaderName] = requestId
            }))
            {
                _logger.LogInformation("Procesando solicitud con X-Request-ID: {RequestId}", requestId);
                await _next(context);
            }
        }
    }

    // Clase de extensión para registrar el middleware fácilmente
    public static class RequestIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIdMiddleware>();
        }
    }
}
