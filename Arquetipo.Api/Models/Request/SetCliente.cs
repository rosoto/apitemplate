using System.ComponentModel.DataAnnotations;

namespace Arquetipo.Api.Models.Request;

public class SetCliente
{
    /// <summary>
    /// Nombre Cliente.
    /// </summary>
    /// <example>Mauricio</example>
    [Required]
    public string Nombre { get; set; }
    /// <summary>
    /// Apellido Cliente.
    /// </summary>
    /// <example>Perez Tapia</example>
    [Required]
    public string Apellido { get; set; }
    /// <summary>
    /// Mail
    /// </summary>
    /// <example>m.tapia@mail.cl</example>
    [Required]
    [EmailAddress(ErrorMessage = "El campo Email no contiene un formato válido.")]
    public string Email { get; set; }
    /// <summary>
    /// Telefono
    /// </summary>
    /// <example>975648375</example>
    [Required]
    public string Telefono { get; set; }
}