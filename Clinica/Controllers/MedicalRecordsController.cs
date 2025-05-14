using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class MedicalRecordsController : ControllerBase
   {
      private readonly IConfiguration _config;

      public MedicalRecordsController(IConfiguration config)
      {
         _config = config;
      }
      
      //api/medicalRecords/{id}
      [HttpGet("{id}")]
      public IActionResult GetById(int id){
        string ? connectionString = _config.GetConnectionString("DefaultConnection");

        try{
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();


          var sql = "SELECT patient_id, weight, height, family_history, notes, created_at, updated_at FROM medical_records WHERE id = @id";

          var cmd = new NpgsqlCommand(sql, conn);

          cmd.Parameters.AddWithValue("id", id);
  
          using var reader = cmd.ExecuteReader();

          if (reader.Read())  
          {
            MedicalRecords medicalRecords = new MedicalRecords  {
              PatientId = reader.GetInt32(0),
              Weight = reader.GetDouble(1),
              Height = reader.GetDouble(2),
              FamilyHistory = reader.GetString(3),
              Notes = reader.GetString(4),
              CreatedAt = reader.GetDateTime(5),
              UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
            };
            return Ok(medicalRecords); 

          }else{
            return NotFound($"Medical record con ID {id} no encontrado.");
          }

        }catch(Exception ex){
          Console.WriteLine($"❌ Error al consultar medical record: {ex.Message}");
          return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }
      
      [HttpPatch("{id}")]
      public IActionResult UpdateById(int id, [FromBody] MedicalRecords medicalRecords)
      {

          string? connectionString = _config.GetConnectionString("DefaultConnection");

          try
          {
              using var conn = new NpgsqlConnection(connectionString);
              conn.Open();

              var updateFields = new Dictionary<string, object>();

              if (medicalRecords.PatientId > 0) updateFields.Add("patient_id", medicalRecords.PatientId);
              if (medicalRecords.Weight > 0) updateFields.Add("weight", medicalRecords.Weight);
              if (medicalRecords.Height > 0) updateFields.Add("height", medicalRecords.Height);
              if (!String.IsNullOrEmpty(medicalRecords.FamilyHistory)) updateFields.Add("family_history", medicalRecords.FamilyHistory);
              if (!String.IsNullOrEmpty(medicalRecords.Notes)) updateFields.Add("notes", medicalRecords.Notes);
              updateFields.Add("updated_at", DateTime.Now);

              // Si el diccionario está vacío, retornar BadRequest
              if (!updateFields.Any())
              {
                  return BadRequest("No hay campos para actualizar.");
              }

              // Construir dinámicamente la consulta SQL
              var setClause = string.Join(", ", updateFields.Keys.Select(k => $"{k} = @{k}"));
              var sql = $"UPDATE medical_records SET {setClause} WHERE id = @id";

              using var cmd = new NpgsqlCommand(sql, conn);

              // Agregar los parámetros al comando SQL
              foreach (var field in updateFields)
              {
                  cmd.Parameters.AddWithValue(field.Key, field.Value);
              }

              cmd.Parameters.AddWithValue("id", id);

              // Ejecutar la consulta
              var rowsAffected = cmd.ExecuteNonQuery();

              if (rowsAffected > 0)
              {
                  return Ok($"Medical record con ID {id} actualizado.");
              }
              else
              {
                  return NotFound($"Medical record con ID {id} no encontrado.");
              }
          }
          catch (Exception ex)
          {
              Console.WriteLine($"❌ Error al actualizar el medical record: {ex.Message}");
              return StatusCode(500, $"Error al actualizar el medical record: {ex.Message}");
          }
      }

      // POST: /medicalRecords
      [HttpPost]
      public IActionResult CreateMedicalRecord([FromBody] MedicalRecords medicalRecord)
      {
          string? connectionString = _config.GetConnectionString("DefaultConnection");

          try
          {
              using var conn = new NpgsqlConnection(connectionString);
              conn.Open();

              var sql = "INSERT INTO medical_records (patient_id, weight, height, family_history, notes, created_at) " +
                        "VALUES (@patient_id, @weight, @height, @family_history, @notes, NOW()) RETURNING id";

              using var cmd = new NpgsqlCommand(sql, conn);

              cmd.Parameters.AddWithValue("patient_id", medicalRecord.PatientId);
              cmd.Parameters.AddWithValue("weight", medicalRecord.Weight);
              cmd.Parameters.AddWithValue("height", medicalRecord.Height);
              cmd.Parameters.AddWithValue("family_history", medicalRecord.FamilyHistory);
              cmd.Parameters.AddWithValue("notes", medicalRecord.Notes);

              // Ejecutar la consulta y obtener el ID del nuevo registro
              var result = cmd.ExecuteScalar();
              if (result == null || result == DBNull.Value)
              {
                  return StatusCode(500, "No se pudo obtener el ID insertado.");
              }

              var newRecordId = Convert.ToInt32(result);  // Obtiene el ID recién creado
              medicalRecord.Id = newRecordId;  // Asigna el ID al objeto de registro
              medicalRecord.CreatedAt = DateTime.Now;

              return CreatedAtAction(nameof(GetById), new { id = medicalRecord.Id }, medicalRecord);
          }
          catch (Exception ex)
          {
              Console.WriteLine($"❌ Error creating medical record: {ex.Message}");
              return StatusCode(500, $"Error creating medical record: {ex.Message}");
          }
      }

   }
}
