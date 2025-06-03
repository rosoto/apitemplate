using System.Text.Json.Serialization;

namespace Arquetipo.Api.Models.Response.ApiOperaciones
{
    public class OperacionesApiResponse<T>
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("comentario")]
        public string Comentario { get; set; }

        [JsonPropertyName("sessionId")]
        public required string SessionId { get; set; }

        [JsonPropertyName("data")]
        public List<T> Data { get; set; } // Data será una lista del tipo específico (TasaCambioItem o FeriadoLegalItem)
    }

    // --- Modelos para GetTasaDeCambio ---
    public class TasaDeCambioItem
    {
        [JsonPropertyName("tasacambio")]
        public decimal TasaCambio { get; set; }

        [JsonPropertyName("fechacambio")]
        public string FechaCambio { get; set; } // Mantener como string si el formato "20-06-2023" es consistente

        [JsonPropertyName("par_cod_error")]
        public int ParCodError { get; set; }

        [JsonPropertyName("par_msg_error")]
        public string ParMsgError { get; set; }
    }

    // --- Modelos para GetFeriadosLegales ---
    public class FeriadoLegalItem
    {
        [JsonPropertyName("anio")]
        public int Anio { get; set; }

        [JsonPropertyName("mes")]
        public int Mes { get; set; }

        [JsonPropertyName("dia")]
        public int Dia { get; set; }

        [JsonPropertyName("diaSemana")]
        public required string DiaSemana { get; set; } // "7", "1", etc.

        [JsonPropertyName("esFeriado")]
        public required string EsFeriado { get; set; } // "S" o "N"
    }
}
