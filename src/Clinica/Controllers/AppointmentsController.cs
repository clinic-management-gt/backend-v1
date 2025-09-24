using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /appointments?status=pendiente
    [HttpGet]
    public async Task<ActionResult<List<DashBoardDTO>>> GetAllAppointments([FromQuery] AppointmentStatus? status)
    {
        try
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            if (status != null)
            {
                query = query.Where(a => a.Status == status);
            }

            var result = await query
                .Select(a => new DashBoardDTO
                {
                    Id = a.Id,
                    PatientId = a.Patient.Id,  // Agregar ID del paciente
                    PatientName = a.Patient.Name + " " + a.Patient.LastName,
                    DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                    Status = a.Status,
                    Date = a.AppointmentDate
                })
                .ToListAsync();

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound("No hay citas registradas.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }

    // GET /appointments/today?status=pendiente
    [HttpGet("today")]
    public async Task<ActionResult<List<DashBoardDTO>>> GetTodaysAppointments([FromQuery] AppointmentStatus? status)
    {
        try
        {
            var today = DateTime.Today;

            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date == today)
                .AsQueryable();

            if (status != null)
            {
                query = query.Where(a => a.Status == status);
            }

            var result = await query
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new DashBoardDTO
                {
                    Id = a.Id,
                    PatientId = a.Patient.Id,  // Agregar ID del paciente
                    PatientName = a.Patient.Name + " " + a.Patient.LastName,
                    DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                    Status = a.Status,
                    Date = a.AppointmentDate
                })
                .ToListAsync();

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound("No hay citas para el día de hoy.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }

    // PATCH /appointments/{id}
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDTO dto)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound("No se encontró la cita.");

            appointment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Estado actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar el estado: {ex.Message}");
        }
    }
}

