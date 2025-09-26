using System.Text.Json.Serialization;

namespace Clinica.Models;

/// <summary>
/// DTO para actualizar parcialmente una cita médica.
/// </summary>
public class UpdateAppointmentDto
{
    /// <summary>
    /// Identificador del paciente asociado a la cita.
    /// </summary>
    [JsonPropertyName("patientId")]
    public int? PatientId { get; set; }

    /// <summary>
    /// Identificador del doctor asignado a la cita.
    /// </summary>
    [JsonPropertyName("doctorId")]
    public int? DoctorId { get; set; }

    /// <summary>
    /// Nueva fecha y hora de la cita.
    /// </summary>
    [JsonPropertyName("appointmentDate")]
    public DateTime? AppointmentDate { get; set; }

    /// <summary>
    /// Alias para aceptar "date" como campo desde el frontend.
    /// </summary>
    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    /// <summary>
    /// Valor textual del estado (acepta nombres de enum o alias).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Motivo de la cita.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Alias para aceptar "notes" como campo desde el frontend.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
