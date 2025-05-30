using Arquetipo.Api.Models.Response;

namespace Arquetipo.Api.Handlers;

public interface IRandomHandler
{
    Task<ResponseDataExample> GeDataRandomDammy();
}