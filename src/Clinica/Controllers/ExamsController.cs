using Clinica.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("exams")]
    public class ExamsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CloudflareR2Service _r2Service;

        public ExamsController(IConfiguration config, CloudflareR2Service r2Service)
        {
            _config = config;
            _r2Service = r2Service;
        }

        // POST /exams/patients
        [HttpPost("patients")]
        [RequestSizeLimit(25_000_000)]
        public async Task<IActionResult> CreatePatientExam(
            [FromForm] int patientId,
            [FromForm] int examId,
            [FromForm] string resultText,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Archivo vacío" });

            var fileUrl = await _r2Service.UploadDocumentToCloudflareR2(file);

            var cs = _config.GetConnectionString("DefaultConnection");
            await using var conn = new NpgsqlConnection(cs);
            await conn.OpenAsync();

            var sql = @"INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path, created_at)
                        VALUES (@p,@e,@t,@f, now()) RETURNING id;";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", patientId);
            cmd.Parameters.AddWithValue("@e", examId);
            cmd.Parameters.AddWithValue("@t", resultText ?? string.Empty);
            cmd.Parameters.AddWithValue("@f", fileUrl);
            var newId = (int)(await cmd.ExecuteScalarAsync()!);

            return Ok(new { id = newId, patientId, examId, resultText, fileUrl });
        }
    }
}
