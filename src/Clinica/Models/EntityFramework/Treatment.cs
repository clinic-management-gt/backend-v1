using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Treatment
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int MedicineId { get; set; }

    public string Dosis { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string? Frequency { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public TreatmentStatus Status { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
