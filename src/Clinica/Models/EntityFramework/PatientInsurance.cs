using System;

namespace Clinica.Models.EntityFramework;

public partial class PatientInsurance
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int InsuranceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual Insurance Insurance { get; set; } = null!;
}

