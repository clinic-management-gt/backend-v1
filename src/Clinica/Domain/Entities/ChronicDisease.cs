namespace Clinica.Domain.Entities;

public partial class ChronicDisease
{
    public int Id { get; set; }

    public string DiseaseCode { get; set; } = null!;

    public string? DiseaseDescription { get; set; }

    public virtual ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();
}
