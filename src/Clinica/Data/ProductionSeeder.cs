using BCrypt.Net;
using Clinica.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Clinica.Domain.Entities;

namespace Clinica.Data;

/// <summary>
/// Seeder de producción - Solo datos esenciales necesarios para que la aplicación funcione.
/// Este seeder se ejecuta en ambientes de producción.
/// </summary>
public static class ProductionSeeder
{
    /// <summary>
    /// Ejecuta el seeder de producción con datos mínimos necesarios.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Database already seeded. Skipping production seeder...");
            return;
        }

        Console.WriteLine("🚀 Starting PRODUCTION database seeding (essential data only)...");

        await SeedTenants(context);
        await SeedRoles(context);
        await SeedModules(context);
        await SeedPermissions(context);
        await SeedUsers(context);
        await SeedPatientTypes(context);
        await SeedBloodTypes(context);
        await SeedAlergiesCatalog(context);
        await SeedChronicDiseasesCatalog(context);
        await SeedExamsCatalog(context);
        await SeedMedicinesCatalog(context);
        await SeedVaccinesCatalog(context);

        Console.WriteLine("✅ Production database seeding completed successfully!");
        Console.WriteLine("⚠️  No test patients created. Ready for production use.");
    }

    private static async Task SeedTenants(ApplicationDbContext context)
    {
        var tenants = new List<Tenant>
        {
            new Tenant
            {
                DbName = "clinicaPediatrica",
                DbHost = "localhost",
                DbUser = "admin",
                DbPassword = "secure_password",
                CreatedAt = DateTime.Now
            }
        };

        await context.Tenants.AddRangeAsync(tenants);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {tenants.Count} tenant(s)");
    }

    private static async Task SeedRoles(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new Role { Type = 1, Name = "Admin", CanEdit = true, CreatedAt = DateTime.Now },
            new Role { Type = 2, Name = "Doctor", CanEdit = false, CreatedAt = DateTime.Now },
            new Role { Type = 3, Name = "Secretaria", CanEdit = true, CreatedAt = DateTime.Now }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {roles.Count} role(s)");
    }

    private static async Task SeedModules(ApplicationDbContext context)
    {
        var modules = new List<Module>
        {
            new Module { Name = "Pacientes", Description = "Gestión de pacientes", CreatedAt = DateTime.Now },
            new Module { Name = "Citas", Description = "Gestión de citas médicas", CreatedAt = DateTime.Now },
            new Module { Name = "Expedientes", Description = "Gestión de expedientes médicos", CreatedAt = DateTime.Now },
            new Module { Name = "Usuarios", Description = "Gestión de usuarios del sistema", CreatedAt = DateTime.Now }
        };

        await context.Modules.AddRangeAsync(modules);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {modules.Count} module(s)");
    }

    private static async Task SeedPermissions(ApplicationDbContext context)
    {
        var permissions = new List<Permission>();
        for (int moduleId = 1; moduleId <= 4; moduleId++)
        {
            permissions.Add(new Permission
            {
                RoleId = 1, // Admin
                ModuleId = moduleId,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.Now
            });
            permissions.Add(new Permission
            {
                RoleId = 2, // Doctor
                ModuleId = moduleId,
                CanView = true,
                CanEdit = moduleId != 4, // Doctors can't edit users
                CanDelete = false,
                CreatedAt = DateTime.Now
            });
            permissions.Add(new Permission
            {
                RoleId = 3, // Secretaria
                ModuleId = moduleId,
                CanView = true,
                CanEdit = moduleId == 2, // Only can edit appointments
                CanDelete = false,
                CreatedAt = DateTime.Now
            });
        }

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {permissions.Count} permission(s)");
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        var users = new List<User>
        {
            // Usuario administrador principal
            new User
            {
                FirstName = "Administrador",
                LastName = "Sistema",
                Email = "admin@clinica.com",
                RoleId = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin2024!"),
                CreatedAt = DateTime.Now
            },
            // Usuario doctor principal
            new User
            {
                FirstName = "Doctor",
                LastName = "Principal",
                Email = "doctor@clinica.com",
                RoleId = 2,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor2024!"),
                CreatedAt = DateTime.Now
            },
            // Usuario secretaria
            new User
            {
                FirstName = "Secretaria",
                LastName = "Recepción",
                Email = "secretaria@clinica.com",
                RoleId = 3,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Secret2024!"),
                CreatedAt = DateTime.Now
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {users.Count} user(s) for production");
        Console.WriteLine("  📧 admin@clinica.com / Admin2024!");
        Console.WriteLine("  📧 doctor@clinica.com / Doctor2024!");
        Console.WriteLine("  📧 secretaria@clinica.com / Secret2024!");
    }

    private static async Task SeedPatientTypes(ApplicationDbContext context)
    {
        var patientTypes = new List<PatientType>
        {
            new PatientType { Name = "Regular", Description = "Paciente pediátrico general", Color = "#9CA3AF" },
            new PatientType { Name = "Emergencia", Description = "Paciente pediátrico en situación de emergencia", Color = "#EAB308" },
            new PatientType { Name = "Crónico", Description = "Paciente pediátrico con enfermedades crónicas", Color = "#F87171" },
            new PatientType { Name = "Control de Crecimiento", Description = "Paciente en seguimiento desarrollo y crecimiento", Color = "#4ADE80" },
            new PatientType { Name = "Vacunación", Description = "Paciente en programa de vacunación pediátrica", Color = "#60A5FA" },
            new PatientType { Name = "Neonatal", Description = "Paciente recién nacido en control", Color = "#C084FC" },
            new PatientType { Name = "Adolescente", Description = "Paciente adolescente en transición a medicina de adultos", Color = "#818CF8" },
            new PatientType { Name = "Especialidad", Description = "Paciente pediátrico atendido en especialidades", Color = "#F472B6" }
        };

        await context.PatientTypes.AddRangeAsync(patientTypes);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientTypes.Count} patient type(s)");
    }

    private static async Task SeedBloodTypes(ApplicationDbContext context)
    {
        var bloodTypes = new List<BloodType>
        {
            new BloodType { Type = "A+", Description = "Tipo A positivo" },
            new BloodType { Type = "A-", Description = "Tipo A negativo" },
            new BloodType { Type = "B+", Description = "Tipo B positivo" },
            new BloodType { Type = "B-", Description = "Tipo B negativo" },
            new BloodType { Type = "AB+", Description = "Tipo AB positivo" },
            new BloodType { Type = "AB-", Description = "Tipo AB negativo" },
            new BloodType { Type = "O+", Description = "Tipo O positivo" },
            new BloodType { Type = "O-", Description = "Tipo O negativo" }
        };

        await context.BloodTypes.AddRangeAsync(bloodTypes);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {bloodTypes.Count} blood type(s)");
    }

    private static async Task SeedAlergiesCatalog(ApplicationDbContext context)
    {
        var alergies = new List<Alergy>
        {
            new Alergy { AlergyCode = "ALG001", AlergyDescription = "Alergia al polen" },
            new Alergy { AlergyCode = "ALG002", AlergyDescription = "Alergia a los ácaros del polvo" },
            new Alergy { AlergyCode = "ALG003", AlergyDescription = "Alergia a la penicilina" },
            new Alergy { AlergyCode = "ALG004", AlergyDescription = "Alergia al maní" },
            new Alergy { AlergyCode = "ALG005", AlergyDescription = "Alergia a los mariscos" },
            new Alergy { AlergyCode = "ALG006", AlergyDescription = "Alergia a la lactosa" },
            new Alergy { AlergyCode = "ALG007", AlergyDescription = "Alergia a los huevos" },
            new Alergy { AlergyCode = "ALG008", AlergyDescription = "Alergia al gluten" },
            new Alergy { AlergyCode = "ALG009", AlergyDescription = "Alergia a picaduras de insectos" },
            new Alergy { AlergyCode = "ALG010", AlergyDescription = "Alergia a animales (pelo de gato/perro)" },
            new Alergy { AlergyCode = "ALG011", AlergyDescription = "Alergia a frutos secos" },
            new Alergy { AlergyCode = "ALG012", AlergyDescription = "Alergia a la soya" },
            new Alergy { AlergyCode = "ALG013", AlergyDescription = "Alergia al látex" },
            new Alergy { AlergyCode = "ALG014", AlergyDescription = "Alergia a medicamentos antiinflamatorios" },
            new Alergy { AlergyCode = "ALG015", AlergyDescription = "Alergia estacional (rinitis alérgica)" }
        };

        await context.Alergies.AddRangeAsync(alergies);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {alergies.Count} allergy catalog item(s)");
    }

    private static async Task SeedChronicDiseasesCatalog(ApplicationDbContext context)
    {
        var chronicDiseases = new List<ChronicDisease>
        {
            new ChronicDisease { DiseaseCode = "CD001", DiseaseDescription = "Asma bronquial" },
            new ChronicDisease { DiseaseCode = "CD002", DiseaseDescription = "Diabetes tipo 1" },
            new ChronicDisease { DiseaseCode = "CD003", DiseaseDescription = "Epilepsia" },
            new ChronicDisease { DiseaseCode = "CD004", DiseaseDescription = "Hipotiroidismo" },
            new ChronicDisease { DiseaseCode = "CD005", DiseaseDescription = "Dermatitis atópica" },
            new ChronicDisease { DiseaseCode = "CD006", DiseaseDescription = "Enfermedad celíaca" },
            new ChronicDisease { DiseaseCode = "CD007", DiseaseDescription = "Fibrosis quística" },
            new ChronicDisease { DiseaseCode = "CD008", DiseaseDescription = "Trastorno por déficit de atención (TDAH)" },
            new ChronicDisease { DiseaseCode = "CD009", DiseaseDescription = "Anemia falciforme" },
            new ChronicDisease { DiseaseCode = "CD010", DiseaseDescription = "Enfermedad de Crohn" },
            new ChronicDisease { DiseaseCode = "CD011", DiseaseDescription = "Artritis reumatoide juvenil" },
            new ChronicDisease { DiseaseCode = "CD012", DiseaseDescription = "Obesidad infantil" }
        };

        await context.ChronicDiseases.AddRangeAsync(chronicDiseases);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {chronicDiseases.Count} chronic disease catalog item(s)");
    }

    private static async Task SeedExamsCatalog(ApplicationDbContext context)
    {
        var exams = new List<Exam>
        {
            new Exam { Name = "Hemograma completo", Description = "Análisis de sangre completo con recuento celular" },
            new Exam { Name = "Química sanguínea", Description = "Glucosa, urea, creatinina, electrolitos" },
            new Exam { Name = "Examen de orina", Description = "Análisis general de orina" },
            new Exam { Name = "Radiografía de tórax", Description = "Estudio radiológico del tórax" },
            new Exam { Name = "Ultrasonido abdominal", Description = "Ecografía de abdomen completo" },
            new Exam { Name = "Electrocardiograma", Description = "Estudio de la actividad eléctrica del corazón" },
            new Exam { Name = "Prueba de tiroides", Description = "TSH, T3, T4" },
            new Exam { Name = "Cultivo de garganta", Description = "Detección de bacterias en garganta" },
            new Exam { Name = "Coprocultivo", Description = "Análisis de heces para detectar parásitos" },
            new Exam { Name = "Audiometría", Description = "Evaluación de la audición" },
            new Exam { Name = "Agudeza visual", Description = "Examen de la vista" },
            new Exam { Name = "Prueba de alergia cutánea", Description = "Test de alergias en piel" },
            new Exam { Name = "Espirometría", Description = "Evaluación de la función pulmonar" },
            new Exam { Name = "Densitometría ósea", Description = "Medición de la densidad del hueso" },
            new Exam { Name = "Electroencefalograma", Description = "Estudio de la actividad cerebral" }
        };

        await context.Exams.AddRangeAsync(exams);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {exams.Count} exam catalog item(s)");
    }

    private static async Task SeedMedicinesCatalog(ApplicationDbContext context)
    {
        var medicines = new List<Medicine>
        {
            new Medicine { Name = "Paracetamol", Provider = "Farmacias Unidas", Type = "Analgésico/Antipirético", CreatedAt = DateTime.Now },
            new Medicine { Name = "Ibuprofeno", Provider = "Laboratorios XYZ", Type = "Antiinflamatorio", CreatedAt = DateTime.Now },
            new Medicine { Name = "Amoxicilina", Provider = "Antibioticos SA", Type = "Antibiótico", CreatedAt = DateTime.Now },
            new Medicine { Name = "Loratadina", Provider = "Farma Plus", Type = "Antihistamínico", CreatedAt = DateTime.Now },
            new Medicine { Name = "Salbutamol", Provider = "Respiratorios Lab", Type = "Broncodilatador", CreatedAt = DateTime.Now },
            new Medicine { Name = "Cetirizina", Provider = "Alergia Med", Type = "Antihistamínico", CreatedAt = DateTime.Now },
            new Medicine { Name = "Azitromicina", Provider = "Antibioticos SA", Type = "Antibiótico", CreatedAt = DateTime.Now },
            new Medicine { Name = "Omeprazol", Provider = "Gastro Lab", Type = "Protector gástrico", CreatedAt = DateTime.Now },
            new Medicine { Name = "Prednisolona", Provider = "Corticoides Med", Type = "Corticoide", CreatedAt = DateTime.Now },
            new Medicine { Name = "Vitamina D3", Provider = "Suplementos Vita", Type = "Vitamina", CreatedAt = DateTime.Now }
        };

        await context.Medicines.AddRangeAsync(medicines);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {medicines.Count} medicine catalog item(s)");
    }

    private static async Task SeedVaccinesCatalog(ApplicationDbContext context)
    {
        var vaccines = new List<Vaccine>
        {
            new Vaccine { Name = "BCG", Brand = "Laboratorios XYZ", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Hepatitis B", Brand = "VacuLab", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Pentavalente (DPT+Hib+HepB)", Brand = "Inmunizaciones SA", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Rotavirus", Brand = "GastroVac", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Neumococo", Brand = "RespiraVax", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Influenza", Brand = "FluProtect", CreatedAt = DateTime.Now },
            new Vaccine { Name = "SRP (Sarampión, Rubéola, Paperas)", Brand = "TripleViral Lab", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Varicela", Brand = "VariVax", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Hepatitis A", Brand = "HepA Lab", CreatedAt = DateTime.Now },
            new Vaccine { Name = "VPH (Virus Papiloma Humano)", Brand = "CerviVax", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Meningococo", Brand = "MeningoProtect", CreatedAt = DateTime.Now },
            new Vaccine { Name = "Fiebre Amarilla", Brand = "TropicVax", CreatedAt = DateTime.Now }
        };

        await context.Vaccines.AddRangeAsync(vaccines);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {vaccines.Count} vaccine catalog item(s)");
    }
}
