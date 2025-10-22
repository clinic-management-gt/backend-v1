namespace Clinica.Models;

public class ConfirmPendingPatientDTO
{
    public required string Address { get; set; }
    public required int BloodTypeId { get; set; }
    public required int PatientTypeId { get; set; }
    public List<int>? Alergies { get; set; }
    public List<int>? Syndromes { get; set; }
    public int? InsuranceId { get; set; }
}
