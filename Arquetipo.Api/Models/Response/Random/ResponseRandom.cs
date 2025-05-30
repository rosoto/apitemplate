namespace Arquetipo.Api.Models.Response.Random;

public class ResponseRandom
{
    public PersonaRandom DatosPersonales { get; set; }
    public List<SeguroRandom> SegurosAsociados { get; set; }
}