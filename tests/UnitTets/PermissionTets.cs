using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.EntityFrameworkTests;

public class PermissionTests : IAsyncLifetime
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
    public async Task AddPermission_ShouldAddPermissionToDatabase()
    {
        var role = new Role
        {
            Name = "Nurse",
            Type = 3,
            CanEdit = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var module = new Module
        {
            Name = "Appointments",
            Description = "Handles scheduling",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);
        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        var permission = new Permission
        {
            Role = role,
            Module = module,
            CanView = true,
            CanEdit = false,
            CanDelete = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        Assert.Equal(1, await _context.Permissions.CountAsync());
        Assert.True((await _context.Permissions.FirstAsync()).CanView);
    }

    [Fact]
    public async Task RetrievePermission_ShouldIncludeRoleAndModule()
    {
        var role = new Role
        {
            Name = "Doctor",
            Type = 4,
            CanEdit = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var module = new Module
        {
            Name = "Lab Results",
            Description = "Manages lab reports",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);
        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        var permission = new Permission
        {
            Role = role,
            Module = module,
            CanView = true,
            CanEdit = true,
            CanDelete = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        var result = await _context.Permissions
            .Include(p => p.Role)
            .Include(p => p.Module)
            .FirstOrDefaultAsync();

        Assert.NotNull(result);
        Assert.Equal("Doctor", result!.Role.Name);
        Assert.Equal("Lab Results", result.Module.Name);
    }
}
