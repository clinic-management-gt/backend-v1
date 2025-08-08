using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Diagnosis
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public string? Description { get; set; }

    public DateTime? DiagnosisDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
