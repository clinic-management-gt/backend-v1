using Microsoft.AspNetCore.Mvc;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class PacientesController : ControllerBase
   {
      [HttpGet]
      public IActionResult Get()
      {
         Console.WriteLine("➡️ Endpoint /pacientes alcanzado");
         return Ok(new[]
         {
               new { id = 1, nombre = "Juan Pérez" },
               new { id = 2, nombre = "María López" },
               new { id = 3, nombre = "Carlos Gómez" }
         });
      }
   }
}
