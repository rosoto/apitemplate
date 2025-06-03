using Arquetipo.Api.Models.Header; //
using System.Collections.Generic; // Necesario para List<T>

namespace Arquetipo.Api.Models.Response; //

public class DataCliente : HeaderBase //
{
    public required List<Cliente> Data { get; set; } //
}