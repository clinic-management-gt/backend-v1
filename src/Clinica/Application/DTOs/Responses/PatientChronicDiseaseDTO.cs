namespace Clinica.Application.DTOs.Responses;

public class PatientChronicDiseaseDTO
{

    public int Id { get; set; }

    public string DiseaseCode { get; set; } = null!;

    public string? DiseaseDescription { get; set; }

}
