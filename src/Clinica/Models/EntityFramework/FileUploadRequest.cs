using Microsoft.AspNetCore.Http;

namespace Clinica.Models.EntityFramework
{
    public class FileUploadRequest
    {
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}