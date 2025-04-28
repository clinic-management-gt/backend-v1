using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class PacientesController : ControllerBase
   {
      private readonly IConfiguration _config;

      public PacientesController(IConfiguration config)
      {
         _config = config;
      }

      [HttpGet]
      public IActionResult GetAll([FromQuery] string? name, [FromQuery] string? lastName)
      {
        Console.WriteLine("➡️ Endpoint /pacientes alcanzado (desde DB)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var pacientes = new List<Paciente>();


          var sql = "SELECT id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at, updated_at FROM patients";
          var filters = new List<string>();

          var cmd = new NpgsqlCommand();
            
          if (!string.IsNullOrEmpty(name))
          {
              filters.Add("name ILIKE @name");
              cmd.Parameters.AddWithValue("name", $"%{name}%");
          }

          if (!string.IsNullOrEmpty(lastName))
          {
              filters.Add("unaccent(last_name) ILIKE unaccent(@lastName)");
              cmd.Parameters.AddWithValue("lastName", $"%{lastName}%");
          }

          if (filters.Any())
          {
              sql += " WHERE " + string.Join(" AND ", filters);
          }

          cmd.CommandText = sql;
          cmd.Connection = conn;
            
          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            pacientes.Add(new Paciente{
              Id = reader.GetInt32(0),
              Name = reader.GetString(1),
              LastName = reader.GetString(2),
              Birthdate = reader.GetDateTime(3),
              Address = reader.GetString(4),
              Gender = reader.GetString(5),
              BloodTypeId = reader.GetInt32(6),
              PatientTypeId = reader.GetInt32(7),
              CreatedAt = reader.GetDateTime(8),
              UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
            });
          }

          return Ok(pacientes);
        }
        catch (Exception ex)
        {
         Console.WriteLine($"❌ Error al consultar pacientes: {ex.Message}");
         return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }
   }
}
