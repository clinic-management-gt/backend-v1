using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /recipes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeResponseDTO>> GetRecipeById(int id)
        {
            var dto = await _context.Recipes
                .Include(r => r.Treatment)
                    .ThenInclude(t => t.Appointment)
                .Where(r => r.Id == id)
                .Select(r => new RecipeResponseDTO
                {
                    Id = r.Id,
                    TreatmentId = r.TreatmentId,
                    Prescription = r.Prescription,
                    CreatedAt = r.CreatedAt,
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound($"Recipe with ID {id} not found.");

            return Ok(dto);
        }

        // POST: /recipes
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeCreateDTO input)
        {
            if (input == null || input.TreatmentId <= 0)
                return BadRequest("TreatmentId is required.");

            // Validar que exista el Treatment
            var treatmentExists = await _context.Treatments
                .AnyAsync(t => t.Id == input.TreatmentId);
            if (!treatmentExists)
                return NotFound($"Treatment with ID {input.TreatmentId} not found.");

            var entity = new Recipe
            {
                TreatmentId = input.TreatmentId,
                Prescription = input.Prescription.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Recipes.Add(entity);
            await _context.SaveChangesAsync();

            var dto = await _context.Recipes
                .Include(r => r.Treatment)
                    .ThenInclude(t => t.Appointment)
                .Where(r => r.Id == entity.Id)
                .Select(r => new RecipeResponseDTO
                {
                    Id = r.Id,
                    TreatmentId = r.TreatmentId,
                    Prescription = r.Prescription,
                    CreatedAt = r.CreatedAt,
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetRecipeById), new { id = dto.Id }, dto);
        }

        // PATCH: /recipes/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] RecipeUpdateDTO patch)
        {
            if (patch == null)
                return BadRequest("Body required.");

            var entity = await _context.Recipes
                .Include(r => r.Treatment)
                    .ThenInclude(t => t.Appointment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
                return NotFound($"Recipe with ID {id} not found.");

            // Si quieren mover la receta a otro Treatment
            if (patch.TreatmentId != entity.TreatmentId)
            {
                var treatmentExists = await _context.Treatments
                    .AnyAsync(t => t.Id == patch.TreatmentId);
                if (!treatmentExists)
                    return NotFound($"Treatment with ID {patch.TreatmentId} not found.");
                entity.TreatmentId = patch.TreatmentId;
            }

            if (patch.Prescription is not null)
            {
                if (string.IsNullOrWhiteSpace(patch.Prescription))
                    return BadRequest("Prescription cannot be empty");
                entity.Prescription = patch.Prescription.Trim();
            }
            await _context.SaveChangesAsync();

            var dto = new RecipeResponseDTO
            {
                Id = entity.Id,
                TreatmentId = entity.TreatmentId,
                Prescription = entity.Prescription,
                CreatedAt = entity.CreatedAt,
            };

            return Ok(dto);
        }
    }
}
