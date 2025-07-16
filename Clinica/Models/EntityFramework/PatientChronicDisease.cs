using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PatientChronicDisease
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int ChronicDiseaseId { get; set; }

    public virtual ChronicDisease ChronicDisease { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
