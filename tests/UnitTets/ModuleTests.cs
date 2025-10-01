using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.EntityFrameworkTests;

public class ModuleTests : IAsyncLifetime
{
    private ApplicationDbContext _context = default!;

    public Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddModule_ShouldAddModuleToDatabase()
    {
        var module = new Module
        {
            Name = "Patient Records",
            Description = "Handles patient data",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        Assert.Equal(1, await _context.Modules.CountAsync());
        Assert.Equal("Patient Records", (await _context.Modules.FirstAsync()).Name);
    }

    [Fact]
    public async Task RetrieveModule_ShouldReturnCorrectModule()
    {
        var module = new Module
        {
            Name = "Billing",
            Description = "Handles invoicing",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        var result = await _context.Modules.FirstOrDefaultAsync(m => m.Name == "Billing");

        Assert.NotNull(result);
        Assert.Equal("Handles invoicing", result!.Description);
    }
}
