using System.ComponentModel.DataAnnotations; // Aunque no se usan atributos de validación aquí, es común tenerlo si se añaden luego.

namespace Arquetipo.Api.Models.Request; //

public class SetClienteAny
{
    /// <summary>
    /// Nombre Cliente.
    /// </summary>
    /// <example>Mauricio</example>
    public string? Nombre { get; set; } //
    /// <summary>
    /// Apellido Cliente.
    /// </summary>
    /// <example>Perez Tapia</example>
    public string? Apellido { get; set; } //
    /// <summary>
    /// Mail
    /// </summary>
    /// <example>m.tapia@mail.cl</example>
    public string? Email { get; set; } //
    /// <summary>
    /// Telefono
    /// </summary>
    /// <example>975648375</example>
    public string? Telefono { get; set; } //
}