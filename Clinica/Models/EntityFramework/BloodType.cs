using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class BloodType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
