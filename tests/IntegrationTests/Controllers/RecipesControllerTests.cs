using System.Net.Http.Json;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using TUnit;
using TUnit.Assertions;

namespace IntegrationTests.Controllers;

public class RecipesControllerTests
{
    [ClassDataSource<WebApplicationFactory<Program>>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactory<Program> Factory { get; init; }

    [Test]
    public async Task PostRecipe_ReturnsCreated()
    {
        var client = Factory.CreateClient();

        var recipe = new Recipes
        {
            TreatmentId = 1,
            Prescription = "Test prescription"
        };

        var response = await client.PostAsJsonAsync("/recipes", recipe);

        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var created = await response.Content.ReadFromJsonAsync<Recipes>();

        await Assert.That(created).IsNotNull();
        await Assert.That(created!.Id).IsGreaterThan(0);
    }

    [Test]
    public async Task GetRecipe_NotFound_WhenMissing()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/recipes/99999");

        await Assert.That(response.StatusCode)
            .IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }
}

