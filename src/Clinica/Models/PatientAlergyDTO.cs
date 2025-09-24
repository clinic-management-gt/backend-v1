namespace Clinica.Models;

public class PatientAlergyDTO
{

    public int Id { get; set; }

    public string AlergyCode { get; set; } = null!;

    public string? AlergyDescription { get; set; }

}
