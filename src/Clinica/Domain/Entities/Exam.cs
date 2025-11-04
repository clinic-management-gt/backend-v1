namespace Clinica.Domain.Entities;

public partial class Exam
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PatientExam> PatientExams { get; set; } = new List<PatientExam>();
}
