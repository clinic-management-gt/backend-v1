using Clinica.Models.EntityFramework;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RecipesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: /recipes
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeDTO recipe)
    {

        Recipe newRecipe = new Recipe
        {
            TreatmentId = recipe.TreatmentId,
            Prescription = recipe.Prescription,
            CreatedAt = DateTime.Now,
        };

        var recipesSet = _context.Recipes;

        recipesSet.Add(newRecipe);
        await _context.SaveChangesAsync();

        RecipeDTO response = new RecipeDTO { Id = newRecipe.Id, TreatmentId = newRecipe.TreatmentId, Prescription = newRecipe.Prescription };
        return CreatedAtAction(nameof(GetRecipeById), new { id = newRecipe.Id }, response);
    }

    // GET: /recipes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipesSet = _context.Recipes;

        Recipe? recipe = await recipesSet.FindAsync(id);

        if (recipe is null)
            return NotFound();

        RecipeDTO response = new RecipeDTO
        {
            Id = recipe.Id,
            TreatmentId = recipe.TreatmentId,
            Prescription = recipe.Prescription,
        };


        return Ok(response);

    }



    // PATCH: /recipes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] RecipeDTO request)
    {
        if (id != request.Id)
            return BadRequest();

        var recipesContext = _context.Recipes;
        Recipe? recipe = await recipesContext.FindAsync(id);

        if (recipe is null)
            return NotFound();

        recipe.TreatmentId = request.TreatmentId;
        recipe.Prescription = request.Prescription;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var recipesSet = _context.Recipes;

        Recipe? recipe = await recipesSet.FindAsync(id);

        if (recipe is null)
            return NotFound();

        recipesSet.Remove(recipe);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}

