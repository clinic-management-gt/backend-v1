using System.Text.Json.Serialization;
using NpgsqlTypes;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppointmentStatus
{
    [PgName("Confirmado")]
    Confirmado,
    [PgName("Pendiente")]
    Pendiente,
    [PgName("Completado")]
    Completado,
    [PgName("Cancelado")]
    Cancelado,
    [PgName("Espera")]
    Espera,
}
