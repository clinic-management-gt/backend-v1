using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Clinica.Domain.Entities;
using Clinica.Infrastructure.ExternalServices;
using Clinica.Infrastructure.Persistence;
using Clinica.Presentation.Controllers;
using Clinica.Tests.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Clinica.Tests.Controllers
{
    public class RecipesControllerTests
    {
        [Fact]
        public async Task CreateAndGetRecipe_ReturnsCorrectRecipe()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestRecipesDb")
                .Options;

            // Arrange: poblar la base de datos en memoria
            using (var context = new ApplicationDbContext(options))
            {
                var patient = new Patient { Id = 1, Name = "Test", LastName = "Patient", Address = "Test", Gender = "M", Birthdate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)), CreatedAt = DateTime.UtcNow };
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

            // Act: obtener la receta por paciente
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new PatientsController(context, BuildCloudflareService());
                var result = await controller.GetAllRecipesByPatientID(1);

                // Assert - ajustado para ActionResult<IEnumerable<Recipe>>
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                
                var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(okResult.Value);
                
                // Verificar que hay al menos una receta y su contenido
                var recipesList = new List<Recipe>(recipes);
                Assert.NotEmpty(recipesList);
                Assert.Equal("Paracetamol 500mg", recipesList[0].Prescription);
            }
        }

        [Fact]
        public async Task GetRecipeForNonExistentPatient_ReturnsEmptyList()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestEmptyRecipes")
                .Options;

            // Act: intentar obtener recetas de un paciente que no existe
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new PatientsController(context, BuildCloudflareService());
                var result = await controller.GetAllRecipesByPatientID(999); // ID que no existe

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                
                var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(okResult.Value);
                Assert.Empty(recipes); // Verifica que la lista está vacía
            }
        }

        private static CloudflareR2Service BuildCloudflareService()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            return new CloudflareR2Service(config, new TestHttpClientFactory());
        }
    }
}
