using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Services;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para controladores
builder.Services.AddControllers();

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

// Agregar el servicio para CloudflareR2Service
builder.Services.AddSingleton<CloudflareR2Service>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Ejemplo de endpoint básico
app.MapGet("/ping", () => Results.Json(new { message = "pong" }));

// Mapea controladores como /pacientes, /testdb
app.MapControllers();

app.Run();
