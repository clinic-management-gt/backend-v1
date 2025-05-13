using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Services; // Importa el servicio CloudflareR2Service

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientExamsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CloudflareR2Service _r2Service;

        public PatientExamsController(IConfiguration config, CloudflareR2Service r2Service)
        {
            _config = config;
            _r2Service = r2Service;
        }

        // POST: /patient/exams
        [HttpPost("patient/exams")]
        public async Task<IActionResult> CreatePatientExam([FromForm] int patientId, [FromForm] int examId, [FromForm] string resultText, [FromForm] IFormFile file)
        {
            Console.WriteLine($"➡️ Endpoint POST /patient/exams");

            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                // Guardar archivo en Cloudflare R2
                string resultFilePath = await _r2Service.UploadDocumentToCloudflareR2(file.FileName);

                if (resultFilePath == null)
                {
                    return StatusCode(500, "Error uploading file to Cloudflare R2.");
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

                return Ok("Exam created and file uploaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inserting exam: {ex.Message}");
                return StatusCode(500, $"Error inserting exam: {ex.Message}");
            }
        }
    }
}
