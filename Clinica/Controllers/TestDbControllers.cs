using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class TestDbController : ControllerBase
   {
      private readonly IConfiguration _config;

      public TestDbController(IConfiguration config)
      {
         _config = config;
      }

      [HttpGet]
      public IActionResult Get()
      {
         Console.WriteLine("➡️ Entró al endpoint /testdb");

         string? connectionString = _config.GetConnectionString("DefaultConnection");

         try
         {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("✅ Conexión a PostgreSQL abierta correctamente");
            return Ok("✅ Conexión exitosa a PostgreSQL");
         }
         catch (Exception ex)
         {
            Console.WriteLine($"❌ Error al conectar: {ex.Message}");
            return StatusCode(500, $"❌ Error de conexión: {ex.Message}");
         }
      }
   }
}
