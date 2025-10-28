namespace Clinica.Models;

/// <summary>
/// DTO para crear una nueva cita médica
/// </summary>
public class CreateAppointmentDTO
{
    /// <summary>
    /// ID del paciente para la cita
    /// </summary>
    public int PatientId { get; set; }

    /// <summary>
    /// Indica si el paciente viene de la tabla pending_patients (true) o patients (false)
    /// </summary>
    public bool IsPendingPatient { get; set; } = false;

    /// <summary>
    /// Fecha y hora de la cita
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Estado de la cita (pendiente, confirmada, completada, cancelada)
    /// </summary>
    public string Status { get; set; } = "pendiente";

    /// <summary>
    /// Motivo o descripción de la cita (opcional)
    /// </summary>
    public string? Reason { get; set; }
}
