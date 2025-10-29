using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class BloodTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BloodTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllBloodTypes()
    {
        try
        {
            var bloodTypes = await _context.BloodTypes
                .Select(bt => new
                {
                    bt.Id,
                    bt.Type,
                    bt.Description
                })
                .ToListAsync();

            return Ok(bloodTypes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar los tipos de sangre: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBloodTypeById(int id)
    {
        try
        {
            var bloodType = await _context.BloodTypes
                .Where(bt => bt.Id == id)
                .Select(bt => new
                {
                    bt.Id,
                    bt.Type,
                    bt.Description
                })
                .FirstOrDefaultAsync();

            if (bloodType == null)
                return NotFound($"No se encontró el tipo de sangre con ID {id}.");

            return Ok(bloodType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar el tipo de sangre: {ex.Message}");
        }
    }
}

