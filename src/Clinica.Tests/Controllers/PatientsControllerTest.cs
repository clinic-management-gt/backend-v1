using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinica.Controllers;
using Clinica.Models.EntityFramework;
using Clinica.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clinica.Tests.Controllers;

public class PatientsControllerTests
{
    [Fact]
    public async Task GetFullMedicalRecords_ReturnsCorrectData()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestPatientRecords")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            var patient = new Patient
            {
                Id = 1,
                Name = "Test",
                LastName = "Patient",
                Address = "123 Test St.",
                Gender = "M",
                Birthdate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)),
                CreatedAt = DateTime.UtcNow
            };
            context.Patients.Add(patient);

            var medicine = new Medicine
            {
                Id = 1,
                Name = "Test Medicine",
                Type = "Test Type",
                Provider = "Test Provider"
            };
            context.Medicines.Add(medicine);

            var appointment = new Appointment
            {
                Id = 1,
                PatientId = 1,
                AppointmentDate = DateTime.UtcNow,
                Reason = "Test Appointment"
            };
            context.Appointments.Add(appointment);

            await context.SaveChangesAsync();

            var treatment = new Treatment
            {
                Id = 1,
                AppointmentId = 1,
                MedicineId = 1,
                Dosis = "Test Dosis",
                Duration = "5 days",
                Status = "No Terminado"
            };
            context.Treatments.Add(treatment);

            await context.SaveChangesAsync();

            var recipe = new Recipe
            {
                Id = 1,
                TreatmentId = 1,
                Prescription = "Test Prescription",
                CreatedAt = DateTime.UtcNow
            };
            context.Recipes.Add(recipe);

            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var controller = new PatientsController(context, CloudflareR2ServiceFactory.Create());
            var result = await controller.GetFullMedicalRecords(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var treatments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(treatments);

            var treatment = treatments.First();
            var treatmentType = treatment.GetType();
            Assert.Equal(1, (int)treatmentType.GetProperty("TreatmentId")!.GetValue(treatment)!);
            Assert.Equal("No Terminado", (string)treatmentType.GetProperty("Status")!.GetValue(treatment)!);

            var recipes = (IEnumerable<object>)treatmentType.GetProperty("Recipes")!.GetValue(treatment)!;
            Assert.NotNull(recipes);
            Assert.Single(recipes);
            var recipeObj = recipes.First();
            var recipeType = recipeObj.GetType();
            Assert.Equal("Test Prescription", (string)recipeType.GetProperty("Prescription")!.GetValue(recipeObj)!);
        }
    }
}
