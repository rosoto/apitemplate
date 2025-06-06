using Arquetipo.Api.Models.Response.ApiOperaciones;
using Arquetipo.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Arquetipo.Api.UnitTests
{
    [TestFixture]
    public class ApiOperacionesClientTests
    {
        // Mocks para las dependencias
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<ILogger<OperacionesApiClient>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private HttpClient _httpClient;

        // La instancia de la clase que vamos a probar
        private OperacionesApiClient _apiClient;

        [SetUp]
        public void Setup()
        {
            // Creamos nuevas instancias para cada prueba
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<OperacionesApiClient>>();
            _configurationMock = new Mock<IConfiguration>();

            // Creamos un HttpClient real, pero le pasamos nuestro handler simulado.
            // Así, cuando el cliente haga una llamada, interceptaremos esa llamada.
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://tests.com/apioperaciones/")
            };

            // Configuramos el mock de IConfiguration para que devuelva las credenciales de prueba
            var mockApiOperacionesSection = new Mock<IConfigurationSection>();
            mockApiOperacionesSection.Setup(x => x["Usuario"]).Returns("testuser");
            mockApiOperacionesSection.Setup(x => x["Password"]).Returns("testpass");
            _configurationMock.Setup(x => x.GetSection("ApiOperaciones")).Returns(mockApiOperacionesSection.Object);

            // Creamos la instancia real del cliente con sus dependencias (reales y simuladas)
            _apiClient = new OperacionesApiClient(_httpClient, _loggerMock.Object, _configurationMock.Object);
        }

        [Test]
        public async Task GetTasaDeCambioAsync_CuandoApiEsExitosa_DebeDevolverTasaDeCambio()
        {
            // --- ARRANGE ---
            // 1. Preparamos la respuesta JSON que esperamos del servicio externo.
            var responsePayload = new OperacionesApiResponse<TasaDeCambioItem>
            {
                Status = "200",
                Comentario = "OK",
                SessionId = "session-123",
                Data = new List<TasaDeCambioItem>
                {
                    new TasaDeCambioItem { TasaCambio = 36.5m, FechaCambio = "06-06-2025" }
                }
            };
            var serializedPayload = JsonSerializer.Serialize(responsePayload);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serializedPayload)
            };

            // 2. Configuramos el mock del HttpMessageHandler para que devuelva nuestra respuesta simulada.
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // --- ACT ---
            // 3. Ejecutamos el método a probar.
            var resultado = await _apiClient.GetTasaDeCambioAsync(new DateTime(2025, 6, 6), "UF");

            // --- ASSERT ---
            // 4. Verificamos que el resultado es el esperado.
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(1);
            resultado.Data[0].TasaCambio.Should().Be(36.5m);
        }

        [Test]
        public async Task GetTasaDeCambioAsync_CuandoApiDevuelveError_DebeLanzarHttpRequestException()
        {
            // --- ARRANGE ---
            // 1. Configuramos el handler para que simule un error del servidor.
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Error interno del servidor")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // --- ACT & ASSERT ---
            // 2. Ejecutamos el método y afirmamos que lanza la excepción esperada.
            Func<Task> act = async () => await _apiClient.GetTasaDeCambioAsync(new DateTime(2025, 6, 6), "UF");

            await act.Should().ThrowAsync<HttpRequestException>();
        }

        // --- PRUEBA PARA FERIADOS ---

        [Test]
        public async Task GetFeriadosLegalesAsync_CuandoApiEsExitosa_DebeDevolverListaDeFeriados()
        {
            // --- ARRANGE ---
            var responsePayload = new OperacionesApiResponse<FeriadoLegalItem>
            {
                Status = "200",
                Comentario = "OK",
                SessionId = "session-456",
                Data = new List<FeriadoLegalItem>
                {
                    new FeriadoLegalItem { Anio = 2025, Mes = 6, Dia = 27, DiaSemana = "5", EsFeriado = "S" }
                }
            };
            var serializedPayload = JsonSerializer.Serialize(responsePayload);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serializedPayload)
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // --- ACT ---
            var resultado = await _apiClient.GetFeriadosLegalesAsync(new DateTime(2025, 6, 1), new DateTime(2025, 6, 30));

            // --- ASSERT ---
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(1);
            resultado.Data[0].Dia.Should().Be(27);
            resultado.Data[0].EsFeriado.Should().Be("S");
        }
    }
}