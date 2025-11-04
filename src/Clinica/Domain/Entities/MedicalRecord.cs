namespace Clinica.Domain.Entities;

public partial class MedicalRecord
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public string? FamilyHistory { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
