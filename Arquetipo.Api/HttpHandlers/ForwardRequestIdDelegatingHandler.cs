using Arquetipo.Api.Middlewares;

namespace Arquetipo.Api.HttpHandlers;

public class ForwardRequestIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardRequestIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Intentar obtener el HttpContext actual
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null && httpContext.Items.TryGetValue(RequestIdMiddleware.RequestIdHeaderName, out var requestIdObj) &&
            requestIdObj is string requestId && !string.IsNullOrEmpty(requestId))
        {
            // Añadir el X-Request-ID a la solicitud saliente
            // Asegúrate de no añadirlo si ya existe (aunque es poco probable para esta cabecera específica)
            if (!request.Headers.Contains(RequestIdMiddleware.RequestIdHeaderName))
            {
                request.Headers.Add(RequestIdMiddleware.RequestIdHeaderName, requestId);
            }
        }
        // else
        // {
        // Opcional: Decide qué hacer si no hay Request ID.
        // Podrías generar uno nuevo aquí para la traza saliente, o no añadir nada.
        // Si no se añade, el servicio receptor podría generar el suyo.
        // }

        return await base.SendAsync(request, cancellationToken);
    }
}