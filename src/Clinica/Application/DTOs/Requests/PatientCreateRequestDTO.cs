namespace Clinica.Application.DTOs.Requests;
/// <summary>
/// Modelo para la creación de un paciente y su hoja de información.
/// </summary>
public class PatientCreateRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int[] Alergies { get; set; } = Array.Empty<int>();
    public int[] Syndromes { get; set; } = Array.Empty<int>();
    public string Pediatrician { get; set; } = string.Empty;
    public List<ContactCreateDTO> Contacts { get; set; } = new();
    public int InsuranceId { get; set; }
    public IFormFile? InfoSheetFile { get; set; }
    public int BloodTypeId { get; set; }
    public int PatientTypeId { get; set; }
}

