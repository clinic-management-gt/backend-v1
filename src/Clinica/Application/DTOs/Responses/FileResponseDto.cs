namespace Clinica.Application.DTOs.Responses;

public class FileResponseDto
{
    public string? Message { get; set; }
    public string? Url { get; set; }
    public long? Size { get; set; }
    public string? ContentType { get; set; }
    public string? MedicalRecordId { get; set; }
    public int? PatientId { get; set; }
}


