
namespace Clinica.Domain.Entities;

public partial class PatientVaccine
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int VaccineId { get; set; }

    public string? Dosis { get; set; }

    public int? AgeAtApplication { get; set; }

    public DateOnly? ApplicationDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
