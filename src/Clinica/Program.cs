using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Clinica;
using Clinica.Infrastructure.Persistence;
using Clinica.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Clinica.Domain.Enums;

public class Program
{
    public static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddHttpClient("R2Client", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        builder.Services.AddSingleton<CloudflareR2Service>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o =>
            {
                o.MapEnum<AppointmentStatus>("appointment_status_enum");
                o.MapEnum<Clinica.Domain.Enums.FileType>("file_type_enum");
            }));

        builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RoutePrefixConvention("api"));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173", "http://172.176.96.39")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Clinica API",
                Version = "v1",
                Description = "API documentation for Clinica system"
            });

            // Configurar para mostrar enums como strings con sus valores
            options.UseInlineDefinitionsForEnums();
        });

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "super_secret_key"))
                };
            });

        builder.Services.Configure<FormOptions>(o =>
        {
            o.MultipartBodyLengthLimit = 25_000_000;
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Aplicar migraciones
            Console.WriteLine("📦 Applying database migrations...");
            await db.Database.MigrateAsync();
            Console.WriteLine("✅ Migrations applied successfully!");

            // Ejecutar seeder apropiado según el ambiente
            var environment = app.Environment.EnvironmentName;
            Console.WriteLine($"🌍 Environment: {environment}");

            if (app.Environment.IsProduction())
            {
                // En producción: solo datos esenciales (usuarios, catálogos)
                Console.WriteLine("🏭 Running PRODUCTION seeder (essential data only)...");
                await Clinica.Infrastructure.Persistence.Seeders.ProductionSeeder.SeedAsync(db);
            }
            else
            {
                // En desarrollo/test: datos completos con 100 pacientes de prueba
                Console.WriteLine("🧪 Running DEVELOPMENT seeder (with test data)...");
                await Clinica.Infrastructure.Persistence.Seeders.DatabaseSeeder.SeedAsync(db);
            }
        }



        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapGet("/ping", () => Results.Json(new { message = "pong" }));
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        Console.WriteLine("CFG Cloudflare AccountId => " + (builder.Configuration["Cloudflare:AccountId"] ?? "NULL"));

        app.Run();
    }
}

