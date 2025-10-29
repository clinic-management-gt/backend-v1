using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Insurance
{
    public int Id { get; set; }

    public string ProviderName { get; set; } = null!;

    public string PolicyNumber { get; set; } = null!;

    public string? CoverageDetails { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PatientInsurance> PatientInsurances { get; set; } = new List<PatientInsurance>();
}
