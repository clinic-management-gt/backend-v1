using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PatientType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
