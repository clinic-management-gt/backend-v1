using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinica.Models.EntityFramework;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("status")]
    public AppointmentStatus Status { get; set; }

    public virtual ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();

    public virtual User Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
