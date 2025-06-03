using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Request.v1;
using Arquetipo.Api.Models.Request.v2;
using Arquetipo.Api.Models.Response;

namespace Arquetipo.Api.Infrastructure
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync(int page, int pageSize);
        Task<Cliente> GetByIdAsync(int? id);

        // CORREGIR AQUÍ: Cambiar el tipo del parámetro a SetClienteAny
        Task<List<Cliente>> GetByAnyAsync(SetClienteAny clienteParams);

        Task<bool> ExistsAsync(int? id);
        Task AddClientesAsync(List<SetCliente> clientes);

        // Decide cuál firma de UpdateAsync quieres mantener o si necesitas ambas con lógica diferente.
        // Por consistencia, recomiendo una sola que use un DTO de repositorio como SetClienteId.
        // Task UpdateAsync(ActualizarClienteRequest cliente); // Si eliminas esta, debes quitar su uso o mapear a SetClienteId en el Handler
        Task UpdateAsync(SetClienteId cliente); // Esta es la que hemos estado usando consistentemente

        Task DeleteAsync(int? id);
    }
}