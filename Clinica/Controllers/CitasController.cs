using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class CitasController : ControllerBase
   {
      private readonly IConfiguration _config;

      public CitasController(IConfiguration config)
      {
         _config = config;
      }

      // GET: /citas/paciente/{id}
      [HttpGet("paciente/{id}")]
      public IActionResult GetCitasByPacienteId(int id)
      {
         Console.WriteLine($"➡️ Endpoint GET /citas/paciente/{id} alcanzado");

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
      // GET: /citas/pacientes/today
      [HttpGet("pacientes/today")]
      public IActionResult GetCitasForToday(){
         string? connectionString = _config.GetConnectionString("DefaultConnection");
         try 
         {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            var citasToday = new List<object>();
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var sql = @"
               SELECT id, appointment_date, reason, status, created_at, updated_at
               FROM appointments
               WHERE appointment_date >= @today AND appointment_date < @tomorrow";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("today", today);
            cmd.Parameters.AddWithValue("tomorrow", tomorrow);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
               citasToday.Add(new
               {
                  Id = reader.GetInt32(0),
                  AppointmentDate = reader.GetDateTime(1),
                  Reason = reader.GetString(2),
                  Status = reader.GetString(3),
                  CreatedAt = reader.GetDateTime(4),
                  UpdatedAt = reader.IsDBNull(5) ? (DateTime?)null: reader.GetDateTime(5)
               });
            }
            return Ok(citasToday);
         }
         catch(Exception ex)
         {
            Console.WriteLine($"❌ Error al consultar citas: {ex.Message}");
            return StatusCode(500, $"Error al consultar las citas: {ex.Message}");
         }
      }
   }
}