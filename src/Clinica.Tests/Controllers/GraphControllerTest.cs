using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Clinica.Application.DTOs.Responses;
using Clinica.Domain.Entities;
using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Clinica.Tests.Controllers;

public class GraphControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GraphControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostGraph_SavesMeasurements()
    {
        var payload = new
        {
            patientId = 1,
            weight = 50m,
            height = 50m,
            headCircumference = 20m
        };

        var response = await _client.PostAsJsonAsync("/api/graph", payload);
        response.EnsureSuccessStatusCode();

        using var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(json);
        var root = json!.RootElement;
        Assert.Equal("Mediciones guardadas correctamente.", root.GetProperty("message").GetString());
        Assert.Equal(4, root.GetProperty("registeredMeasurements").GetInt32());
    }

    [Fact]
    public async Task GetGrowthChart_ReturnsWeightForAgeEntries()
    {
        await PostGraphAsync();

        var response = await _client.GetAsync("/api/graph/1/BirthTo2Years/WeightForAge");
        response.EnsureSuccessStatusCode();

        var graphSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var graph = await response.Content.ReadFromJsonAsync<GraphResponseDTO>(graphSerializerOptions);
        Assert.NotNull(graph);
        Assert.Equal(1, graph!.PatientId);
        Assert.NotEmpty(graph.Data);
        Assert.All(graph.Data, dp => Assert.True(dp.Y > 0));
    }

    private async Task PostGraphAsync()
    {
        var requestBody = new
        {
            patientId = 1,
            weight = 50m,
            height = 50m,
            headCircumference = 20m
        };

        var response = await _client.PostAsJsonAsync("/api/graph", requestBody);
        response.EnsureSuccessStatusCode();
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedTestPatient(db);
        });
    }

    private static void SeedTestPatient(ApplicationDbContext db)
    {
        const int bloodTypeId = 1;
        const int patientTypeId = 1;

        if (!db.BloodTypes.Any(bt => bt.Id == bloodTypeId))
        {
            db.BloodTypes.Add(new BloodType
            {
                Id = bloodTypeId,
                Type = "O+",
                Description = "Tipo O positivo"
            });
        }

        if (!db.PatientTypes.Any(pt => pt.Id == patientTypeId))
        {
            db.PatientTypes.Add(new PatientType
            {
                Id = patientTypeId,
                Name = "Control",
                Description = "Pacientes de prueba",
                Color = "#000000"
            });
        }

        if (!db.Patients.Any(p => p.Id == 1))
        {
            db.Patients.Add(new Patient
            {
                Id = 1,
                Name = "Paciente",
                LastName = "De Prueba",
                Birthdate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)),
                LastVisit = DateOnly.FromDateTime(DateTime.UtcNow),
                Address = "No aplica",
                Gender = "M",
                BloodTypeId = bloodTypeId,
                PatientTypeId = patientTypeId,
                CreatedAt = DateTime.UtcNow
            });
        }

        db.SaveChanges();
    }
}
