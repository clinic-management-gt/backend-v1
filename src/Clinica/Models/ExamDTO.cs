namespace Clinica.Models;

public class ExamDTO
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int ExamId { get; set; }
    public string ResultText { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}


