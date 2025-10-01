using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests;

public class PingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPing_ReturnsPong()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/ping");
        response.EnsureSuccessStatusCode();

        var stringContent = await response.Content.ReadAsStringAsync();

        Assert.Equal("pong", stringContent);
    }
}
