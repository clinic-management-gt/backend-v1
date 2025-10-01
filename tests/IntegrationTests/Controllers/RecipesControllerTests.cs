using System.Net.Http.Json;
using System.Threading.Tasks;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.Controllers;

public class RecipesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RecipesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostRecipe_ReturnsCreated()
    {
        var client = _factory.CreateClient();

        var recipe = new RecipeDTO
        {
            TreatmentId = 1,
            Prescription = "Test prescription"
        };

        var response = await client.PostAsJsonAsync("/recipes", recipe);

        Assert.True(response.IsSuccessStatusCode);

        var created = await response.Content.ReadFromJsonAsync<RecipeDTO>();

        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
    }

    [Fact]
    public async Task GetRecipe_NotFound_WhenMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/recipes/99999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
