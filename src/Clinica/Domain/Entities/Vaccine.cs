
namespace Clinica.Domain.Entities;

public partial class Vaccine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PatientVaccine> PatientVaccines { get; set; } = new List<PatientVaccine>();
}
