using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
      public IActionResult Get()
      {
         Console.WriteLine("➡️ Endpoint /pacientes alcanzado (desde DB)");

         string? connectionString = _config.GetConnectionString("DefaultConnection");

         try
         {
               using var conn = new NpgsqlConnection(connectionString);
               conn.Open();

               var pacientes = new List<object>();

               // Actualizamos la consulta para usar "name" en lugar de "nombre"
               using var cmd = new NpgsqlCommand("SELECT id, name FROM patients", conn);
               using var reader = cmd.ExecuteReader();

               while (reader.Read())
               {
                  pacientes.Add(new
                  {
                     id = reader.GetInt32(0),
                     name = reader.GetString(1) // Cambiamos "nombre" a "name"
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
