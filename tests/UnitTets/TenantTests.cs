using System;
using System.Threading.Tasks;
using Clinica.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;

namespace UnitTests;

public class TenantTests : IAsyncDisposable

{
    private ApplicationDbContext _context;

    public TenantTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }


    [Test]
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

        await Assert.That(_context.Tenants.CountAsync().Result).IsGreaterThan(0);
    }

    [Test]
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

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.DbName).IsEqualTo("mydb");
        await Assert.That(result.DbHost).IsEqualTo("127.0.0.1");
    }


    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync(); // Dispose DbContext asynchronously
        }

    }

}
