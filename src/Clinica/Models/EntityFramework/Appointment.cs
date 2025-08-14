using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinica.Models.EntityFramework;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    [Column("doctor_id")]
    public int DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("status")]
    public AppointmentStatus Status { get; set; }

    public virtual ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();

    // Relaciones
    public virtual Patient Patient { get; set; } = null!;
    
    // ✅ CAMBIAR DE Doctor a User:
    public virtual User Doctor { get; set; } = null!;

    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();

    // Añadir la propiedad Duration a la clase Appointment
    public int? Duration { get; set; } = 30; // Duración predeterminada en minutos
}
