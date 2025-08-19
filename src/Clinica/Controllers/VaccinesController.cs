using Clinica.Models.EntityFramework;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VaccinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VaccinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /vaccines
        [HttpPost]
        public async Task<IActionResult> CreateVaccine(VaccineDTO vaccine)

        {
            if (string.IsNullOrEmpty(vaccine.Name) || string.IsNullOrEmpty(vaccine.Brand))
            {
                return BadRequest("Name and Brand are required fields.");
            }

            var vaccinesSet = _context.Vaccines;
            Vaccine newVaccine = new Vaccine
            {
                Name = vaccine.Name,
                Brand = vaccine.Brand,
                CreatedAt = DateTime.Now
            };

            vaccinesSet.Add(newVaccine);


            await _context.SaveChangesAsync();

            VaccineDTO reponseDTO = new VaccineDTO
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                Brand = vaccine.Brand,

            };

            return CreatedAtAction(nameof(GetVaccineById), new { id = vaccine.Id }, reponseDTO);


        }

        // GET: /vaccines/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineDTO>> GetVaccineById(int id)
        {
            var vaccionesSet = _context.Vaccines;
            Vaccine? vaccine = await vaccionesSet.FindAsync(id);

            if (vaccine is null)
                return NotFound();

            return new VaccineDTO
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                Brand = vaccine.Brand,
            };

        }

        // PATCH: /vaccines/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateVaccine(int id, [FromBody] VaccineDTO vaccine)
        {
            if (id != vaccine.Id)
                return BadRequest();

            var vaccinesSet = _context.Vaccines;

            Vaccine? existingVaccine = await vaccinesSet.FindAsync(id);

            if (existingVaccine is null)
                return NotFound();

            existingVaccine.Name = vaccine.Name;
            existingVaccine.Brand = vaccine.Brand;



            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
