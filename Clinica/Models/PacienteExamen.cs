
namespace Clinica.Models
{
    public class PacienteExamen{
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ExamId { get; set; }
        public string ResultText { get; set; } = String.Empty;
        public string ResultFilePath { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }  
    }
}
