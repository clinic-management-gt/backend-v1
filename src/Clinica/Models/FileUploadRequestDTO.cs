using System.ComponentModel.DataAnnotations;

namespace Clinica.Models
{
    public class FileUploadRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
