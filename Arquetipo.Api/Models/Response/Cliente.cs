using Arquetipo.Api.Models.Header;

namespace Arquetipo.Api.Models.Response;

// es el objeto return del Repository
public class Cliente
{
    /// <summary>
    /// Id de Cliente
    /// </summary>
    /// <example>1234</example>
    public int Id { get; set; }
    /// <summary>
    /// Nombre de Cliente
    /// </summary>
    /// <example>Juan Pablo</example>
    public string Nombre { get; set; }
    /// <summary>
    /// Apellidos de Cliente
    /// </summary>
    /// <example>Soto Rojas</example>
    public string Apellido { get; set; }
    /// <summary>
    /// Mail del Cliente
    /// </summary>
    /// <example>sotorojas@mail.com</example>
    public string Email { get; set; }
    /// <summary>
    /// Telefono del Cliente
    /// </summary>
    /// <example>457892</example>
    public string Telefono { get; set; }
}