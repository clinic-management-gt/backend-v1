using Clinica.Models.EntityFramework;
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
    public async Task<ActionResult<List<Insurance>>> GetAllInsurances()
    {
        var insurances = await _context.Insurances
        .Include(i => i.Patient)
        .ToListAsync();

        return Ok(insurances);

    }
}