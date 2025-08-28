using Clinica.Services;
using Microsoft.AspNetCore.Mvc;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly CloudflareR2Service _r2;
        private readonly ApplicationDbContext _context;

        public FilesController(CloudflareR2Service r2, ApplicationDbContext context)
        {
            _r2 = r2;
            _context = context;
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

                var url = await _r2.UploadDocumentToCloudflareR2(request.File, request.PatientId, request.Type);
                var doc = new PatientDocument
                {
                    PatientId = request.PatientId,
                    Type = request.Type,
                    Description = request.Description,
                    FileUrl = url,
                    UploadedBy = null,
                    UploadedAt = DateTime.UtcNow
                };

                _context.PatientDocuments.Add(doc);
                await _context.SaveChangesAsync();

                result = Ok(new FileDTO
                {
                    Message = "Archivo subido",
                    Url = url,
                    Size = request.File.Length,
                    ContentType = request.File.ContentType,
                });
            }
            else
            {
                result = BadRequest(new { error = "Archivo vacío" });
            }

            return result;

        }

        [HttpGet("download")]
        public async Task<IActionResult> GetPatientDocuments([FromQuery] int PatientId, [FromQuery] string Type)
        {
            var docs = await _context.PatientDocuments
                .Where(d => d.PatientId == PatientId && d.Type == Type)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new FileDTO
                {
                    Url = d.FileUrl,
                    Size = null,
                    ContentType = null,
                    Message = "Archivo encontrado"
                })
                .ToListAsync();

            return Ok(docs);
        }
    }
}
