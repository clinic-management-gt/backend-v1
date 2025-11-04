using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using Clinica.Domain.Entities;
using Clinica.Application.DTOs.Requests;
using Clinica.Application.DTOs.Responses;

namespace Clinica.Presentation.Controllers;

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
                Id = newVaccine.Id,
                Name = newVaccine.Name,
                Brand = newVaccine.Brand,

            };

            return CreatedAtAction(nameof(GetVaccineById), new { id = newVaccine.Id }, reponseDTO);


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

