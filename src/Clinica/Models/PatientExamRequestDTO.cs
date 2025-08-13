using System.ComponentModel.DataAnnotations;

namespace Clinica.Models
{
    public class PatientExamRequest
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        public string? ResultText { get; set; }
        
        [Required]
        public IFormFile File { get; set; }
    }
}