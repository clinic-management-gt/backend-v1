using System.Net.Http.Json;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using TUnit;
using TUnit.Assertions;

namespace IntegrationTests.Controllers;

public class VaccinesControllerTests
{
    [ClassDataSource<WebApplicationFactory<Program>>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactory<Program> Factory { get; init; }

    [Test]
    public async Task PostVaccine_ReturnsCreated()
    {
        var client = Factory.CreateClient();

        var vaccine = new VaccineDTO
        {
            Name = "TestVaccine",
            Brand = "TestBrand"
        };

        var response = await client.PostAsJsonAsync("/vaccines", vaccine);

        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var created = await response.Content.ReadFromJsonAsync<VaccineDTO>();

        await Assert.That(created).IsNotNull();
        await Assert.That(created!.Name).IsEqualTo("TestVaccine");
        await Assert.That(created.Brand).IsEqualTo("TestBrand");
    }

    [Test]
    public async Task GetVaccine_NotFound_WhenMissing()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/vaccines/99999");

        await Assert.That(response.StatusCode)
            .IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateVaccine_ReturnsNoContent()
    {
        var client = Factory.CreateClient();

        // First, create a vaccine
        var vaccine = new VaccineDTO
        {
            Name = "Initial",
            Brand = "BrandX"
        };
        var createResponse = await client.PostAsJsonAsync("/vaccines", vaccine);
        var created = await createResponse.Content.ReadFromJsonAsync<VaccineDTO>();

        // Update it
        created!.Name = "Updated";
        created.Brand = "BrandY";

        var updateResponse = await client.PatchAsJsonAsync($"/vaccines/{created.Id}", created);

        await Assert.That(updateResponse.StatusCode)
            .IsEqualTo(System.Net.HttpStatusCode.NoContent);

        // Fetch it again and verify
        var getResponse = await client.GetFromJsonAsync<VaccineDTO>($"/vaccines/{created.Id}");
        await Assert.That(getResponse!.Name).IsEqualTo("Updated");
        await Assert.That(getResponse.Brand).IsEqualTo("BrandY");
    }
}
