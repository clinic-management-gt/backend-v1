namespace Clinica.Models;

public class ExamDTO
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int ExamId { get; set; }
    public string ResultText { get; set; }
    public string FileUrl { get; set; }
}


