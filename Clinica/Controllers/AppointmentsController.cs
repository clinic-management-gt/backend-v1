using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class AppointmentsController : ControllerBase
   {
      private readonly IConfiguration _config;

      public AppointmentsController(IConfiguration config)
      {
         _config = config;
      }

      // GET: /appointments/patients/{id}
      [HttpGet("patient/{id}")]
      public IActionResult GetAppointmentByPacienteId(int id)
      {
         Console.WriteLine($"➡️ Endpoint GET /appointments/patient/{id} alcanzado");

         string? connectionString = _config.GetConnectionString("DefaultConnection");

         try
         {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var citas = new List<object>();

            var sql = @"
               SELECT id, appointment_date, reason, status, created_at, updated_at
               FROM appointments
               WHERE patient_id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
               citas.Add(new
               {
                     Id = reader.GetInt32(0),
                     AppointmentDate = reader.GetDateTime(1),
                     Reason = reader.GetString(2),
                     Status = reader.GetString(3),
                     CreatedAt = reader.GetDateTime(4),
                     UpdatedAt = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5) // Corregido
               });
            }

            return Ok(citas);
         }
         catch (Exception ex)
         {
            Console.WriteLine($"❌ Error al consultar citas: {ex.Message}");
            return StatusCode(500, $"Error al consultar las citas: {ex.Message}");
         }
      }
   }
}