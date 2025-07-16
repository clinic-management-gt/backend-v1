
namespace Clinica.Models;
public class DashBoardDTO
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public DateTime Date { get; set; }
}
