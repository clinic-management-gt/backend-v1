using Clinica.Services;
using Clinica.Models;
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ExamDTO>> CreatePatientExam([FromForm] PatientExamRequest request)
        {
            if (request.File != null && request.File.Length > 0)
            {
                var fileUrl = await _r2Service.UploadDocumentToCloudflareR2(request.File);

                var cs = _config.GetConnectionString("DefaultConnection");
                await using var conn = new NpgsqlConnection(cs);
                await conn.OpenAsync();

                var sql = @"INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path, created_at)
                            VALUES (@p,@e,@t,@f, now()) RETURNING id;";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p", request.PatientId);
                cmd.Parameters.AddWithValue("@e", request.ExamId);
                cmd.Parameters.AddWithValue("@t", request.ResultText ?? string.Empty);
                cmd.Parameters.AddWithValue("@f", fileUrl);
                
                var result = await cmd.ExecuteScalarAsync();
                if (result == null)
                    return StatusCode(500, new { error = "Error al crear el examen" });

                var newId = (int)result;

                return Ok(new ExamDTO
                {
                    Id = newId,
                    PatientId = request.PatientId,
                    ExamId = request.ExamId,
                    ResultText = request.ResultText ?? string.Empty,
                    FileUrl = fileUrl
                });
            }
            else
            {
                return BadRequest(new { error = "Archivo vacío" });
            }
            

           
        }
    }
}
