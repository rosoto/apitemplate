using System.ComponentModel.DataAnnotations;

namespace Arquetipo.Api.Models.Request;

public class RandomItemRequest
{
    /// <summary>
    /// Id del Request.
    /// </summary>
    /// <example>4</example>
    [Required]
    public int? Id { get; set; }
    /// <summary>
    /// Descripcion Objeto.
    /// </summary>
    /// <example>Descripcion Objeto</example>
    [Required]
    public string Descripcion { get; set; }
    /// <summary>
    /// Fecha Objeto.
    /// </summary>
    /// <example>2015-06-19T12-01-03.45Z</example>
    [Required]
    public DateTime? Fecha { get; set; }
}

