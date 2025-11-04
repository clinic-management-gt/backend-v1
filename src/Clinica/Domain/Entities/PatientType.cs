
namespace Clinica.Domain.Entities;

public partial class PatientType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
