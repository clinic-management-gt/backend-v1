using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Clinica.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var connString = _config.GetConnectionString("DefaultConnection");
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            // UN SOLO SELECT: email + comprobación crypt()
            var sql = @"
            SELECT 
                u.id,
                u.first_name,
                u.last_name,
                u.email,
                u.role_id,
                r.name AS role,
                u.created_at  
            FROM users u
            JOIN roles r ON u.role_id = r.id
            WHERE u.email = @Email
                AND crypt(@Password, u.password_hash) = u.password_hash
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("Email", request.Email);
            cmd.Parameters.AddWithValue("Password", request.Password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                // o bien el email no existe, o la contraseña no coincide
                return Unauthorized(new { error = "Credenciales inválidas" });
            }

            // Si llegamos aquí, el login es válido
            var userId = reader.GetInt32(0);
            var firstName = reader.GetString(1);
            var lastName = reader.GetString(2);
            var email = reader.GetString(3);
            var roleId = reader.GetInt32(4);
            var roleName = reader.GetString(5);
            var createdAt = reader.GetDateTime(6);

            // Genera el JWT
            var token = GenerateJwtToken(userId, firstName, lastName, email, roleId, roleName);

            return Ok(new
            {
                token,
                user = new
                {
                    id = userId,
                    first_name = firstName,
                    last_name = lastName,
                    email = email,
                    role_id = roleId,
                    role = roleName,
                    created_at = createdAt.ToString("o")
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }


    private string GenerateJwtToken(int id, string firstName, string lastName, string email, int roleId, string role)
    {
        var claims = new[]
        {
            new Claim("id", id.ToString()),
            new Claim("first_name", firstName),
            new Claim("last_name", lastName),
            new Claim("email", email),
            new Claim("role_id", roleId.ToString()),
            new Claim("role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "super_secret_key"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "clinica",
            audience: _config["Jwt:Audience"] ?? "clinica",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
