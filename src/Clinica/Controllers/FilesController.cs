using Clinica.Models.EntityFramework;
using Clinica.Models.EntityFramework.Enums;
using Clinica.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers
{
    /// <summary>
    /// Controlador para gestionar la subida y descarga de archivos de pacientes.
    /// </summary>
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly CloudflareR2Service _r2;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor del controlador de archivos.
        /// </summary>
        /// <param name="r2">Servicio para subir archivos a Cloudflare R2.</param>
        /// <param name="context">Contexto de base de datos.</param>
        public FilesController(CloudflareR2Service r2, ApplicationDbContext context)
        {
            _r2 = r2;
            _context = context;
        }

        /// <summary>
        /// Sube un archivo de paciente al sistema y lo almacena en Cloudflare R2.
        /// </summary>
        /// <param name="request">Datos del archivo y metadatos.</param>
        /// <returns>Información del archivo subido o error.</returns>
        [HttpPost("upload")]
        [RequestSizeLimit(25_000_000)] // 25 MB ejemplo
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
        {
            IActionResult result;

            if (request.File != null && request.File.Length > 0)
            {
                var typeString = request.Type.ToString();
                var url = await _r2.UploadDocumentToCloudflareR2(request.File, request.PatientId, typeString, request.MedicalRecordId);
                var doc = new PatientDocument
                {
                    PatientId = request.PatientId,
                    Type = request.Type,
                    Description = request.Description,
                    FileUrl = url,
                    UploadedBy = null,
                    UploadedAt = DateTime.UtcNow,
                    Size = request.File.Length,
                    ContentType = request.File.ContentType,
                    MedicalRecordId = request.MedicalRecordId
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

        /// <summary>
        /// Obtiene la lista de archivos de un paciente filtrados por tipo y expediente médico.
        /// </summary>
        /// <param name="PatientId">Id del paciente.</param>
        /// <param name="Type">Tipo de documento.</param>
        /// <param name="MedicalRecordId">Id del expediente médico (opcional).</param>
        /// <returns>Lista de archivos encontrados.</returns>
        [HttpGet("download")]
        public async Task<IActionResult> GetPatientDocuments([FromQuery] int PatientId, [FromQuery] FileType Type, [FromQuery] int? MedicalRecordId = null)
        {
            var query = _context.PatientDocuments
                .Where(d => d.PatientId == PatientId && d.Type == Type);

            if (MedicalRecordId.HasValue)
            {
                query = query.Where(d => d.MedicalRecordId == MedicalRecordId);
            }

            var docs = await query
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new FileDTO
                {
                    Url = d.FileUrl,
                    Size = d.Size,
                    ContentType = d.ContentType,
                    Message = "Archivo encontrado",
                    Patient = "Id del paciente: " + d.PatientId,
                    MedicalRecordId = "Id del medical record: " + d.MedicalRecordId
                })
                .ToListAsync();

            return Ok(docs);
        }
    }
}
