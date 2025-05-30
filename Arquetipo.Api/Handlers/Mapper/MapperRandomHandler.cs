using Arquetipo.Api.Models.Response.Random;

namespace Arquetipo.Api.Handlers;

public static class MapperRandomHandler
{
    public static PersonaRandom GetPersonaRandom()
    {
        var response = new PersonaRandom { Nombre = " Nombre dammy", Edad = 35, Mail = "as@as.com "};
        return response;
    }

    public static List<SeguroRandom> GetSegurosRandomList()
    {
        var response = new List<SeguroRandom>{
            new SeguroRandom { Nombre = "Seguro Salud", Poliza = 458 },
            new SeguroRandom { Nombre = "Seguro Vida", Poliza = 6897 },
            new SeguroRandom { Nombre = "Seguro Vehiculo", Poliza = 854788 }
        };

        return response;
    }
}