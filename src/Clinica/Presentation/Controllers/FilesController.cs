using Clinica.Application.DTOs.Requests;
using Clinica.Application.DTOs.Responses;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Infrastructure.ExternalServices;
using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Presentation.Controllers;

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
                UploadedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Size = request.File.Length,
                ContentType = request.File.ContentType,
                MedicalRecordId = request.MedicalRecordId
            };

            _context.PatientDocuments.Add(doc);
            await _context.SaveChangesAsync();

            result = Ok(new FileResponseDto
            {
                Message = "Archivo subido",
                Url = url,
                Size = request.File.Length,
                ContentType = request.File.ContentType,
                MedicalRecordId = request.MedicalRecordId?.ToString(),
                PatientId = request.PatientId
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

        List<FileResponseDto> docs = await query
            .OrderByDescending(d => d.UploadedAt)
            .Select(d => new FileResponseDto
            {
                Url = d.FileUrl,
                Size = d.Size,
                ContentType = d.ContentType,
                Message = "Archivo encontrado",
                PatientId = d.PatientId,
                MedicalRecordId = d.MedicalRecordId.HasValue ? d.MedicalRecordId.Value.ToString() : null
            })
            .ToListAsync();

        return Ok(docs);
    }
}

