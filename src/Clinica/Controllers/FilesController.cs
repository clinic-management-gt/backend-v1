using Clinica.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly CloudflareR2Service _r2;

        public FilesController(CloudflareR2Service r2)
        {
            _r2 = r2;
        }

        // POST /files/upload
        [HttpPost("upload")]
        [RequestSizeLimit(25_000_000)] // 25 MB ejemplo
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Archivo vacío" });

            var url = await _r2.UploadDocumentToCloudflareR2(file);
            return Ok(new
            {
                message = "Archivo subido",
                url,
                size = file.Length,
                contentType = file.ContentType
            });
        }
    }
}