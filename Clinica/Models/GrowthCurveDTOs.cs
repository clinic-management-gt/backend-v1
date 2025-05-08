
namespace Clinica.Models
{


    public class GrowthCurvesDTO
    {
        public List<HeightToAgeEntryDTO> HeightToAgeEntries { set; get; }
        public List<WeightToAgeEntryDTO> WeightToAgeEntries { set; get; } 
        public List<WeightToHeightEntryDTO> WeightToHeightEntries { set; get; }
        public List<BodyMassIndexEntryDTO> BodyMassIndexEntries { set; get; }

        public GrowthCurvesDTO(List<HeightToAgeEntryDTO> heightToAgeEntires, List<WeightToAgeEntryDTO> weightToAgeEntries, List<WeightToHeightEntryDTO> weightToHeightEntries, List<BodyMassIndexEntryDTO> bodyMassIndexEntries)
        {   
            HeightToAgeEntries = heightToAgeEntires;      
            WeightToAgeEntries = weightToAgeEntries;
            WeightToHeightEntries = weightToHeightEntries;
            BodyMassIndexEntries = bodyMassIndexEntries;
        }
        
    }

    public class HeightToAgeEntryDTO
    {
        public double Height { set; get; }
        public int AgeInDays { set; get; }
    }

    public class WeightToAgeEntryDTO
    {
        public double Weight { set; get; }
        public int AgeInDays { set; get; }
    }

    public class WeightToHeightEntryDTO 
    {
        public double Weight { set; get; }
        public double Height { set; get; }
    }

    public class BodyMassIndexEntryDTO 
    {
        public double BodyMassIndex { set; get; }
        public int AgeInDays { set; get; }
    }
}
