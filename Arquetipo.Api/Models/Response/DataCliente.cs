using Arquetipo.Api.Models.Header;

namespace Arquetipo.Api.Models.Response;

public class DataCliente : HeaderBase
{
    public List<Cliente> Data { get; set; }
}