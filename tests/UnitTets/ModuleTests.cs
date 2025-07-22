using System;
using System.Threading.Tasks;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TUnit;
using TUnit.Assertions;

namespace UnitTests.EntityFrameworkTests;

public class ModuleTests
{
    private ApplicationDbContext _context;

    public ModuleTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Test]
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

        await Assert.That(await _context.Modules.CountAsync()).IsEqualTo(1);
        await Assert.That((await _context.Modules.FirstAsync()).Name).IsEqualTo("Patient Records");
    }

    [Test]
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

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Description).IsEqualTo("Handles invoicing");
    }
}
