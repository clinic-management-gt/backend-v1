namespace Clinica.Application.DTOs.Responses;
public class RecipeDTO
{
    public int? Id { get; set; }
    public required int TreatmentId { get; set; }
    public required string Prescription { get; set; }
}


