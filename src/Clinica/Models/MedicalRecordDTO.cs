
namespace Clinica.Models;

public class MedicalRecordDTO
{
    public int? Id { get; set; }
    public int PatientId { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public string FamilyHistory { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

