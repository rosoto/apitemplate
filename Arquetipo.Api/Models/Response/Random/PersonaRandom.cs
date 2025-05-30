namespace Arquetipo.Api.Models.Response.Random;

public class PersonaRandom
{
    /// <summary>
    /// Nombre Completo
    /// </summary>
    /// <example>Jaime Lopez Zamora</example>
    public string Nombre { get; set; }
    /// <summary>
    /// Edad en años
    /// </summary>
    /// <example>45</example>
    public int Edad { get; set; }
    /// <summary>
    /// Correo Electronico
    /// </summary>
    /// <example>jretamales@mail.com</example>
    public string Mail { get; set; }
}