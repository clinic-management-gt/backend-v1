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

public class RecipesControllerTests
{
    [Fact]
    public async Task CreateAndGetRecipe_ReturnsCorrectRecipe()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestRecipesDb")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            var patient = new Patient { Id = 1, Name = "Test", LastName = "Patient", Address = "Test", Gender = "M", Birthdate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)), CreatedAt = DateTime.UtcNow };
            context.Patients.Add(patient);

            var appointment = new Appointment { Id = 1, PatientId = 1, AppointmentDate = DateTime.UtcNow, Reason = "Test" };
            context.Appointments.Add(appointment);

            var medicine = new Medicine { Id = 1, Name = "TestMed", Type = "Type", Provider = "Provider" };
            context.Medicines.Add(medicine);

            var treatment = new Treatment { Id = 1, AppointmentId = 1, MedicineId = 1, Dosis = "Dosis", Duration = "5 days", Status = "No Terminado" };
            context.Treatments.Add(treatment);

            await context.SaveChangesAsync();

            var recipe = new Recipe { Id = 1, TreatmentId = 1, Prescription = "Paracetamol 500mg", CreatedAt = DateTime.UtcNow };
            context.Recipes.Add(recipe);

            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var controller = new PatientsController(context, CloudflareR2ServiceFactory.Create());
            var result = await controller.GetAllRecipesByPatientID(1);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(okResult.Value);
            var recipesList = recipes.ToList();
            Assert.NotEmpty(recipesList);
            Assert.Equal("Paracetamol 500mg", recipesList[0].Prescription);
        }
    }

    [Fact]
    public async Task GetRecipeForNonExistentPatient_ReturnsEmptyList()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestEmptyRecipes")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            var controller = new PatientsController(context, CloudflareR2ServiceFactory.Create());
            var result = await controller.GetAllRecipesByPatientID(999);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(okResult.Value);
            Assert.Empty(recipes);
        }
    }
}
