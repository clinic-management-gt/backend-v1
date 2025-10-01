using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.Controllers;

public class PatientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PatientsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllPatients_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/patients");

        Assert.True(response.IsSuccessStatusCode);

        var patients = await response.Content.ReadFromJsonAsync<List<Patient>>();

        Assert.NotNull(patients);
    }

    [Fact]
    public async Task GetPatientById_NotFound_WhenMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/patients/99999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
