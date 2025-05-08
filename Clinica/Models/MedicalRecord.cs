using System;

namespace Clinica.Models
{
    public class MedicalRecord{
        public int Id { get; set; }
        public int PatientId { get; set; } 
        public double Weight { get; set; }
        public double Height { get; set; }
        public string FamilyHistory {get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
