using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PatientAlergy
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int AlergyId { get; set; }

    public virtual Alergy Alergy { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
