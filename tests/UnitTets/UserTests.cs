using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;

namespace UnitTests;

public class UserTests
{
    private ApplicationDbContext _context;

    // This constructor runs before each test to reset the in-memory database
    public UserTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure isolation per test
            .Options;

        _context = new ApplicationDbContext(options);
    }

    // First test: check that a user is successfully added
    [Test]
    public async Task AddUser_ShouldAddUserToDatabase()
    {
        // Arrange: create a Role (required by User)
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

        // : database should have at least one user
        await Assert.That(_context.Users.CountAsync().Result).IsGreaterThan(0);
    }

    // Second test: add and then retrieve the user
    [Test]
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

        // Act: try to retrieve user
        var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == "hugo@example.com");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Email).IsEqualTo("hugo@example.com");
        await Assert.That(result.FirstName).IsEqualTo("Hugo");
    }
}




