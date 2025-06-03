using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Request.v1;
using Arquetipo.Api.Models.Request.v2;
using Arquetipo.Api.Models.Response.v1;
using Arquetipo.Api.Models.Response.v2;

namespace Arquetipo.Api.Handlers.Mapper
{
    public static class ClienteMapper // Puede ser estática si los mapeos no tienen dependencias o estado
    {
        // --- Mapeos a Response DTOs ---

        public static ClienteResponse ToClienteResponseV1(Models.Response.Cliente repoCliente)
        {
            if (repoCliente == null) return null;
            return new ClienteResponse
            {
                Id = repoCliente.Id,
                Nombre = repoCliente.Nombre,
                Apellido = repoCliente.Apellido,
                Email = repoCliente.Email,
                Telefono = repoCliente.Telefono
            };
        }

        public static ClienteResponseV2 ToClienteResponseV2(Models.Response.Cliente repoCliente)
        {
            if (repoCliente == null) return null;

            // Asumimos que `repoCliente` es tu entidad de base de datos/repositorio.
            // Necesitarás obtener FechaRegistro y EstadoCivil de alguna parte si no están en repoCliente.
            // Si están en repoCliente, mapealos directamente.
            return new ClienteResponseV2
            {
                Id = repoCliente.Id,
                Nombre = repoCliente.Nombre,
                Apellido = repoCliente.Apellido,
                Email = repoCliente.Email,
                Telefono = repoCliente.Telefono,
                // Ejemplo: si tuvieras estos campos en la entidad Cliente del repositorio:
                // EstadoCivil = repoCliente.EstadoCivil, 
                // FechaRegistro = repoCliente.FechaDeCreacionEnBd 
                EstadoCivil = "N/A", // Simulado por ahora
                FechaRegistro = DateTime.UtcNow // Simulado por ahora
            };
        }

        // --- Mapeos desde Request DTOs a Modelos de Repositorio (ej. SetCliente, SetClienteId) ---

        public static SetCliente ToSetCliente(CrearClienteRequestV1 requestV1)
        {
            if (requestV1 == null) return null;
            return new SetCliente // Asumiendo que SetCliente es el modelo que espera tu repositorio para crear
            {
                Nombre = requestV1.Nombre,
                Apellido = requestV1.Apellido,
                Email = requestV1.Email,
                Telefono = requestV1.Telefono
            };
        }

        public static SetClienteId ToSetClienteId(ActualizarClienteRequest requestV1)
        {
            if (requestV1 == null) return null;
            return new SetClienteId
            {
                Id = requestV1.Id,
                Nombre = requestV1.Nombre,
                Apellido = requestV1.Apellido,
                Email = requestV1.Email,
                Telefono = requestV1.Telefono
            };
        }

        // Para V2 (Crear)
        public static SetCliente ToSetCliente(CrearClienteRequestV2 requestV2)
        {
            if (requestV2 == null) return null;
            // Aquí decides cómo mapear los campos de V2 al modelo del repositorio.
            // Por ejemplo, si SetCliente no tiene PreferenciaContacto, lo ignoras o lo logueas.
            return new SetCliente
            {
                Nombre = requestV2.Nombre,
                Apellido = requestV2.Apellido,
                Email = requestV2.Email,
                Telefono = requestV2.Telefono ?? "N/A" // Manejar opcionalidad
                // PreferenciaContacto = requestV2.PreferenciaContacto // Si SetCliente tuviera este campo
            };
        }

        // Para V2 (Actualizar) - Este es más complejo porque es una actualización parcial
        // y necesitas la entidad existente. Podrías tener un método que tome la entidad y el request.
        public static void ApplyUpdate(Models.Response.Cliente entidadCliente, ActualizarClienteRequestV2 requestV2)
        {
            if (entidadCliente == null || requestV2 == null) return;

            if (!string.IsNullOrEmpty(requestV2.Nombre)) entidadCliente.Nombre = requestV2.Nombre;
            if (!string.IsNullOrEmpty(requestV2.Apellido)) entidadCliente.Apellido = requestV2.Apellido;
            if (!string.IsNullOrEmpty(requestV2.Email)) entidadCliente.Email = requestV2.Email;

            // Si Telefono en requestV2 es null, ¿quieres borrar el teléfono en la entidad?
            // Si requestV2.Telefono tiene valor, actualiza.
            // Esto depende de la lógica de negocio para campos opcionales en actualización.
            if (requestV2.Telefono != null) // Podrías querer una forma explícita de borrarlo
            {
                entidadCliente.Telefono = string.IsNullOrEmpty(requestV2.Telefono) ? "N/A" : requestV2.Telefono;
            }

            // Actualizar campos V2 si existen en la entidad Cliente del repositorio
            // entidadCliente.PreferenciaContacto = requestV2.PreferenciaContacto ?? entidadCliente.PreferenciaContacto;
            // entidadCliente.EstadoCivil = requestV2.EstadoCivil ?? entidadCliente.EstadoCivil;
        }

        // Puedes añadir más métodos de mapeo según necesites (ej. para BuscarClienteQueryV1/V2 a SetClienteAny)
        public static SetClienteAny ToSetClienteAny(BuscarClienteRequest query)
        {
            if (query == null) return null;
            return new SetClienteAny
            {
                Nombre = query.Nombre,
                Apellido = query.Apellido,
                Email = query.Email,
                Telefono = query.Telefono
            };
        }

        public static SetClienteAny ToSetClienteAny(BuscarClienteRequestV2 query)
        {
            if (query == null) return null;
            return new SetClienteAny
            {
                Nombre = query.Nombre,
                Apellido = query.Apellido,
                Email = query.Email,
                // Nickname = query.Nickname // Si SetClienteAny tiene un campo para Nickname o mapeas a otro campo
            };
        }
    }
}