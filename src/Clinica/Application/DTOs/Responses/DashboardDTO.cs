using Clinica.Domain.Enums;

namespace Clinica.Application.DTOs.Responses;

public class DashBoardDTO
{
    public int Id { get; set; }
    public int PatientId { get; set; }  // ID del paciente para navegación
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public DateTime Date { get; set; }
    public int? PatientTypeId { get; set; }
    public string? PatientTypeName { get; set; }
    public string? PatientTypeColor { get; set; }
}
