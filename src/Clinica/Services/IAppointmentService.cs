using System;
using System.Threading.Tasks;
using Clinica.Models;

namespace Clinica.Services
{
    public interface IAppointmentService
    {
        Task<bool> CheckForConflicts(int patientId, int doctorId, DateTime appointmentDate, int durationMinutes);
        Task<Result<AppointmentDTO>> CreateAppointmentAsync(AppointmentCreateDto appointmentDto);
        Task<Result<AppointmentDTO>> GetAppointmentByIdAsync(int id);
    }
}
