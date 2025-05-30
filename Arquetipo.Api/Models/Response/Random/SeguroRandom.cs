namespace Arquetipo.Api.Models.Response.Random;

public class SeguroRandom
{
    /// <summary>
    /// Nombre de Poliza
    /// </summary>
    /// <example>Seguro de Vida</example>
    public string Nombre { get; set; }
    /// <summary>
    /// numero de Poliza
    /// </summary>
    /// <example>45235452</example>
    public int Poliza { get; set; }

}