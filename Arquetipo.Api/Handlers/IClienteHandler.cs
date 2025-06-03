using Arquetipo.Api.Models.Request; //
using Arquetipo.Api.Models.Response; //
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arquetipo.Api.Handlers; //

public interface IClienteHandler
{
    Task<DataCliente> GetClientes(int page, int pageSize); //
    Task<DataCliente> GetClienteId(int? Id); //
    Task<DataCliente> GetClientesAny(SetClienteAny cliente); //
    Task PostClientes(List<SetCliente> clientes); //
    Task<bool> UpdateCliente(SetClienteId cliente); //
    Task<bool> DeleteCliente(int? IdCliente); //
}