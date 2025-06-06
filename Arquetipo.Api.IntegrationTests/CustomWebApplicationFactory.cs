using Arquetipo.Api.Infrastructure.Persistence;
using Arquetipo.Api.Models.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Arquetipo.Api.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program> // Cambiado a 'Program' sin el prefijo del espacio de nombres
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. Buscamos la configuración del DbContext que usa SQL Server.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ArquetipoDbContext>));

                // Asegúrate de incluir esta directiva using
                // 2. Si existe, la eliminamos.
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 3. Añadimos una nueva configuración de DbContext para usar una BD en memoria.
                // Usamos un nombre de base de datos único para cada ejecución de factory para evitar conflictos.
                services.AddDbContext<ArquetipoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting-" + System.Guid.NewGuid());
                });

                // 4. Obtenemos el proveedor de servicios y poblamos la BD en memoria con datos de prueba.
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ArquetipoDbContext>();
                    db.Database.EnsureCreated(); // Aseguramos que la BD en memoria esté creada.

                    // Añadimos datos de prueba.
                    db.Clientes.Add(new Cliente { Id = 1, Nombre = "Ana", Apellido = "Garcia", Email = "ana.garcia@test.com", Telefono = "87654321" });
                    db.SaveChanges();
                }
            });

            builder.UseEnvironment("Development"); // Especificamos el entorno de ejecución.
        }
    }
}