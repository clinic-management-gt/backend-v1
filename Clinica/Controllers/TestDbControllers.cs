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
         string? connectionString = _config.GetConnectionString("DefaultConnection");

         try
         {
               using var conn = new NpgsqlConnection(connectionString);
               conn.Open();
               return Ok("✅ Conexión exitosa a PostgreSQL");
         }
         catch (Exception ex)
         {
               return StatusCode(500, $"❌ Error de conexión: {ex.Message}");
         }
      }
   }
}
