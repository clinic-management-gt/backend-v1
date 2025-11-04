using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPatientTypes()
    {
        try
        {
            var patientTypes = await _context.PatientTypes
                .Select(pt => new
                {
                    pt.Id,
                    pt.Name,
                    pt.Description,
                    pt.Color
                })
                .ToListAsync();

            return Ok(patientTypes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar los tipos de paciente: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPatientTypeById(int id)
    {
        try
        {
            var patientType = await _context.PatientTypes
                .Where(pt => pt.Id == id)
                .Select(pt => new
                {
                    pt.Id,
                    pt.Name,
                    pt.Description,
                    pt.Color
                })
                .FirstOrDefaultAsync();

            if (patientType == null)
                return NotFound($"No se encontró el tipo de paciente con ID {id}.");

            return Ok(patientType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar el tipo de paciente: {ex.Message}");
        }
    }
}

