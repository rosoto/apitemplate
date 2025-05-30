namespace Arquetipo.Api.Models.Response
{
    public class UserJwtData
    {
        /// <summary>
        /// Id de claims del token
        /// </summary>
        /// <example>1234</example>
        public string Id { get; set; }
        /// <summary>
        /// Mail del claim del token
        /// </summary>
        /// <example>usuarioName@dominio.com</example>
        public string Email { get; set; }
    }
}