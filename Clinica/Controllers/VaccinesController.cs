using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VaccinesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VaccinesController(IConfiguration config)
        {
            _config = config;
        }

        // POST: /vaccines
        [HttpPost]
        public IActionResult CreateVaccine([FromBody] Vaccine vaccine)
        {
            if (string.IsNullOrEmpty(vaccine.Name) || string.IsNullOrEmpty(vaccine.Brand))
            {
                return BadRequest("Name and Brand are required fields.");
            }

            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var sql = "INSERT INTO vaccines (name, brand, created_at, updated_at) " +
                          "VALUES (@name, @brand, NOW(), NULL) RETURNING id";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("name", vaccine.Name);
                cmd.Parameters.AddWithValue("brand", vaccine.Brand);

                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return StatusCode(500, "No se pudo obtener el ID insertado.");
                }

                var newVaccineId = Convert.ToInt32(result);
                vaccine.Id = newVaccineId;
                vaccine.CreatedAt = DateTime.Now;

                return CreatedAtAction(nameof(GetVaccineById), new { id = vaccine.Id }, vaccine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating vaccine: {ex.Message}");
                return StatusCode(500, $"Error creating vaccine: {ex.Message}");
            }
        }

        // GET: /vaccines/{id}
        [HttpGet("{id}")]
        public IActionResult GetVaccineById(int id)
        {
            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var sql = "SELECT id, name, brand, created_at, updated_at FROM vaccines WHERE id = @id";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var vaccine = new Vaccine
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Brand = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3),
                        UpdatedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                    };
                    return Ok(vaccine);
                }
                else
                {
                    return NotFound($"Vaccine with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error when consulting vaccine: {ex.Message}");
                return StatusCode(500, $"Error querying the database: {ex.Message}");
            }
        }

        // PATCH: /vaccines/{id}
        [HttpPatch("{id}")]
        public IActionResult UpdateVaccine(int id, [FromBody] Vaccine vaccine)
        {
            if (string.IsNullOrEmpty(vaccine.Name) && string.IsNullOrEmpty(vaccine.Brand))
            {
                return BadRequest("At least one field (Name or Brand) must be provided for update.");
            }

            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var updateFields = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(vaccine.Name)) updateFields.Add("name", vaccine.Name);
                if (!string.IsNullOrEmpty(vaccine.Brand)) updateFields.Add("brand", vaccine.Brand);
                updateFields.Add("updated_at", DateTime.Now);

                if (!updateFields.Any())
                {
                    return BadRequest("No fields to update.");
                }

                var setClause = string.Join(", ", updateFields.Keys.Select(k => $"{k} = @{k}"));
                var sql = $"UPDATE vaccines SET {setClause} WHERE id = @id";

                using var cmd = new NpgsqlCommand(sql, conn);

                foreach (var field in updateFields)
                {
                    cmd.Parameters.AddWithValue(field.Key, field.Value);
                }

                cmd.Parameters.AddWithValue("id", id);

                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok($"Vaccine with ID {id} updated.");
                }
                else
                {
                    return NotFound($"Vaccine with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating vaccine: {ex.Message}");
                return StatusCode(500, $"Error updating vaccine: {ex.Message}");
            }
        }
    }
}
