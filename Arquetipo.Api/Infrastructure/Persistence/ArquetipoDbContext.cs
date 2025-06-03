using Arquetipo.Api.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace Arquetipo.Api.Infrastructure.Persistence
{
    public class ArquetipoDbContext : DbContext
    {
        public ArquetipoDbContext(DbContextOptions<ArquetipoDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } // Usando tu clase Cliente existente como entidad
        public DbSet<Usuario> Usuarios { get; set; } // Asegúrate de tener una clase Usuario definida
    }
}
