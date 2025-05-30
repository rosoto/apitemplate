using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Response;
using Microsoft.Data.SqlClient;

namespace Arquetipo.Api.Infrastructure;

public class ClienteRespository : IClienteRepository
{
    private readonly ILogger<ClienteRespository> _logger;
    private readonly string _connectionString;

    public ClienteRespository(ILogger<ClienteRespository> logger,
                            IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionStrings:Database"];
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            const string Sql = @"SELECT TOP 10
                                    Id
		                            ,Nombre
		                            ,Apellido
		                            ,Email
		                            ,Telefono
                                FROM Cliente";

            var response = await connection.QueryAsync<Cliente>(Sql); //IEnumerable<Cliente>
            return response.AsList();
        }
    }

    public async Task<Cliente> GetByIdAsync(int? id)
    {
        const string Sql = @"SELECT Id
		                            ,Nombre
		                            ,Apellido
		                            ,Email
		                            ,Telefono
                            FROM Cliente
                            WHERE Id = @Id";

        using (var connection = new SqlConnection(_connectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<Cliente>(Sql, new { Id = id });
        }
    }

    public async Task<List<Cliente>> GetByAnyAsync(SetClienteAny cliente)
    {

        const string Sql = @"SELECT TOP 10
                            Id, Nombre, Apellido, Email, Telefono
                            FROM Cliente
                            WHERE
                            (@Nombre IS NULL OR [Nombre] = @Nombre) AND
                            (@Apellido IS NULL OR [Apellido] = @Apellido) AND
                            (@Email IS NULL OR [Email] = @Email) AND
                            (@Telefono IS NULL OR [Telefono] = @Telefono)";

        using (var connection = new SqlConnection(_connectionString))
        {
            var response = await connection.QueryAsync<Cliente>(Sql, cliente);
            return response.AsList();
        }
    }

    public async Task<bool> ExistsAsync(int? id)
    {
        const string Sql = @"SELECT COUNT(*) FROM Cliente WHERE Id = @Id";

        using (var connection = new SqlConnection(_connectionString))
        {
            int count = await connection.ExecuteScalarAsync<int>(Sql, new { Id = id });
            return count > 0;
        }
    }

    public async Task AddClientesAsync(List<SetCliente> clientes)
    {
        const string Sql = @"INSERT INTO Cliente (Nombre, Apellido, Email, Telefono)
                            VALUES (@Nombre, @Apellido, @Email, @Telefono)";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(Sql, clientes);
        }
    }

    public async Task UpdateAsync(SetClienteId cliente)
    {
        const string Sql = @"UPDATE Cliente
                            SET
                                Nombre = @Nombre
                                ,Apellido = @Apellido
                                ,Email = @Email
                                ,Telefono = @Telefono
                            WHERE Id = @Id";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(Sql, cliente);
        }
    }

    public async Task DeleteAsync(int? id)
    {
        const string Sql = @"DELETE FROM Cliente WHERE Id = @Id";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(Sql, new { Id = id });
        }
    }
}