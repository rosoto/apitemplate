using Arquetipo.Api.Models.Request; //
using Arquetipo.Api.Models.Response; //

namespace Arquetipo.Api.Infrastructure; //

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync(int page, int pageSize); //
    Task<Cliente> GetByIdAsync(int? id); //
    Task<List<Cliente>> GetByAnyAsync(SetClienteAny cliente); //
    Task<bool> ExistsAsync(int? id); //
    Task AddClientesAsync(List<SetCliente> clientes); //
    Task UpdateAsync(SetClienteId cliente); //
    Task DeleteAsync(int? id); //
}