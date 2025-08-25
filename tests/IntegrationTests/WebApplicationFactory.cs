using Microsoft.AspNetCore.Mvc.Testing;
using TUnit.Core.Interfaces;

namespace IntegrationTests;

public class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public Task InitializeAsync()
    {
        _ = Server;

        return Task.CompletedTask;
    }
}
