using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
//using Clinica.Models.EntityFramework;
using Clinica.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/*
// Detectar ruta real del .env (está en backend-v1/.env)
var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env"));
Console.WriteLine($"[BOOT] .env path => {envPath} Exists? {File.Exists(envPath)}");
if (File.Exists(envPath)) Env.Load(envPath);

*/
// Crear builder DESPUÉS de cargar .env
var builder = WebApplication.CreateBuilder(args);


// Asegurar que agregamos env vars (re-lee proceso)
//builder.Configuration.AddEnvironmentVariables();

// Agregar esto antes de builder.Services.AddSingleton<CloudflareR2Service>();
builder.Services.AddHttpClient("R2Client", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

// 1) Registrar tu servicio de Cloudflare R2
builder.Services.AddSingleton<CloudflareR2Service>();

/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//        o => o.MapEnum<AppointmentStatus>("appointment_status_enum"))
*/

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar para que los enums se serialicen como strings
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Opcional: configurar nombres de propiedades en camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Habilita CORS para el frontend en localhost:5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Clinica API",
        Version = "v1",
        Description = "API documentation for Clinica system"
    });
});

// Configuración de autenticación
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
    o.MultipartBodyLengthLimit = 25_000_000; // 25 MB
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication(); // Agregar uso de autenticación
app.UseAuthorization(); // Agregar uso de autorización

// Ejemplo de endpoint básico
app.MapGet("/ping", () => Results.Json(new { message = "pong" }));


// Mapea controladores como /pacientes, /testdb
app.MapControllers();

// (puedes comentar el bloque DotNetEnv si ya usas appsettings.*)
Console.WriteLine("CFG Cloudflare AccountId => " + (builder.Configuration["Cloudflare:AccountId"] ?? "NULL"));

app.Run();

