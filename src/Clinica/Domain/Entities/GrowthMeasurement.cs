using Clinica.Domain.Enums;

namespace Clinica.Domain.Entities;

public class GrowthMeasurement
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int? MedicalRecordId { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public decimal Value { get; set; }
    public DateTime MeasuredAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
    public MedicalRecord? MedicalRecord { get; set; }
}