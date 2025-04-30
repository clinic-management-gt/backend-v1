using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class RecetasController : ControllerBase
   {
      private readonly IConfiguration _config;

      public RecetasController(IConfiguration config)
      {
         _config = config;
      }

      // GET: /recetas/paciente/{id}
      [HttpGet("paciente/{id}")]
      public IActionResult GetRecetasByPacienteId(int id)
      {
         Console.WriteLine($"➡️ Endpoint GET /recetas/paciente/{id} alcanzado");

         string? connectionString = _config.GetConnectionString("DefaultConnection");

         try
         {
               using var conn = new NpgsqlConnection(connectionString);
               conn.Open();

               var recetas = new List<object>();

               var sql = @"
                  SELECT r.id, r.prescription, t.medicine_id, t.dosis, t.duration, t.frequency, t.observaciones
                  FROM recipes r
                  INNER JOIN treatments t ON r.treatment_id = t.id
                  INNER JOIN appointments a ON t.appointment_id = a.id
                  WHERE a.patient_id = @id";

               using var cmd = new NpgsqlCommand(sql, conn);
               cmd.Parameters.AddWithValue("id", id);

               using var reader = cmd.ExecuteReader();
               while (reader.Read())
               {
                  recetas.Add(new
                  {
                     Id = reader.GetInt32(0),
                     Prescription = reader.GetString(1),
                     MedicineId = reader.GetInt32(2),
                     Dosis = reader.GetString(3),
                     Duration = reader.GetString(4),
                     Frequency = reader.IsDBNull(5) ? null : reader.GetString(5),
                     Observaciones = reader.IsDBNull(6) ? null : reader.GetString(6)
                  });
               }

               return Ok(recetas);
         }
         catch (Exception ex)
         {
               Console.WriteLine($"❌ Error al consultar recetas: {ex.Message}");
               return StatusCode(500, $"Error al consultar las recetas: {ex.Message}");
         }
      }
   }
}