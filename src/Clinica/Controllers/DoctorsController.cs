using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinica.Models.EntityFramework;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /doctors
        [HttpGet]
        public async Task<ActionResult> GetDoctors()
        {
            Console.WriteLine("➡️ GET /doctors");

            try
            {
                var doctors = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.RoleId == 2) // Solo usuarios con rol Doctor
                    .Select(u => new
                    {
                        Id = u.Id,
                        Name = $"Dr. {u.FirstName} {u.LastName}",
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role.Name,
                        CreatedAt = u.CreatedAt
                    })
                    .OrderBy(d => d.FirstName)
                    .ToListAsync();

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting doctors: {ex.Message}");
                return StatusCode(500, new { error = $"Error retrieving doctors: {ex.Message}" });
            }
        }

        // GET /doctors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDoctor(int id)
        {
            Console.WriteLine($"➡️ GET /doctors/{id}");

            try
            {
                var doctor = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Id == id && u.RoleId == 2) // Solo si es doctor
                    .Select(u => new
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        FullName = $"Dr. {u.FirstName} {u.LastName}",
                        Email = u.Email,
                        Role = u.Role.Name,
                        CreatedAt = u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (doctor == null)
                    return NotFound(new { error = $"Doctor with ID {id} not found." });

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting doctor: {ex.Message}");
                return StatusCode(500, new { error = $"Error retrieving doctor: {ex.Message}" });
            }
        }

        // GET /doctors/available
        [HttpGet("available")]
        public async Task<ActionResult> GetAvailableDoctors([FromQuery] DateTime? date = null)
        {
            Console.WriteLine($"➡️ GET /doctors/available?date={date}");

            try
            {
                var searchDate = date ?? DateTime.Today;

                // Doctores (usuarios con role_id = 2) que NO tienen citas en la fecha especificada
                var availableDoctors = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.RoleId == 2) // Solo doctores
                    .Where(u => !_context.Appointments
                        .Any(a => a.DoctorId == u.Id && 
                                 a.AppointmentDate.Date == searchDate.Date &&
                                 a.Status != AppointmentStatus.Cancelado))
                    .Select(u => new
                    {
                        Id = u.Id,
                        Name = $"Dr. {u.FirstName} {u.LastName}",
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email
                    })
                    .OrderBy(d => d.FirstName)
                    .ToListAsync();

                return Ok(new 
                {
                    Date = searchDate.ToString("yyyy-MM-dd"),
                    AvailableDoctors = availableDoctors,
                    Count = availableDoctors.Count
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting available doctors: {ex.Message}");
                return StatusCode(500, new { error = $"Error retrieving available doctors: {ex.Message}" });
            }
        }

        // GET /doctors/appointments/{doctorId}
        [HttpGet("appointments/{doctorId}")]
        public async Task<ActionResult> GetDoctorAppointments(int doctorId, [FromQuery] DateTime? date = null)
        {
            Console.WriteLine($"➡️ GET /doctors/appointments/{doctorId}?date={date}");

            try
            {
                // Verificar que el usuario sea doctor
                var doctor = await _context.Users
                    .Where(u => u.Id == doctorId && u.RoleId == 2)
                    .FirstOrDefaultAsync();

                if (doctor == null)
                    return NotFound(new { error = $"Doctor with ID {doctorId} not found." });

                var searchDate = date ?? DateTime.Today;

                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == doctorId && 
                               a.AppointmentDate.Date == searchDate.Date)
                    .Select(a => new
                    {
                        Id = a.Id,
                        PatientName = $"{a.Patient.Name} {a.Patient.LastName}",
                        AppointmentDate = a.AppointmentDate,
                        Reason = a.Reason,
                        Status = a.Status.ToString(),
                        Duration = a.Duration ?? 30
                    })
                    .OrderBy(a => a.AppointmentDate)
                    .ToListAsync();

                return Ok(new
                {
                    DoctorId = doctorId,
                    DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                    Date = searchDate.ToString("yyyy-MM-dd"),
                    Appointments = appointments,
                    Count = appointments.Count
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting doctor appointments: {ex.Message}");
                return StatusCode(500, new { error = $"Error retrieving doctor appointments: {ex.Message}" });
            }
        }
    }
}