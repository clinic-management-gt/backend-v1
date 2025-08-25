using System.Net.Http.Json;
using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc.Testing;
using TUnit;
using TUnit.Assertions;

namespace IntegrationTests.Controllers;

public class PatientsControllerTests
{
    [ClassDataSource<WebApplicationFactory<Program>>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactory<Program> Factory { get; init; }

    [Test]
    public async Task GetAllPatients_ReturnsOk()
    {
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/patients");

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var patients = await response.Content.ReadFromJsonAsync<List<Patient>>();

        await Assert.That(patients).IsNotNull();
    }

    [Test]
    public async Task GetPatientById_NotFound_WhenMissing()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/patients/99999");

        await Assert.That(response.StatusCode)
            .IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }
}
