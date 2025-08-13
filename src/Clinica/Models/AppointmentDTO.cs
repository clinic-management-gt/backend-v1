namespace Clinica.Models
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; } // Permitir nulo
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; } // Permitir nulo
        public AppointmentStatus Status { get; set; }
        public DateTime Date { get; set; }
        public int? Duration { get; set; }
        public string? Notes { get; set; } // Permitir nulo
    }
}
