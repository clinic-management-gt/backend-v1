using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests;

public class UserTests : IAsyncLifetime
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
    public async Task AddUser_ShouldAddUserToDatabase()
    {
        var role = new Role
        {
            Id = 1,
            Name = "Admin",
            Type = 1,
            CanEdit = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);

        var user = new User
        {
            Email = "hugo@example.com",
            FirstName = "Hugo",
            LastName = "Barillas",
            PasswordHash = "securepassword123",
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        Assert.True(await _context.Users.CountAsync() > 0);
    }

    [Fact]
    public async Task RetrieveUser_ShouldReturnCorrectUser()
    {
        var role = new Role
        {
            Id = 2,
            Name = "User",
            Type = 2,
            CanEdit = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(role);

        var user = new User
        {
            Email = "hugo@example.com",
            FirstName = "Hugo",
            LastName = "Barillas",
            PasswordHash = "securepassword123",
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == "hugo@example.com");

        Assert.NotNull(result);
        Assert.Equal("hugo@example.com", result!.Email);
        Assert.Equal("Hugo", result.FirstName);
    }
}
