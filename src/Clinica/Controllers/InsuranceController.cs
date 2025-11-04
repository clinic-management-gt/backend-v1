using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class InsuranceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InsuranceController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllInsurances()
    {
        try
        {
            var insurances = await _context.Insurances
                .Include(i => i.PatientInsurances)
                    .ThenInclude(pi => pi.Patient)
                .Select(i => new
                {
                    i.Id,
                    i.ProviderName,
                    i.PolicyNumber,
                    i.CoverageDetails,
                    i.CreatedAt,
                    i.UpdatedAt,
                    Patients = i.PatientInsurances.Select(pi => new
                    {
                        pi.Patient.Id,
                        pi.Patient.Name,
                        pi.Patient.LastName,
                        pi.Patient.Birthdate,
                        AssignedAt = pi.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return Ok(insurances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar los seguros: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetInsuranceById(int id)
    {
        try
        {
            var insurance = await _context.Insurances
                .Include(i => i.PatientInsurances)
                    .ThenInclude(pi => pi.Patient)
                .Where(i => i.Id == id)
                .Select(i => new
                {
                    i.Id,
                    i.ProviderName,
                    i.PolicyNumber,
                    i.CoverageDetails,
                    i.CreatedAt,
                    i.UpdatedAt,
                    Patients = i.PatientInsurances.Select(pi => new
                    {
                        pi.Patient.Id,
                        pi.Patient.Name,
                        pi.Patient.LastName,
                        pi.Patient.Birthdate,
                        AssignedAt = pi.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (insurance == null)
                return NotFound($"No se encontró el seguro con ID {id}.");

            return Ok(insurance);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar el seguro: {ex.Message}");
        }
    }
}
