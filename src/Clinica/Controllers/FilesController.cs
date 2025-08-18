using Clinica.Models;
using Clinica.Services;
using Microsoft.AspNetCore.Mvc;
using Sprache;

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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
        {
            IActionResult result;

            if (request.File != null && request.File.Length > 0)
            {
                var url = await _r2.UploadDocumentToCloudflareR2(request.File);
                result = Ok(new FileDTO
                {
                    Message = "Archivo subido",
                    Url = url,
                    Size = request.File.Length,
                    ContentType = request.File.ContentType
                });
            }
            else
            {
                result = BadRequest(new { error = "Archivo vacío" });
            }


            return result;

        }
    }
}
