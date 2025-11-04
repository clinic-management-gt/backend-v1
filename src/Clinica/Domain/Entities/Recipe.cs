
namespace Clinica.Domain.Entities;
public partial class Recipe
{
    public int Id { get; set; }

    public int TreatmentId { get; set; }

    public string? Prescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Treatment Treatment { get; set; } = null!;
}
