using NpgsqlTypes;
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
