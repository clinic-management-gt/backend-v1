using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.EntityFrameworkTests;

public class RoleTests : IAsyncLifetime
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

        Assert.Equal(1, await _context.Roles.CountAsync());
        Assert.Equal("Manager", (await _context.Roles.FirstAsync()).Name);
    }

    [Fact]
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

        Assert.NotNull(result);
        Assert.Equal(2, result!.Type);
    }
}
