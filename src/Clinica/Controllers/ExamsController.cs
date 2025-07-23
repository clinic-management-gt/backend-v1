using Clinica.Services; // Importa el servicio CloudflareR2Service
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
    //    [ApiController]
    //   [Route("exams")]                
    public class ExamsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CloudflareR2Service _r2Service;

        public ExamsController(IConfiguration config, CloudflareR2Service r2Service)
        {
            _config = config;
            _r2Service = r2Service;
        }

        // POST: /exams/patients
        //      [HttpPost("patients")]
        public async Task<IActionResult> CreatePatientExam([FromForm] int patientId, [FromForm] int examId, [FromForm] string resultText, [FromForm] IFormFile file)
        {
            Console.WriteLine($"➡️ Endpoint POST /exams/patients");

            string? connectionString = _config.GetConnectionString("DefaultConnection");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            try
            {
                // Verificar si el archivo está vacío
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Guardar archivo en Cloudflare R2
                var resultFilePath = await _r2Service.UploadDocumentToCloudflareR2(file);

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

                return Ok(new
                {
                    message = "Exam created and file uploaded successfully.",
                    fileUrl = resultFilePath
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inserting exam: {ex.Message}");
                return StatusCode(500, new
                {
                    error = "Error uploading file to Cloudflare R2",
                    detail = ex.Message,
                });
            }
        }
    }
}
