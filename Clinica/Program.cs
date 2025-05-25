using Clinica.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Registrar tu servicio de Cloudflare R2
builder.Services.AddSingleton<CloudflareR2Service>();

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
app.MapGet("/ping", () => Results.Json(new {message = "pong"}) );


// Mapea controladores como /pacientes, /testdb
app.MapControllers();


app.Run();

