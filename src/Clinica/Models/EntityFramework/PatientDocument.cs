using System;

namespace Clinica.Models.EntityFramework
{
    public class PatientDocument
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public int? UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }

        // Relaciones
        public Patient Patient { get; set; } = null!;
        public User? UploadedByUser { get; set; }
        public long? Size { get; set; }
        public string? ContentType { get; set; }
    }
}
