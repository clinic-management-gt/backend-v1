using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests;

public class TenantTests : IAsyncLifetime
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
    public async Task AddTenant_ShouldAddTenantToDatabase()
    {
        var tenant = new Tenant
        {
            DbName = "tenant_db",
            DbHost = "localhost",
            DbUser = "tenant_user",
            DbPassword = "secure_password",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        Assert.True(await _context.Tenants.CountAsync() > 0);
    }

    [Fact]
    public async Task RetrieveTenant_ShouldReturnCorrectTenant()
    {
        var tenant = new Tenant
        {
            DbName = "mydb",
            DbHost = "127.0.0.1",
            DbUser = "admin",
            DbPassword = "pass1234",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var result = await _context.Tenants.FirstOrDefaultAsync(t => t.DbName == "mydb");

        Assert.NotNull(result);
        Assert.Equal("mydb", result!.DbName);
        Assert.Equal("127.0.0.1", result.DbHost);
    }
}
