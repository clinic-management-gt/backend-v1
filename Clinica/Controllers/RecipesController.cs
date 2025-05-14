using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public RecipesController(IConfiguration config)
        {
            _config = config;
        }

        // POST: /recipes
        [HttpPost]
        public IActionResult CreateRecipe([FromBody] Recipes recipe)
        {
            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
               using var conn = new NpgsqlConnection(connectionString);
               conn.Open();

               var sql = "INSERT INTO recipes (treatment_id, prescription, created_at) " +
                          "VALUES (@treatment_id, @prescription, NOW()) RETURNING id";

               using var cmd = new NpgsqlCommand(sql, conn);
               cmd.Parameters.AddWithValue("treatment_id", recipe.TreatmentId);
               
               // Si Prescription es nulo, se pasa DBNull.Value
               cmd.Parameters.AddWithValue("prescription", recipe.Prescription ?? (object)DBNull.Value);

               var result = cmd.ExecuteScalar();
               if (result == null || result == DBNull.Value)
               {
                     return StatusCode(500, "No se pudo obtener el ID insertado.");
               }

               var newRecipeId = Convert.ToInt32(result);
               recipe.Id = newRecipeId;
               recipe.CreatedAt = DateTime.Now;

               return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating recipe: {ex.Message}");
                return StatusCode(500, $"Error creating recipe: {ex.Message}");
            }
        }

        // GET: /recipes/{id}
        [HttpGet("{id}")]
        public IActionResult GetRecipeById(int id)
        {
            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var sql = "SELECT id, treatment_id, prescription, created_at FROM recipes WHERE id = @id";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var recipe = new Recipes
                    {
                        Id = reader.GetInt32(0),
                        TreatmentId = reader.GetInt32(1),
                        Prescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };
                    return Ok(recipe);
                }
                else
                {
                    return NotFound($"Recipe with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error when consulting recipe: {ex.Message}");
                return StatusCode(500, $"Error querying the database: {ex.Message}");
            }
        }

        // PATCH: /recipes/{id}
        [HttpPatch("{id}")]
        public IActionResult UpdateRecipe(int id, [FromBody] Recipes recipe)
        {
            string? connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var updateFields = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(recipe.Prescription)) updateFields.Add("prescription", recipe.Prescription);
                

                var setClause = string.Join(", ", updateFields.Keys.Select(k => $"{k} = @{k}"));
                var sql = $"UPDATE recipes SET {setClause} WHERE id = @id";

                using var cmd = new NpgsqlCommand(sql, conn);

                foreach (var field in updateFields)
                {
                    cmd.Parameters.AddWithValue(field.Key, field.Value);
                }

                cmd.Parameters.AddWithValue("id", id);

                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok($"Recipe with ID {id} updated.");
                }
                else
                {
                    return NotFound($"Recipe with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating recipe: {ex.Message}");
                return StatusCode(500, $"Error updating recipe: {ex.Message}");
            }
        }
    }
}
