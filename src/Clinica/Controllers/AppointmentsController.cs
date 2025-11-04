using System.Collections.Generic;
using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinica.Domain.Enums;
using Clinica.Domain.Entities;
using Clinica.Application.DTOs.Requests;
using Clinica.Application.DTOs.Responses;

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

            return Ok(result);
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

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }

    // PATCH /appointments/{id}
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto dto)
    {
        if (dto is null)
        {
            return BadRequest("Debe proporcionar los datos a actualizar.");
        }

        var hasUpdates = dto.PatientId.HasValue
            || dto.DoctorId.HasValue
            || dto.AppointmentDate.HasValue
            || dto.Date.HasValue
            || dto.Status is not null
            || dto.Reason is not null
            || dto.Notes is not null;

        if (!hasUpdates)
        {
            return BadRequest("Debe proporcionar al menos un campo para actualizar.");
        }

        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound("No se encontró la cita.");

            var updated = false;

            if (dto.PatientId.HasValue && dto.PatientId.Value != appointment.PatientId)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId.Value);
                if (!patientExists)
                    return NotFound($"No se encontró el paciente con ID {dto.PatientId.Value}.");
                appointment.PatientId = dto.PatientId.Value;
                updated = true;
            }

            if (dto.DoctorId.HasValue && dto.DoctorId.Value != appointment.DoctorId)
            {
                var doctorExists = await _context.Users.AnyAsync(u => u.Id == dto.DoctorId.Value);
                if (!doctorExists)
                    return NotFound($"No se encontró el doctor con ID {dto.DoctorId.Value}.");
                appointment.DoctorId = dto.DoctorId.Value;
                updated = true;
            }

            var requestedDate = dto.AppointmentDate ?? dto.Date;
            if (requestedDate.HasValue)
            {
                appointment.AppointmentDate = DateTime.SpecifyKind(requestedDate.Value, DateTimeKind.Unspecified);
                updated = true;
            }

            if (dto.Status is { Length: > 0 })
            {
                if (!TryParseStatus(dto.Status, out var newStatus))
                {
                    var validStatuses = string.Join(", ", Enum.GetNames(typeof(AppointmentStatus)));
                    return BadRequest($"Estado no válido. Valores permitidos: {validStatuses}.");
                }

                appointment.Status = newStatus;
                updated = true;
            }

            if (dto.Reason is not null || dto.Notes is not null)
            {
                appointment.Reason = dto.Reason ?? dto.Notes;
                updated = true;
            }

            if (!updated)
            {
                return BadRequest("No se detectaron cambios en la cita.");
            }

            appointment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cita actualizada correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar la cita: {ex.Message}");
        }
    }

    // DELETE /appointments/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAppointment(int id)
    {
        try
        {
            var appointment = await _context.Appointments
                .Include(a => a.Diagnoses)
                .Include(a => a.Treatments)
                    .ThenInclude(t => t.Recipes)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound("No se encontró la cita.");

            if (appointment.Diagnoses.Any())
            {
                _context.Diagnoses.RemoveRange(appointment.Diagnoses);
            }

            if (appointment.Treatments.Any())
            {
                foreach (var treatment in appointment.Treatments)
                {
                    if (treatment.Recipes.Any())
                    {
                        _context.Recipes.RemoveRange(treatment.Recipes);
                    }
                }

                _context.Treatments.RemoveRange(appointment.Treatments);
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cita eliminada correctamente." });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"No se pudo eliminar la cita por restricciones de integridad: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar la cita: {ex.Message}");
        }
    }

    // POST /appointments
    [HttpPost]
    public async Task<ActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
    {
        try
        {
            int finalPatientId;

            if (dto.IsPendingPatient)
            {
                // El paciente viene de pending_patients, necesitamos confirmarlo
                var pendingPatient = await _context.PendingPatients
                    .Include(p => p.PendingPatientContacts)
                        .ThenInclude(c => c.PendingPatientPhones)
                    .FirstOrDefaultAsync(p => p.Id == dto.PatientId);

                if (pendingPatient == null)
                    return NotFound("No se encontró el paciente pendiente.");

                // Crear paciente confirmado desde el pendiente
                var newPatient = new Patient
                {
                    Name = pendingPatient.Name,
                    LastName = pendingPatient.LastName,
                    Birthdate = pendingPatient.Birthdate,
                    Gender = pendingPatient.Gender,
                    Address = "",
                    BloodTypeId = 1, // Por defecto
                    PatientTypeId = 1, // Por defecto
                    LastVisit = DateOnly.FromDateTime(DateTime.Now),
                    CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
                };

                _context.Patients.Add(newPatient);
                await _context.SaveChangesAsync();

                finalPatientId = newPatient.Id;

            }
            else
            {
                // El paciente viene de la tabla patients (paciente existente)
                var patient = await _context.Patients.FindAsync(dto.PatientId);

                if (patient == null)
                    return NotFound("No se encontró el paciente.");

                finalPatientId = patient.Id;
            }

            // Obtener el primer doctor disponible (por simplicidad)
            var doctor = await _context.Users.FirstOrDefaultAsync();
            if (doctor == null)
                return StatusCode(500, "No hay doctores registrados en el sistema.");

            var appointment = new Appointment
            {
                PatientId = finalPatientId,
                DoctorId = doctor.Id,
                AppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate, DateTimeKind.Unspecified),
                Status = Enum.TryParse<AppointmentStatus>(dto.Status, true, out var status)
                    ? status
                    : AppointmentStatus.Pendiente,
                Reason = dto.Reason,
                CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = appointment.Id,
                message = "Cita creada exitosamente."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear la cita: {ex.Message}");
        }
    }

    private static bool TryParseStatus(string rawValue, out AppointmentStatus status)
    {
        if (Enum.TryParse(rawValue, true, out status))
        {
            return true;
        }

        var normalized = rawValue.Trim();
        if (StatusAliases.TryGetValue(normalized, out status))
        {
            return true;
        }

        return false;
    }

    private static readonly Dictionary<string, AppointmentStatus> StatusAliases = new(System.StringComparer.OrdinalIgnoreCase)
    {
        ["general.pending"] = AppointmentStatus.Pendiente,
        ["general.confirmed"] = AppointmentStatus.Confirmado,
        ["general.completed"] = AppointmentStatus.Completado,
        ["general.canceled"] = AppointmentStatus.Cancelado,
        ["general.cancelled"] = AppointmentStatus.Cancelado,
        ["general.waiting"] = AppointmentStatus.Espera,
        ["pending"] = AppointmentStatus.Pendiente,
        ["confirmed"] = AppointmentStatus.Confirmado,
        ["completed"] = AppointmentStatus.Completado,
        ["canceled"] = AppointmentStatus.Cancelado,
        ["cancelled"] = AppointmentStatus.Cancelado,
        ["waiting"] = AppointmentStatus.Espera
    };

}
