using System;
using System.ComponentModel.DataAnnotations;
using Clinica.Models.EntityFramework;

namespace Clinica.Models
{
    public class AppointmentCreateDto
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        public int Duration { get; set; } = 60; // Default: 60 min
        
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pendiente;
        
        public string? Notes { get; set; } // Permitir nulo
    }
}