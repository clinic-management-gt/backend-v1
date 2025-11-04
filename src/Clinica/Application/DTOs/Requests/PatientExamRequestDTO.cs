using System.ComponentModel.DataAnnotations;

namespace Clinica.Application.DTOs.Requests;
    public class PatientExamRequest
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ExamId { get; set; }

        public string? ResultText { get; set; }

        [Required]
        public required IFormFile File { get; set; } = default!;
    }

