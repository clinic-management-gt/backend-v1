
namespace Clinica.Application.DTOs.Responses;

public class GrowthCurveDTO
{
    public List<HeightToAgeEntryDTO> HeightToAgeEntries { set; get; }
    public List<WeightToAgeEntryDTO> WeightToAgeEntries { set; get; }
    public List<WeightToHeightEntryDTO> WeightToHeightEntries { set; get; }
    public List<BodyMassIndexEntryDTO> BodyMassIndexEntries { set; get; }

    public GrowthCurveDTO(List<HeightToAgeEntryDTO> heightToAgeEntires, List<WeightToAgeEntryDTO> weightToAgeEntries, List<WeightToHeightEntryDTO> weightToHeightEntries, List<BodyMassIndexEntryDTO> bodyMassIndexEntries)
    {
        HeightToAgeEntries = heightToAgeEntires;
        WeightToAgeEntries = weightToAgeEntries;
        WeightToHeightEntries = weightToHeightEntries;
        BodyMassIndexEntries = bodyMassIndexEntries;
    }

}

public class HeightToAgeEntryDTO
{
    public decimal? Height { set; get; }
    public int AgeInDays { set; get; }
}

public class WeightToAgeEntryDTO
{
    public decimal? Weight { set; get; }
    public int AgeInDays { set; get; }
}

public class WeightToHeightEntryDTO
{
    public decimal? Weight { set; get; }
    public decimal? Height { set; get; }
}

public class BodyMassIndexEntryDTO
{
    public decimal? BodyMassIndex { set; get; }
    public int AgeInDays { set; get; }
}

