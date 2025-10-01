using System.Net.Http.Json;
using System.Threading.Tasks;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.Controllers;

public class VaccinesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VaccinesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostVaccine_ReturnsCreated()
    {
        var client = _factory.CreateClient();

        var vaccine = new VaccineDTO
        {
            Name = "TestVaccine",
            Brand = "TestBrand"
        };

        var response = await client.PostAsJsonAsync("/vaccines", vaccine);

        Assert.True(response.IsSuccessStatusCode);

        var created = await response.Content.ReadFromJsonAsync<VaccineDTO>();

        Assert.NotNull(created);
        Assert.Equal("TestVaccine", created!.Name);
        Assert.Equal("TestBrand", created.Brand);
    }

    [Fact]
    public async Task GetVaccine_NotFound_WhenMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/vaccines/99999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVaccine_ReturnsNoContent()
    {
        var client = _factory.CreateClient();

        var vaccine = new VaccineDTO
        {
            Name = "Initial",
            Brand = "BrandX"
        };
        var createResponse = await client.PostAsJsonAsync("/vaccines", vaccine);
        var created = await createResponse.Content.ReadFromJsonAsync<VaccineDTO>();

        Assert.NotNull(created);
        var updated = new VaccineDTO
        {
            Id = created!.Id,
            Name = "Updated",
            Brand = "BrandY"
        };

        var updateResponse = await client.PatchAsJsonAsync($"/vaccines/{created.Id}", updated);

        Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await client.GetFromJsonAsync<VaccineDTO>($"/vaccines/{created.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal("Updated", getResponse!.Name);
        Assert.Equal("BrandY", getResponse.Brand);
    }
}
