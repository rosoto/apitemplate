using Arquetipo.Api.Models.Request;
using Arquetipo.Api.Models.Request.v1;
using Arquetipo.Api.Models.Request.v2;
using Arquetipo.Api.Models.Response.v1;
using Arquetipo.Api.Models.Response.v2;

namespace Arquetipo.Api.Handlers.Mapper
{
    public static class ClienteMapper
    {
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

            return new ClienteResponseV2
            {
                Id = repoCliente.Id,
                Nombre = repoCliente.Nombre,
                Apellido = repoCliente.Apellido,
                Email = repoCliente.Email,
                Telefono = repoCliente.Telefono,
                EstadoCivil = "N/A",
                FechaRegistro = DateTime.UtcNow
            };
        }

        public static SetCliente ToSetCliente(CrearClienteRequestV1 requestV1)
        {
            if (requestV1 == null) return null;
            return new SetCliente
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

        public static SetCliente ToSetCliente(CrearClienteRequestV2 requestV2)
        {
            if (requestV2 == null) return null;

            return new SetCliente
            {
                Nombre = requestV2.Nombre,
                Apellido = requestV2.Apellido,
                Email = requestV2.Email,
                Telefono = requestV2.Telefono ?? "N/A"
            };
        }

        public static void ApplyUpdate(Models.Response.Cliente entidadCliente, ActualizarClienteRequestV2 requestV2)
        {
            if (entidadCliente == null || requestV2 == null) return;

            if (!string.IsNullOrEmpty(requestV2.Nombre)) entidadCliente.Nombre = requestV2.Nombre;
            if (!string.IsNullOrEmpty(requestV2.Apellido)) entidadCliente.Apellido = requestV2.Apellido;
            if (!string.IsNullOrEmpty(requestV2.Email)) entidadCliente.Email = requestV2.Email;

            if (requestV2.Telefono != null)
            {
                entidadCliente.Telefono = string.IsNullOrEmpty(requestV2.Telefono) ? "N/A" : requestV2.Telefono;
            }
        }

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
            };
        }
    }
}