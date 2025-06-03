namespace Arquetipo.Api.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Estas son las cabeceras mínimas y comunes. Ajusta según el estándar 7.11.
            // Content-Security-Policy es compleja y necesita una configuración cuidadosa.
            // El estándar menciona: "Debe EXISTIR la directiva frame-ancestors con valor 'none'. Si se requiere otro valor, se acepta, pero debe EXISTIR la directiva."

            if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            }

            // X-Frame-Options es más antiguo; CSP frame-ancestors es preferido.
            // if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
            // {
            //     context.Response.Headers.Append("X-Frame-Options", "DENY");
            // }

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                // Ejemplo básico. Debes configurarlo según tus necesidades y el estándar.
                context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");
            }

            if (!context.Response.Headers.ContainsKey("Strict-Transport-Security") && context.Request.IsHttps) // Solo para HTTPS
            {
                // Valor exigido: max-age=31536000; includeSubdomains
                context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }

            if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
            {
                // Valor exigido: same-origin
                context.Response.Headers.Append("Referrer-Policy", "same-origin");
            }

            // Otras cabeceras del estándar 7.11:
            // Permissions-Policy, Cross-Origin-Opener-Policy, etc.
            // Ejemplo para Permissions-Policy (antes Feature-Policy):
            // if (!context.Response.Headers.ContainsKey("Permissions-Policy"))
            // {
            //     context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()"); // Deniega por defecto
            // }


            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
