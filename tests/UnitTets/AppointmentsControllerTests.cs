using System;
using System.Linq;
using System.Threading.Tasks;
using Clinica.Controllers;
using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;

namespace UnitTests.Controllers;

public class AppointmentsControllerTests : IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AppointmentsController _controller;
    private int _appointmentId;
    private int _patientId;
    private int _doctorId;

    public AppointmentsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        SeedDataAsync().GetAwaiter().GetResult();
        _controller = new AppointmentsController(_context);
    }

    [Test]
    public async Task UpdateAppointment_WithAliasStatusAndNotes_ShouldPersistChanges()
    {
        var dto = new UpdateAppointmentDto
        {
            Status = "general.confirmed",
            Notes = "Paciente reagendado",
            Date = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Unspecified)
        };

        var result = await _controller.UpdateAppointment(_appointmentId, dto);

        var okResult = result as OkObjectResult;
        await Assert.That(okResult).IsNotNull();

        var updated = await _context.Appointments.FirstAsync(a => a.Id == _appointmentId);
        await Assert.That(updated.Status).IsEqualTo(AppointmentStatus.Confirmado);
        await Assert.That(updated.Reason).IsEqualTo("Paciente reagendado");
        await Assert.That(updated.AppointmentDate.Date).IsEqualTo(dto.Date!.Value.Date);
    }

    [Test]
    public async Task UpdateAppointment_WithMissingPatient_ShouldReturnNotFound()
    {
        var dto = new UpdateAppointmentDto
        {
            PatientId = 9999
        };

        var result = await _controller.UpdateAppointment(_appointmentId, dto);

        var notFound = result as NotFoundObjectResult;
        await Assert.That(notFound).IsNotNull();
        await Assert.That(notFound!.Value).IsEqualTo("No se encontró el paciente con ID 9999.");
    }

    [Test]
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

        var okResult = result as OkObjectResult;
        await Assert.That(okResult).IsNotNull();

        await Assert.That(await _context.Appointments.AnyAsync(a => a.Id == _appointmentId)).IsFalse();
        await Assert.That(await _context.Diagnoses.AnyAsync(d => d.AppointmentId == _appointmentId)).IsFalse();
        await Assert.That(await _context.Treatments.AnyAsync(t => t.AppointmentId == _appointmentId)).IsFalse();
        await Assert.That(await _context.Recipes.AnyAsync(r => r.TreatmentId == treatment.Id)).IsFalse();
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

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
