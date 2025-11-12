using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
   public class PatientsControllerTests
   {
      [Fact]
   public async Task GetFullMedicalRecords_ReturnsCorrectData()
   {
      // Arrange
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestPatientRecords")
            .Options;

      // Crear y poblar la base de datos en memoria
      using (var context = new ApplicationDbContext(options))
      {
         // Crear datos de prueba: paciente
            var patient = new Patient
            {
                  Id = 1,
                  Name = "Test",
                  LastName = "Patient",
                  Address = "123 Test St.",
                  Gender = "M",
                  Birthdate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)), // Fecha de nacimiento
                  CreatedAt = DateTime.UtcNow
            };
            context.Patients.Add(patient);
            
            // Crear una medicina primero
            var medicine = new Medicine
            {
                  Id = 1,
                  Name = "Test Medicine",
                  Type = "Test Type",
                  Provider = "Test Provider"
            };
            context.Medicines.Add(medicine);
            
            // Crear una cita
            var appointment = new Appointment
            { 
                  Id = 1, 
                  PatientId = 1,
                  AppointmentDate = DateTime.UtcNow,
                  Reason = "Test Appointment"
            };
            context.Appointments.Add(appointment);
            
            // Guardar para asegurar que las entidades estén en la BD antes de crear relaciones
            await context.SaveChangesAsync();
            
            // Ahora crear tratamiento
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
            
            // Guardar de nuevo para asegurar que el tratamiento esté en la BD
            await context.SaveChangesAsync();
            
            // Finalmente crear receta
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

      // Act - Usar una nueva instancia de contexto para probar
      using (var context = new ApplicationDbContext(options))
      {
            var controller = new PatientsController(context, BuildCloudflareService());
            var result = await controller.GetFullMedicalRecords(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var treatments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(treatments); // Debe haber 1 tratamiento

            // Acceder a las propiedades por reflexión
            var treatment = treatments.First();
            var treatmentType = treatment.GetType();
            Assert.Equal(1, (int)treatmentType.GetProperty("TreatmentId").GetValue(treatment));
            Assert.Equal("No Terminado", (string)treatmentType.GetProperty("Status").GetValue(treatment));

            var recipes = (IEnumerable<object>)treatmentType.GetProperty("Recipes").GetValue(treatment);
            Assert.NotNull(recipes);
            Assert.Single(recipes); // Debe haber 1 receta
            var recipeObj = recipes.First();
            var recipeType = recipeObj.GetType();
            Assert.Equal("Test Prescription", (string)recipeType.GetProperty("Prescription").GetValue(recipeObj));
      }
      }

      private static CloudflareR2Service BuildCloudflareService()
      {
          var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
          return new CloudflareR2Service(config, new TestHttpClientFactory());
      }
   }
}
