namespace Arquetipo.Api.Middlewares
{
    public class RemovePoweredByMiddleware
    {
        private readonly RequestDelegate _next;
        private const string PoweredByHeaderName = "X-Powered-By";

        public RemovePoweredByMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Registrar un callback para modificar las cabeceras antes de que se envíen
            context.Response.OnStarting(() =>
            {
                if (context.Response.Headers.ContainsKey(PoweredByHeaderName))
                {
                    context.Response.Headers.Remove(PoweredByHeaderName);
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    // Clase de extensión para registrar el middleware fácilmente
    public static class RemovePoweredByHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseRemovePoweredByHeader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RemovePoweredByMiddleware>();
        }
    }
}