using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PatientExam
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int ExamId { get; set; }

    public string? ResultText { get; set; }

    public string? ResultFilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
