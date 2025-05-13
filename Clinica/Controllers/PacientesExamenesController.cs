using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Services; // Importa el servicio CloudflareR2Service

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacienteExamenesController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CloudflareR2Service _r2Service;

        public PacienteExamenesController(IConfiguration config, CloudflareR2Service r2Service)
        {
            _config = config;
            _r2Service = r2Service;
        }

        // POST: /patient/exams
        [HttpPost("patient/exams")]
        public async Task<IActionResult> CreatePatientExam([FromForm] int patientId, [FromForm] int examId, [FromForm] string resultText, [FromForm] IFormFile file)
        {
            Console.WriteLine($"➡️ Endpoint POST /patient/exams alcanzado");

            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                // Guardar archivo en Cloudflare R2
                string resultFilePath = await _r2Service.UploadDocumentToCloudflareR2(file.FileName);

                if (resultFilePath == null)
                {
                    return StatusCode(500, "Error al subir el archivo a Cloudflare R2.");
                }

                // Guardar los detalles del examen en la base de datos
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var sql = @"
                    INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path, created_at) 
                    VALUES (@PatientId, @ExamId, @ResultText, @ResultFilePath, @CreatedAt)";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);
                cmd.Parameters.AddWithValue("@ExamId", examId);
                cmd.Parameters.AddWithValue("@ResultText", resultText);
                cmd.Parameters.AddWithValue("@ResultFilePath", resultFilePath);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();

                return Ok("Examen creado y archivo subido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al insertar examen: {ex.Message}");
                return StatusCode(500, $"Error al insertar examen: {ex.Message}");
            }
        }
    }
}