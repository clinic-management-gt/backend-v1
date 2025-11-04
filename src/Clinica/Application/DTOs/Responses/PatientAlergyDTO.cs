namespace Clinica.Application.DTOs.Responses;

public class PatientAlergyDTO
{

    public int Id { get; set; }

    public string AlergyCode { get; set; } = null!;

    public string? AlergyDescription { get; set; }

}
