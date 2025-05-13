using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class RecipesController : ControllerBase
   {
      private readonly IConfiguration _config;

      public RecipesController(IConfiguration config)
      {
         _config = config;
      }

   }
}