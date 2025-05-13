
namespace Clinica.Models
{
    public class Recipes{

        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public string? Prescription { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

