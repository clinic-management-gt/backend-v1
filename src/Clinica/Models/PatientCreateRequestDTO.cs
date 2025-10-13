using Microsoft.AspNetCore.Http;

namespace Clinica.Models
{
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
        public int[] Alergies { get; set; }
        public int[] Syndromes { get; set; }
        public string Pediatrician { get; set; } = string.Empty;
        public string MotherInfo { get; set; } = string.Empty;
        public string FatherInfo { get; set; } = string.Empty;
        public List<string> ContactPhones { get; set; } = new();
        public string Insurance { get; set; } = string.Empty;
        public string ResidenceAddress { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public IFormFile? InfoSheetFile { get; set; }
        public int BloodTypeId { get; set; }
        public int PatientTypeId { get; set; }
    }
}
