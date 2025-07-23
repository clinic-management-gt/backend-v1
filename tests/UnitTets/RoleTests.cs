using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;

namespace UnitTests.EntityFrameworkTests;

public class RoleTests : IAsyncDisposable

{
    private ApplicationDbContext _context;

    public RoleTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }


    [Test]
    public async Task AddRole_ShouldAddRoleToDatabase()
    {
        var role = new Role
        {
            Name = "Manager",
            Type = 1,
            CanEdit = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        await Assert.That(await _context.Roles.CountAsync()).IsEqualTo(1);
        await Assert.That((await _context.Roles.FirstAsync()).Name).IsEqualTo("Manager");
    }

    [Test]
    public async Task RetrieveRole_ShouldReturnCorrectRole()
    {
        var role = new Role
        {
            Name = "Receptionist",
            Type = 2,
            CanEdit = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        var result = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Receptionist");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Type).IsEqualTo(2);
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync(); // Dispose DbContext asynchronously
        }

    }
}
