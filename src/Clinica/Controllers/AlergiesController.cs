using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class AlergiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AlergiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAlergies()
    {
        try
        {
            var alergies = await _context.Alergies
                .Select(a => new
                {
                    a.Id,
                    a.AlergyCode,
                    a.AlergyDescription
                })
                .ToListAsync();

            return Ok(alergies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar las alergias: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAlergyById(int id)
    {
        try
        {
            var alergy = await _context.Alergies
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.AlergyCode,
                    a.AlergyDescription
                })
                .FirstOrDefaultAsync();

            if (alergy == null)
                return NotFound($"No se encontró la alergia con ID {id}.");

            return Ok(alergy);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la alergia: {ex.Message}");
        }
    }
}

