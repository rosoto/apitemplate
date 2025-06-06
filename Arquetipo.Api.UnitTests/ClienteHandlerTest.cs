using Arquetipo.Api.Handlers;
using Arquetipo.Api.Infrastructure;
using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Request.v1;
using Arquetipo.Api.Models.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Arquetipo.Api.UnitTests
{
    [TestFixture]
    public class ClienteHandlerTests
    {
        // Mocks para las dependencias del handler
        private Mock<IClienteRepository> _clienteRepositoryMock;
        private Mock<ILogger<ClienteHandler>> _loggerMock;

        // La instancia de la clase que vamos a probar
        private ClienteHandler _clienteHandler;

        [SetUp]
        public void Setup()
        {
            // Esta sección se ejecuta antes de cada prueba.
            // Creamos nuevas instancias de los mocks para asegurar que cada prueba esté aislada.
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _loggerMock = new Mock<ILogger<ClienteHandler>>();

            // Creamos una instancia real del Handler, pero le pasamos los mocks como dependencias.
            _clienteHandler = new ClienteHandler(_clienteRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetClienteByIdV1Async_CuandoClienteExiste_DebeDevolverDatosDelCliente()
        {
            // --- ARRANGE (Organizar) ---
            // 1. Preparamos los datos de entrada y los datos que esperamos que el mock devuelva.
            var idCliente = 1;
            var clienteDePrueba = new Cliente
            {
                Id = idCliente,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan.perez@test.com",
                Telefono = "12345678"
            };

            // 2. Configuramos el mock del repositorio.
            // Le decimos: "Cuando se llame al método GetByIdAsync con el id 1,
            // quiero que simules que encontraste en la BD y devuelvas el clienteDePrueba".
            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(idCliente))
                                  .ReturnsAsync(clienteDePrueba);

            // --- ACT (Actuar) ---
            // 3. Ejecutamos el método que queremos probar.
            var resultado = await _clienteHandler.GetClienteByIdV1Async(idCliente);

            // --- ASSERT (Afirmar) ---
            // 4. Verificamos que el resultado es el que esperábamos.
            resultado.Should().NotBeNull(); // El resultado general no debe ser nulo.
            resultado.Data.Should().NotBeNull(); // La lista de datos no debe ser nula.
            resultado.Data.Should().HaveCount(1); // Debería haber exactamente un cliente en la lista.

            // Verificamos que el mapeo de Cliente a ClienteResponseV1 fue correcto.
            var clienteEncontrado = resultado.Data[0];
            clienteEncontrado.Id.Should().Be(clienteDePrueba.Id);
            clienteEncontrado.Nombre.Should().Be(clienteDePrueba.Nombre);
            clienteEncontrado.Email.Should().Be(clienteDePrueba.Email);
            clienteEncontrado.Telefono.Should().Be(clienteDePrueba.Telefono);

            // 5. (Opcional pero recomendado) Verificar que el método del mock se llamó como esperábamos.
            _clienteRepositoryMock.Verify(repo => repo.GetByIdAsync(idCliente), Times.Once);
        }

        [Test]
        public async Task GetClienteByIdV1Async_CuandoClienteNoExiste_DebeDevolverListaVacia()
        {
            // --- ARRANGE ---
            var idCliente = 999;

            // Configuramos el mock para que devuelva null, simulando que no encontró el cliente.
            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(idCliente))
                                  .ReturnsAsync((Cliente)null);

            // --- ACT ---
            var resultado = await _clienteHandler.GetClienteByIdV1Async(idCliente);

            // --- ASSERT ---
            resultado.Should().NotBeNull();
            resultado.Data.Should().NotBeNull();
            resultado.Data.Should().BeEmpty(); // La lista de datos debe estar vacía.
        }

        [Test]
        public async Task PostClientesV1Async_ConDatosValidos_DebeMapearYLLamarAlRepositorio()
        {
            // --- ARRANGE ---
            // 1. Preparamos el objeto de solicitud que llegaría desde el controlador.
            var solicitudCrearCliente = new List<CrearClienteRequestV1>
    {
        new CrearClienteRequestV1
        {
            Nombre = "Nuevo",
            Apellido = "Cliente",
            Email = "nuevo.cliente@test.com",
            Telefono = "5551234"
        }
    };

            // 2. (Opcional pero recomendado) Para verificar que el mapeo fue correcto,
            // capturamos la lista que el handler le pasa al repositorio.
            List<SetCliente> listaCapturada = null;
            _clienteRepositoryMock
                .Setup(repo => repo.AddClientesAsync(It.IsAny<List<SetCliente>>()))
                // Callback nos permite "espiar" el argumento pasado al método del mock.
                .Callback<List<SetCliente>>(lista => listaCapturada = lista)
                .Returns(Task.CompletedTask); // Como AddClientesAsync no devuelve nada, completamos la tarea.

            // --- ACT ---
            // 3. Ejecutamos el método a probar.
            await _clienteHandler.PostClientesV1Async(solicitudCrearCliente);

            // --- ASSERT ---
            // 4. Afirmamos que el método AddClientesAsync del repositorio fue llamado una sola vez.
            _clienteRepositoryMock.Verify(repo => repo.AddClientesAsync(It.IsAny<List<SetCliente>>()), Times.Once);

            // 5. Afirmamos que la lista capturada (los datos ya mapeados) es correcta.
            listaCapturada.Should().NotBeNull();
            listaCapturada.Should().HaveCount(1);
            listaCapturada[0].Nombre.Should().Be(solicitudCrearCliente[0].Nombre);
            listaCapturada[0].Email.Should().Be(solicitudCrearCliente[0].Email);
        }
    }
}