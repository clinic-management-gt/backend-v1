
namespace Clinica.Models
{
    public class RecipeCreateDTO
    {
        public int TreatmentId { get; set; }
        public required string Prescription { get; set; }
    }

    public class RecipeUpdateDTO
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public required string Prescription { get; set; }
    }

    public class RecipeResponseDTO
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public required string Prescription { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

