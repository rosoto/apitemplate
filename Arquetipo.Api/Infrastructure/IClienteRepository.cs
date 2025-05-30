using Arquetipo.Api.Models.Response;
using Arquetipo.Api.Models.Request;

namespace Arquetipo.Api.Infrastructure;

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente> GetByIdAsync(int? id);
    Task<List<Cliente>> GetByAnyAsync(SetClienteAny cliente);
    Task<bool> ExistsAsync(int? id);
    Task AddClientesAsync(List<SetCliente> clientes);
    Task UpdateAsync(SetClienteId cliente);
    Task DeleteAsync(int? id);
}