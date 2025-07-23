using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Alergy
{
    public int Id { get; set; }

    public string AlergyCode { get; set; } = null!;

    public string? AlergyDescription { get; set; }

    public virtual ICollection<PatientAlergy> PatientAlergies { get; set; } = new List<PatientAlergy>();
}
