using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(ApplicationDbContext context, ILogger<AppointmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CheckForConflicts(int patientId, int doctorId, DateTime appointmentDate, int durationMinutes)
        {
            var appointmentEnd = appointmentDate.AddMinutes(durationMinutes);
            
            // Consulta más eficiente usando una sola expresión LINQ
            return await _context.Appointments
                .Where(a => a.PatientId == patientId || a.DoctorId == doctorId) // Filtrar por paciente o doctor
                .Where(a => (appointmentDate >= a.AppointmentDate && appointmentDate < a.AppointmentDate.AddMinutes(a.Duration ?? 30)) || // Caso 1
                           (appointmentEnd > a.AppointmentDate && appointmentEnd <= a.AppointmentDate.AddMinutes(a.Duration ?? 30)) ||   // Caso 2
                           (appointmentDate <= a.AppointmentDate && appointmentEnd >= a.AppointmentDate.AddMinutes(a.Duration ?? 30)))   // Caso 3
                .AnyAsync();
        }

        public async Task<Result<AppointmentDTO>> CreateAppointmentAsync(AppointmentCreateDto appointmentDto)
        {
            try
            {
                // Validaciones adicionales
                var patient = await _context.Patients.FindAsync(appointmentDto.PatientId);
                if (patient == null)
                    return Result<AppointmentDTO>.Failure($"Paciente con ID {appointmentDto.PatientId} no encontrado");
                    
                var doctor = await _context.Users.FindAsync(appointmentDto.DoctorId);
                if (doctor == null)
                    return Result<AppointmentDTO>.Failure($"Doctor con ID {appointmentDto.DoctorId} no encontrado");
            
                // Crear nueva cita
                var appointment = new Appointment
                {
                    PatientId = appointmentDto.PatientId,
                    DoctorId = appointmentDto.DoctorId,
                    AppointmentDate = appointmentDto.Date,
                    Status = appointmentDto.Status,
                    Reason = appointmentDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    Duration = appointmentDto.Duration
                };
                
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                
                // Cargar relaciones para el resultado
                await _context.Entry(appointment)
                    .Reference(a => a.Patient)
                    .LoadAsync();
                    
                await _context.Entry(appointment)
                    .Reference(a => a.Doctor)
                    .LoadAsync();
            
                // Mapear a DTO
                var resultDto = new AppointmentDTO
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    PatientName = $"{appointment.Patient.Name} {appointment.Patient.LastName}",
                    DoctorId = appointment.DoctorId,
                    DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                    Status = appointment.Status,
                    Date = appointment.AppointmentDate,
                    Notes = appointment.Reason,
                    Duration = appointment.Duration
                };
                
                return Result<AppointmentDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cita");
                return Result<AppointmentDTO>.Failure($"Error al crear la cita: {ex.Message}");
            }
        }
        
        // Implementar el método faltante
        public async Task<Result<AppointmentDTO>> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                    return Result<AppointmentDTO>.Failure($"No se encontró la cita con ID {id}");

                var appointmentDto = new AppointmentDTO
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    PatientName = $"{appointment.Patient.Name} {appointment.Patient.LastName}",
                    DoctorId = appointment.DoctorId,
                    DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                    Status = appointment.Status,
                    Date = appointment.AppointmentDate,
                    Notes = appointment.Reason,
                    Duration = appointment.Duration
                };

                return Result<AppointmentDTO>.Success(appointmentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cita");
                return Result<AppointmentDTO>.Failure($"Error al obtener la cita: {ex.Message}");
            }
        }
    }
}