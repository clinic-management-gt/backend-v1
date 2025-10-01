using System;
using System.Threading.Tasks;
using Clinica.Controllers;
using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Controllers;

public class AppointmentsControllerTests : IAsyncLifetime
{
    private ApplicationDbContext _context = default!;
    private AppointmentsController _controller = default!;
    private int _appointmentId;
    private int _patientId;
    private int _doctorId;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        await SeedDataAsync();
        _controller = new AppointmentsController(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task UpdateAppointment_WithAliasStatusAndNotes_ShouldPersistChanges()
    {
        var dto = new UpdateAppointmentDto
        {
            Status = "general.confirmed",
            Notes = "Paciente reagendado",
            Date = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Unspecified)
        };

        var result = await _controller.UpdateAppointment(_appointmentId, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var updated = await _context.Appointments.FirstAsync(a => a.Id == _appointmentId);
        Assert.Equal(AppointmentStatus.Confirmado, updated.Status);
        Assert.Equal("Paciente reagendado", updated.Reason);
        Assert.Equal(dto.Date!.Value.Date, updated.AppointmentDate.Date);
    }

    [Fact]
    public async Task UpdateAppointment_WithMissingPatient_ShouldReturnNotFound()
    {
        var dto = new UpdateAppointmentDto
        {
            PatientId = 9999
        };

        var result = await _controller.UpdateAppointment(_appointmentId, dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No se encontró el paciente con ID 9999.", notFound.Value);
    }

    [Fact]
    public async Task DeleteAppointment_ShouldRemoveDependencies()
    {
        var medicine = new Medicine
        {
            Name = "Ibuprofeno",
            Type = "Tableta",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Medicines.AddAsync(medicine);
        await _context.SaveChangesAsync();

        var diagnosis = new Diagnosis
        {
            AppointmentId = _appointmentId,
            Description = "Dolor de cabeza",
            CreatedAt = DateTime.UtcNow
        };

        var treatment = new Treatment
        {
            AppointmentId = _appointmentId,
            MedicineId = medicine.Id,
            Dosis = "1 cada 8h",
            Duration = "3 días",
            Status = "Activo",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Diagnoses.AddAsync(diagnosis);
        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        var recipe = new Recipe
        {
            TreatmentId = treatment.Id,
            Prescription = "Usar según indicado",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Recipes.AddAsync(recipe);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteAppointment(_appointmentId);

        Assert.IsType<OkObjectResult>(result);

        Assert.False(await _context.Appointments.AnyAsync(a => a.Id == _appointmentId));
        Assert.False(await _context.Diagnoses.AnyAsync(d => d.AppointmentId == _appointmentId));
        Assert.False(await _context.Treatments.AnyAsync(t => t.AppointmentId == _appointmentId));
        Assert.False(await _context.Recipes.AnyAsync(r => r.TreatmentId == treatment.Id));
    }

    private async Task SeedDataAsync()
    {
        var patient = new Patient
        {
            Name = "Juan",
            LastName = "Pérez",
            Birthdate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            Gender = "M",
            Address = "Centro",
            BloodTypeId = 1,
            PatientTypeId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var doctor = new User
        {
            FirstName = "Laura",
            LastName = "García",
            Email = "laura@example.com",
            PasswordHash = "hash",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Patients.AddAsync(patient);
        await _context.Users.AddAsync(doctor);
        await _context.SaveChangesAsync();

        _patientId = patient.Id;
        _doctorId = doctor.Id;

        var appointment = new Appointment
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            AppointmentDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            Status = AppointmentStatus.Pendiente,
            Reason = "Chequeo",
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        _appointmentId = appointment.Id;
    }
}
