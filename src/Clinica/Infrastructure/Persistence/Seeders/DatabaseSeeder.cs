using BCrypt.Net;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder de base de datos para poblar con datos iniciales y de prueba.
/// Genera datos completos para cada paciente con todas las relaciones.
/// </summary>
public static class DatabaseSeeder
{
    private static readonly Random Random = new Random(42); // Seed fijo para resultados reproducibles

    /// <summary>
    /// Método principal que ejecuta todos los seeders en orden.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Database already seeded. Skipping...");
            return;
        }

        Console.WriteLine("Starting database seeding with comprehensive data...");

        await SeedTenants(context);
        await SeedRoles(context);
        await SeedModules(context);
        await SeedPermissions(context);
        await SeedUsers(context);
        await SeedPatientTypes(context);
        await SeedBloodTypes(context);

        // Seed base catalogs first
        await SeedAlergies(context);
        await SeedChronicDiseases(context);
        await SeedExams(context);
        await SeedMedicines(context);
        await SeedVaccines(context);

        // Now seed patients with all their relationships
        await SeedPatientsWithRelationships(context);

        Console.WriteLine("Database seeding completed successfully!");
    }

    private static async Task SeedTenants(ApplicationDbContext context)
    {
        var tenants = new List<Tenant>
        {
            new Tenant
            {
                DbName = "clinicaPediatrica",
                DbHost = "localhost",
                DbUser = "sudo",
                DbPassword = "root",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Tenants.AddRangeAsync(tenants);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {tenants.Count} tenants");
    }

    private static async Task SeedRoles(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new Role { Type = 1, Name = "Admin", CanEdit = true, CreatedAt = DateTime.UtcNow },
            new Role { Type = 2, Name = "Doctor", CanEdit = false, CreatedAt = DateTime.UtcNow },
            new Role { Type = 3, Name = "Secretaria", CanEdit = true, CreatedAt = DateTime.UtcNow }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {roles.Count} roles");
    }

    private static async Task SeedModules(ApplicationDbContext context)
    {
        var modules = new List<Module>
        {
            new Module { Name = "Pacientes", Description = "Gestión de pacientes", CreatedAt = DateTime.UtcNow },
            new Module { Name = "Citas", Description = "Gestión de citas médicas", CreatedAt = DateTime.UtcNow },
            new Module { Name = "Expedientes", Description = "Gestión de expedientes médicos", CreatedAt = DateTime.UtcNow },
            new Module { Name = "Usuarios", Description = "Gestión de usuarios del sistema", CreatedAt = DateTime.UtcNow }
        };

        await context.Modules.AddRangeAsync(modules);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {modules.Count} modules");
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
                CreatedAt = DateTime.UtcNow
            });
            permissions.Add(new Permission
            {
                RoleId = 2, // Doctor
                ModuleId = moduleId,
                CanView = true,
                CanEdit = moduleId != 4, // Doctors can't edit users
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            });
            permissions.Add(new Permission
            {
                RoleId = 3, // Secretaria
                ModuleId = moduleId,
                CanView = true,
                CanEdit = moduleId == 2, // Only can edit appointments
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {permissions.Count} permissions");
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        var users = new List<User>
        {
            // Main users
            new User { FirstName = "Flor", LastName = "Ramírez", Email = "flor.ramirez@example.com", RoleId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctoraPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "María", LastName = "Secretaria", Email = "secretaria@example.com", RoleId = 3, PasswordHash = BCrypt.Net.BCrypt.HashPassword("secretariaPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Ernesto", LastName = "Ascencio", Email = "asc23009@uvg.edu.gt", RoleId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("ernestoPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Esteban", LastName = "Cárcamo", Email = "car23016@uvg.edu.gt", RoleId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("estebanPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Nico", LastName = "Concua", Email = "con23197@uvg.edu.gt", RoleId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("nicoPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Hugo", LastName = "Barillas", Email = "bar23556@uvg.edu.gt", RoleId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("hugoPass"), CreatedAt = DateTime.UtcNow },

            // Additional doctors
            new User { FirstName = "Carlos", LastName = "Mendoza", Email = "carlos.mendoza@clinica.com", RoleId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctorPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Ana", LastName = "Torres", Email = "ana.torres@clinica.com", RoleId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctorPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Luis", LastName = "Fernández", Email = "luis.fernandez@clinica.com", RoleId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctorPass"), CreatedAt = DateTime.UtcNow },
            new User { FirstName = "Patricia", LastName = "Gómez", Email = "patricia.gomez@clinica.com", RoleId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctorPass"), CreatedAt = DateTime.UtcNow }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {users.Count} users");
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
        Console.WriteLine($"✓ Seeded {patientTypes.Count} patient types");
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
        Console.WriteLine($"✓ Seeded {bloodTypes.Count} blood types");
    }

    private static async Task SeedAlergies(ApplicationDbContext context)
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
        Console.WriteLine($"✓ Seeded {alergies.Count} alergies");
    }

    private static async Task SeedChronicDiseases(ApplicationDbContext context)
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
        Console.WriteLine($"✓ Seeded {chronicDiseases.Count} chronic diseases");
    }

    private static async Task SeedExams(ApplicationDbContext context)
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
        Console.WriteLine($"✓ Seeded {exams.Count} exams");
    }

    private static async Task SeedMedicines(ApplicationDbContext context)
    {
        var medicines = new List<Medicine>
        {
            new Medicine { Name = "Paracetamol", Provider = "Farmacias Unidas", Type = "Analgésico/Antipirético", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Ibuprofeno", Provider = "Laboratorios XYZ", Type = "Antiinflamatorio", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Amoxicilina", Provider = "Antibioticos SA", Type = "Antibiótico", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Loratadina", Provider = "Farma Plus", Type = "Antihistamínico", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Salbutamol", Provider = "Respiratorios Lab", Type = "Broncodilatador", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Cetirizina", Provider = "Alergia Med", Type = "Antihistamínico", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Azitromicina", Provider = "Antibioticos SA", Type = "Antibiótico", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Omeprazol", Provider = "Gastro Lab", Type = "Protector gástrico", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Prednisolona", Provider = "Corticoides Med", Type = "Corticoide", CreatedAt = DateTime.UtcNow },
            new Medicine { Name = "Vitamina D3", Provider = "Suplementos Vita", Type = "Vitamina", CreatedAt = DateTime.UtcNow }
        };

        await context.Medicines.AddRangeAsync(medicines);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {medicines.Count} medicines");
    }

    private static async Task SeedVaccines(ApplicationDbContext context)
    {
        var vaccines = new List<Vaccine>
        {
            new Vaccine { Name = "BCG", Brand = "Laboratorios XYZ", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Hepatitis B", Brand = "VacuLab", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Pentavalente (DPT+Hib+HepB)", Brand = "Inmunizaciones SA", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Rotavirus", Brand = "GastroVac", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Neumococo", Brand = "RespiraVax", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Influenza", Brand = "FluProtect", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "SRP (Sarampión, Rubéola, Paperas)", Brand = "TripleViral Lab", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Varicela", Brand = "VariVax", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Hepatitis A", Brand = "HepA Lab", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "VPH (Virus Papiloma Humano)", Brand = "CerviVax", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Meningococo", Brand = "MeningoProtect", CreatedAt = DateTime.UtcNow },
            new Vaccine { Name = "Fiebre Amarilla", Brand = "TropicVax", CreatedAt = DateTime.UtcNow }
        };

        await context.Vaccines.AddRangeAsync(vaccines);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {vaccines.Count} vaccines");
    }

    private static async Task SeedPatientsWithRelationships(ApplicationDbContext context)
    {
        var firstNames = new[] { "Juan", "María", "Pedro", "Ana", "Carlos", "Lucía", "José", "Carmen", "Miguel", "Isabel", "Francisco", "Teresa", "Antonio", "Rosa", "Manuel", "Patricia", "Javier", "Laura", "Diego", "Sofía", "Alejandro", "Elena", "Fernando", "Marta", "Roberto", "Cristina", "Andrés", "Beatriz", "Pablo", "Raquel", "Sergio", "Silvia", "Rafael", "Mónica", "Alberto", "Natalia", "Gonzalo", "Victoria", "Óscar", "Andrea", "Rubén", "Clara", "Iván", "Nuria", "Adrián", "Eva", "Álvaro", "Julia", "Marcos", "Irene" };
        var lastNames = new[] { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Gómez", "Martín", "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Muñoz", "Álvarez", "Romero", "Alonso", "Gutiérrez", "Navarro", "Torres", "Domínguez", "Vázquez", "Ramos", "Gil", "Ramírez", "Serrano", "Blanco", "Suárez", "Molina", "Castro", "Ortiz", "Rubio", "Marín", "Sanz", "Iglesias", "Nuñez", "Medina", "Garrido" };
        var streets = new[] { "Calle Principal", "Avenida Central", "Boulevard del Sol", "Calle de la Luna", "Paseo de las Flores", "Avenida de los Pinos", "Calle Real", "Plaza Mayor", "Camino Verde", "Ronda del Este", "Calle Nueva", "Avenida del Parque", "Callejón del Río", "Pasaje de la Paz", "Vía del Mar" };
        var genders = new[] { "Male", "Female" };

        var patients = new List<Patient>();
        var random = Random;

        patients.Add(new Patient
        {
            Name = "Alisson",
            LastName = "Aquino",
            Birthdate = new DateOnly(2025, 6, 11),
            LastVisit = new DateOnly(2025, 8, 11),
            Address = "3 Calle 22-34 Casa Z3 Cuarta los Robles",
            Gender = genders[1],
            BloodTypeId = random.Next(1, 9),
            PatientTypeId = random.Next(1, 9),
            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365))

        });

        // Generar 100 pacientes
        for (int i = 1; i <= 100; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var gender = genders[random.Next(genders.Length)];
            var birthYear = random.Next(2019, 2024);
            var birthMonth = random.Next(1, 13);
            var birthDay = random.Next(1, 29);
            var street = streets[random.Next(streets.Length)];
            var number = random.Next(1, 999);

            patients.Add(new Patient
            {
                Name = firstName,
                LastName = lastName,
                Birthdate = new DateOnly(birthYear, birthMonth, birthDay),
                LastVisit = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-random.Next(1, 365))),
                Address = $"{street} #{number}",
                Gender = gender,
                BloodTypeId = random.Next(1, 9),
                PatientTypeId = random.Next(1, 9),
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365))
            });
        }

        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patients.Count} patients");

        // Ahora crear todas las relaciones para cada paciente
        var allPatientIds = patients.Select(p => p.Id).ToList();

        await SeedPatientAlergiesForAll(context, allPatientIds);
        await SeedPatientChronicDiseasesForAll(context, allPatientIds);
        await SeedMedicalRecordsForAll(context, allPatientIds);
        await SeedContactsForAll(context, allPatientIds);
        await SeedPatientExamsForAll(context, allPatientIds);
        await SeedInsuranceForAll(context, allPatientIds);
        await SeedPatientVaccinesForAll(context, allPatientIds);
        await SeedAppointmentsForAll(context, allPatientIds);
    }

    private static async Task SeedPatientAlergiesForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var patientAlergies = new List<PatientAlergy>();

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene entre 0-3 alergias
            var alergyCount = Random.Next(0, 4);
            var assignedAlergies = new HashSet<int>();

            for (int i = 0; i < alergyCount; i++)
            {
                var alergyId = Random.Next(1, 16);
                if (assignedAlergies.Add(alergyId))
                {
                    patientAlergies.Add(new PatientAlergy { PatientId = patientId, AlergyId = alergyId });
                }
            }
        }

        await context.PatientAlergies.AddRangeAsync(patientAlergies);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientAlergies.Count} patient alergies");
    }

    private static async Task SeedPatientChronicDiseasesForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var patientChronicDiseases = new List<PatientChronicDisease>();

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene entre 0-2 enfermedades crónicas
            var diseaseCount = Random.Next(0, 3);
            var assignedDiseases = new HashSet<int>();

            for (int i = 0; i < diseaseCount; i++)
            {
                var diseaseId = Random.Next(1, 13);
                if (assignedDiseases.Add(diseaseId))
                {
                    patientChronicDiseases.Add(new PatientChronicDisease { PatientId = patientId, ChronicDiseaseId = diseaseId });
                }
            }
        }

        await context.PatientChronicDiseases.AddRangeAsync(patientChronicDiseases);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientChronicDiseases.Count} patient chronic diseases");
    }

    private static async Task SeedMedicalRecordsForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var medicalRecords = new List<MedicalRecord>();
        var familyHistories = new[] { "Sin antecedentes familiares relevantes", "Diabetes en familia", "Hipertensión familiar", "Alergias hereditarias", "Asma en familia", "Ninguno reportado" };
        var notes = new[] { "Paciente en buen estado general", "Desarrollo normal para su edad", "Requiere seguimiento", "Paciente colaborador", "Control periódico recomendado" };

        foreach (var patientId in patientIds)
        {
            medicalRecords.Add(new MedicalRecord
            {
                PatientId = patientId,
                Weight = (decimal)(Random.Next(30, 800) / 10.0), // 3.0 - 80.0 kg
                Height = (decimal)(Random.Next(500, 1800) / 1000.0), // 0.5 - 1.8 m
                FamilyHistory = familyHistories[Random.Next(familyHistories.Length)],
                Notes = notes[Random.Next(notes.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 180)),
                UpdatedAt = DateTime.UtcNow.AddDays(-Random.Next(0, 30))
            });
        }

        await context.MedicalRecords.AddRangeAsync(medicalRecords);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {medicalRecords.Count} medical records");
    }

    private static async Task SeedContactsForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var contacts = new List<Contact>();
        var emails = new List<Email>();
        var contactEmails = new List<ContactEmail>();
        var phones = new List<Phone>();

        var firstNames = new[] { "Roberto", "María", "Carlos", "Ana", "Luis", "Carmen", "José", "Isabel", "Pedro", "Laura" };
        var lastNames = new[] { "Gómez", "Martínez", "López", "García", "Pérez", "Rodríguez", "Fernández", "González" };
        var contactTypes = new[] { "Padre", "Madre", "Tutor", "Abuelo", "Abuela", "Tío", "Tía" };

        int emailId = 1;
        int contactId = 1;

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene 1-3 contactos
            var contactCount = Random.Next(1, 4);

            for (int i = 0; i < contactCount; i++)
            {
                var contact = new Contact
                {
                    Id = contactId,
                    PatientId = patientId,
                    Type = contactTypes[Random.Next(contactTypes.Length)],
                    Name = firstNames[Random.Next(firstNames.Length)],
                    LastName = lastNames[Random.Next(lastNames.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 365))
                };
                contacts.Add(contact);

                // Cada contacto tiene 1-2 emails
                var emailCount = Random.Next(1, 3);
                for (int j = 0; j < emailCount; j++)
                {
                    var email = new Email
                    {
                        Id = emailId,
                        Value = $"{contact.Name.ToLower()}.{contact.LastName.ToLower()}{Random.Next(1, 999)}@email.com"
                    };
                    emails.Add(email);

                    contactEmails.Add(new ContactEmail
                    {
                        ContactId = contactId,
                        EmailId = emailId
                    });

                    emailId++;
                }

                // Cada contacto tiene 1-2 teléfonos
                var phoneCount = Random.Next(1, 3);
                for (int j = 0; j < phoneCount; j++)
                {
                    phones.Add(new Phone
                    {
                        ContactId = contactId,
                        Phone1 = $"{Random.Next(2000, 9999)}-{Random.Next(1000, 9999)}",
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 365))
                    });
                }

                contactId++;
            }
        }

        await context.Contacts.AddRangeAsync(contacts);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {contacts.Count} contacts");

        await context.Emails.AddRangeAsync(emails);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {emails.Count} emails");

        await context.ContactEmails.AddRangeAsync(contactEmails);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {contactEmails.Count} contact emails");

        await context.Phones.AddRangeAsync(phones);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {phones.Count} phones");
    }

    private static async Task SeedPatientExamsForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var patientExams = new List<PatientExam>();
        var resultTexts = new[] { "Resultados normales", "Valores dentro del rango esperado", "Se requiere seguimiento", "Resultados satisfactorios", "Análisis completo sin anomalías" };

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene 2-5 exámenes
            var examCount = Random.Next(2, 6);
            var assignedExams = new HashSet<int>();

            for (int i = 0; i < examCount; i++)
            {
                var examId = Random.Next(1, 16);
                if (assignedExams.Add(examId))
                {
                    patientExams.Add(new PatientExam
                    {
                        PatientId = patientId,
                        ExamId = examId,
                        ResultText = resultTexts[Random.Next(resultTexts.Length)],
                        ResultFilePath = $"/exams/patient_{patientId}_exam_{examId}.pdf",
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 180))
                    });
                }
            }
        }

        await context.PatientExams.AddRangeAsync(patientExams);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientExams.Count} patient exams");
    }

    private static async Task SeedInsuranceForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var insurances = new List<Insurance>();
        var patientInsurances = new List<PatientInsurance>();
        var providers = new[]
        {
            "Seguros Universales",
            "MediCare Plus",
            "Salud Total",
            "Protección Familiar",
            "Vida Sana Seguros",
            "Cobertura Integral",
            "Aseguradora Nacional",
            "Seguro Médico Total"
        };
        var coverages = new[]
        {
            "Cobertura completa",
            "Cobertura básica",
            "Plan premium",
            "Plan familiar",
            "Cobertura de emergencias",
            "Plan individual",
            "Cobertura pediátrica especializada"
        };

        // Paso 1: Crear un catálogo de seguros disponibles (30 seguros diferentes)
        Console.WriteLine("  Creating insurance catalog...");
        for (int i = 1; i <= 30; i++)
        {
            insurances.Add(new Insurance
            {
                Id = i,
                ProviderName = providers[Random.Next(providers.Length)],
                PolicyNumber = $"POL-{Random.Next(100000, 999999):D6}",
                CoverageDetails = coverages[Random.Next(coverages.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(30, 730)) // 30 días a 2 años
            });
        }

        await context.Insurances.AddRangeAsync(insurances);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {insurances.Count} insurance records");

        // Paso 2: Asignar seguros a pacientes (puede haber seguros compartidos)
        Console.WriteLine("  Assigning insurances to patients...");
        foreach (var patientId in patientIds)
        {
            // 85% de pacientes tienen al menos un seguro
            if (Random.Next(100) < 85)
            {
                // Mayoría tiene 1 seguro, algunos tienen 2
                var insuranceCount = Random.Next(100) < 70 ? 1 : 2;
                var assignedInsurances = new HashSet<int>();

                for (int i = 0; i < insuranceCount; i++)
                {
                    // Seleccionar un seguro aleatorio del catálogo
                    var insuranceId = Random.Next(1, 31); // IDs del 1 al 30

                    // Evitar asignar el mismo seguro dos veces al mismo paciente
                    if (assignedInsurances.Add(insuranceId))
                    {
                        patientInsurances.Add(new PatientInsurance
                        {
                            PatientId = patientId,
                            InsuranceId = insuranceId,
                            CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(7, 365))
                        });
                    }
                }
            }
        }

        await context.PatientInsurances.AddRangeAsync(patientInsurances);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientInsurances.Count} patient-insurance relationships");
        Console.WriteLine($"  → Average: {(double)patientInsurances.Count / patientIds.Count:F2} insurances per patient");

        // Mostrar estadísticas de seguros compartidos
        var sharedInsurances = patientInsurances
            .GroupBy(pi => pi.InsuranceId)
            .Where(g => g.Count() > 1)
            .Count();
        Console.WriteLine($"  → {sharedInsurances} insurances are shared by multiple patients");
    }

    private static async Task SeedPatientVaccinesForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var patientVaccines = new List<PatientVaccine>();
        var dosisOptions = new[] { "Primera dosis", "Segunda dosis", "Tercera dosis", "Refuerzo", "Dosis única" };

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene 3-8 vacunas
            var vaccineCount = Random.Next(3, 9);
            var assignedVaccines = new HashSet<int>();

            for (int i = 0; i < vaccineCount; i++)
            {
                var vaccineId = Random.Next(1, 13);
                if (assignedVaccines.Add(vaccineId))
                {
                    var ageAtApplication = Random.Next(0, 180); // 0-15 años en meses
                    var applicationDate = new DateOnly(Random.Next(2010, 2025), Random.Next(1, 13), Random.Next(1, 29));

                    patientVaccines.Add(new PatientVaccine
                    {
                        PatientId = patientId,
                        VaccineId = vaccineId,
                        Dosis = dosisOptions[Random.Next(dosisOptions.Length)],
                        AgeAtApplication = ageAtApplication,
                        ApplicationDate = applicationDate,
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 365))
                    });
                }
            }
        }

        await context.PatientVaccines.AddRangeAsync(patientVaccines);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {patientVaccines.Count} patient vaccines");
    }

    private static async Task SeedAppointmentsForAll(ApplicationDbContext context, List<int> patientIds)
    {
        var appointments = new List<Appointment>();
        var diagnoses = new List<Diagnosis>();
        var treatments = new List<Treatment>();
        var recipes = new List<Recipe>();

        var reasons = new[] { "Chequeo general", "Control de vacunas", "Consulta por fiebre", "Dolor abdominal", "Tos persistente", "Control de crecimiento", "Seguimiento de tratamiento", "Revisión de exámenes", "Consulta de urgencia", "Control rutinario", "Dolor de garganta", "Problemas dermatológicos", "Consulta nutricional", "Evaluación del desarrollo", "Revisión post-operatoria" };
        var statuses = new[] { AppointmentStatus.Confirmado, AppointmentStatus.Completado, AppointmentStatus.Pendiente, AppointmentStatus.Espera, AppointmentStatus.Cancelado };
        var diagnosisDescriptions = new[] { "Infección viral leve", "Estado de salud normal", "Requiere tratamiento antibiótico", "Alergia estacional", "Control satisfactorio", "Diagnóstico de faringitis", "Gastroenteritis aguda", "Bronquitis leve", "Dermatitis atópica", "Desarrollo normal para la edad" };
        var frequencies = new[] { "Cada 8 horas", "Cada 12 horas", "Cada 24 horas", "Cada 6 horas", "Dos veces al día", "Tres veces al día" };
        var durations = new[] { "5 días", "7 días", "10 días", "14 días", "3 días", "21 días" };
        var observations = new[] { "Tomar con alimentos", "Tomar después de las comidas", "No tomar con lácteos", "Tomar con abundante agua", "Evitar exposición al sol", "Completar el tratamiento" };

        var today = DateTime.Today;
        int appointmentId = 1;
        int diagnosisId = 1;
        int treatmentId = 1;
        int recipeId = 1;

        foreach (var patientId in patientIds)
        {
            // Cada paciente tiene 2-6 citas históricas
            var historicalAppointmentCount = Random.Next(2, 7);

            for (int i = 0; i < historicalAppointmentCount; i++)
            {
                var daysAgo = Random.Next(30, 365);
                var hour = Random.Next(8, 18);
                var minute = Random.Next(0, 60);
                var doctorId = Random.Next(1, 11); // 10 doctores disponibles
                var status = statuses[Random.Next(statuses.Length)];

                var appointment = new Appointment
                {
                    Id = appointmentId,
                    PatientId = patientId,
                    DoctorId = doctorId,
                    AppointmentDate = today.AddDays(-daysAgo).AddHours(hour).AddMinutes(minute),
                    Reason = reasons[Random.Next(reasons.Length)],
                    Status = status,
                    CreatedAt = DateTime.UtcNow.AddDays(-daysAgo - 5)
                };
                appointments.Add(appointment);

                // Si la cita fue completada, agregar diagnóstico
                if (status == AppointmentStatus.Completado)
                {
                    diagnoses.Add(new Diagnosis
                    {
                        Id = diagnosisId,
                        AppointmentId = appointmentId,
                        Description = diagnosisDescriptions[Random.Next(diagnosisDescriptions.Length)],
                        DiagnosisDate = appointment.AppointmentDate.AddMinutes(30),
                        CreatedAt = appointment.AppointmentDate
                    });

                    // 70% de probabilidad de tener tratamiento
                    if (Random.Next(100) < 70)
                    {
                        var medicineCount = Random.Next(1, 3);
                        for (int m = 0; m < medicineCount; m++)
                        {
                            var treatment = new Treatment
                            {
                                Id = treatmentId,
                                AppointmentId = appointmentId,
                                MedicineId = Random.Next(1, 11),
                                Dosis = $"{Random.Next(100, 500)}mg",
                                Duration = durations[Random.Next(durations.Length)],
                                Frequency = frequencies[Random.Next(frequencies.Length)],
                                Observaciones = observations[Random.Next(observations.Length)],
                                Status = Random.Next(100) < 60 ? "Terminado" : "No Terminado",
                                CreatedAt = appointment.AppointmentDate
                            };
                            treatments.Add(treatment);

                            // Cada tratamiento tiene una receta
                            recipes.Add(new Recipe
                            {
                                Id = recipeId,
                                TreatmentId = treatmentId,
                                Prescription = $"Administrar {treatment.Dosis} {treatment.Frequency} durante {treatment.Duration}. {treatment.Observaciones}",
                                CreatedAt = appointment.AppointmentDate
                            });

                            recipeId++;
                            treatmentId++;
                        }
                    }

                    diagnosisId++;
                }

                appointmentId++;
            }

            // 50% de pacientes tienen citas futuras/hoy
            if (Random.Next(100) < 50)
            {
                var futureAppointmentCount = Random.Next(1, 3);
                for (int i = 0; i < futureAppointmentCount; i++)
                {
                    var daysAhead = Random.Next(0, 30);
                    var hour = Random.Next(8, 18);
                    var minute = Random.Next(0, 4) * 15; // Intervalos de 15 min
                    var doctorId = Random.Next(1, 11);
                    var futureStatuses = new[] { AppointmentStatus.Confirmado, AppointmentStatus.Pendiente, AppointmentStatus.Espera };

                    appointments.Add(new Appointment
                    {
                        Id = appointmentId,
                        PatientId = patientId,
                        DoctorId = doctorId,
                        AppointmentDate = today.AddDays(daysAhead).AddHours(hour).AddMinutes(minute),
                        Reason = reasons[Random.Next(reasons.Length)],
                        Status = futureStatuses[Random.Next(futureStatuses.Length)],
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Next(1, 7))
                    });

                    appointmentId++;
                }
            }
        }

        await context.Appointments.AddRangeAsync(appointments);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {appointments.Count} appointments");

        await context.Diagnoses.AddRangeAsync(diagnoses);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {diagnoses.Count} diagnoses");

        await context.Treatments.AddRangeAsync(treatments);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {treatments.Count} treatments");

        await context.Recipes.AddRangeAsync(recipes);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Seeded {recipes.Count} recipes");

        // Resetear las secuencias de autoincremento después del seeding
        await ResetSequences(context);
    }

    /// <summary>
    /// Resetea todas las secuencias de autoincremento para evitar conflictos de ID.
    /// Esto es necesario cuando se insertan datos con IDs explícitos.
    /// </summary>
    private static async Task ResetSequences(ApplicationDbContext context)
    {
        Console.WriteLine("Resetting database sequences...");

        var sequencesToReset = new[]
        {
            "appointments",
            "diagnoses",
            "treatments",
            "recipes",
            "patients",
            "patient_contacts",
            "patient_phones",
            "patient_emails",
            "medical_records",
            "alergies",
            "syndromes",
            "vaccines",
            "chronic_diseases"
        };

        foreach (var tableName in sequencesToReset)
        {
            try
            {
                var sql = $@"
                    SELECT setval(
                        pg_get_serial_sequence('{tableName}', 'id'),
                        COALESCE((SELECT MAX(id) FROM {tableName}), 0) + 1,
                        false
                    );";

                await context.Database.ExecuteSqlRawAsync(sql);
                Console.WriteLine($"  ✓ Reset sequence for {tableName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Warning: Could not reset sequence for {tableName}: {ex.Message}");
            }
        }

        Console.WriteLine("✓ Database sequences reset completed");
    }
}
