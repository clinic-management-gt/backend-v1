namespace Clinica.Application.DTOs.Requests;
public class UpdateMedicalRecordDto
{
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string? FamilyHistory { get; set; }
    public string? Notes { get; set; }
}

