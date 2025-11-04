using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;

namespace UnitTests.EntityFrameworkTests;

public class PermissionTests : IAsyncDisposable

{
    private ApplicationDbContext _context;

    public PermissionTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Test]
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

        await Assert.That(await _context.Permissions.CountAsync()).IsEqualTo(1);
        await Assert.That((await _context.Permissions.FirstAsync()).CanView).IsTrue();
    }

    [Test]
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

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Role.Name).IsEqualTo("Doctor");
        await Assert.That(result.Module.Name).IsEqualTo("Lab Results");
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync(); // Dispose DbContext asynchronously
        }

        GC.SuppressFinalize(this);
    }


}
