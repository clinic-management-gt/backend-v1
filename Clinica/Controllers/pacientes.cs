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

               using var cmd = new NpgsqlCommand("SELECT id, nombre FROM pacientes", conn);
               using var reader = cmd.ExecuteReader();

               while (reader.Read())
               {
                  pacientes.Add(new
                  {
                     id = reader.GetInt32(0),
                     nombre = reader.GetString(1)
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



// using Microsoft.AspNetCore.Mvc;

// namespace Clinica.Controllers
// {
//    [ApiController]
//    [Route("[controller]")]
//    public class PacientesController : ControllerBase
//    {
//       [HttpGet]
//       public IActionResult Get()
//       {
//          Console.WriteLine("➡️ Endpoint /pacientes alcanzado");
//          return Ok(new[]
//          {
//                new { id = 1, nombre = "Juan Pérez" },
//                new { id = 2, nombre = "María López" },
//                new { id = 3, nombre = "Carlos Gómez" }
//          });
//       }
//    }
// }
