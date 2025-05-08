using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class PacientesController : ControllerBase
   {
      private readonly IConfiguration _config;

      public PacientesController(IConfiguration config)
      {
         _config = config;
      }
      
      // GET: api/pacientes
      [HttpGet]
      public IActionResult GetAll()
      {
        Console.WriteLine("➡️ Endpoint /pacientes alcanzado (desde DB)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var pacientes = new List<Paciente>();


          var sql = "SELECT id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at, updated_at FROM patients";

          var cmd = new NpgsqlCommand(sql, conn);

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            pacientes.Add(new Paciente{
              Id = reader.GetInt32(0),
              Name = reader.GetString(1),
              LastName = reader.GetString(2),
              Birthdate = reader.GetDateTime(3),
              Address = reader.GetString(4),
              Gender = reader.GetString(5),
              BloodTypeId = reader.GetInt32(6),
              PatientTypeId = reader.GetInt32(7),
              CreatedAt = reader.GetDateTime(8),
              UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
            });
          }

          return Ok(pacientes);
        }
        catch (Exception ex)
        {
         Console.WriteLine($"❌ Error al consultar pacientes: {ex.Message}");
         return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }


      //GetById
      [HttpGet("{id}")]
      public IActionResult GetPacienteById(int id){
        Console.WriteLine($"➡️ Endpoint GET /pacientes/{id} alcanzado (para obtener un paciente)");
        
        string ? connectionString = _config.GetConnectionString("DefaultConnection");

        try{
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();


          var sql = "SELECT id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at, updated_at FROM patients WHERE id = @id";

          var cmd = new NpgsqlCommand(sql, conn);

          cmd.Parameters.AddWithValue("id", id);
  
          using var reader = cmd.ExecuteReader();

          if (reader.Read())  // Si encuentra el paciente
          {
            var paciente = new Paciente  {
              Id = reader.GetInt32(0),
              Name = reader.GetString(1),
              LastName = reader.GetString(2),
              Birthdate = reader.GetDateTime(3),
              Address = reader.GetString(4),
              Gender = reader.GetString(5),
              BloodTypeId = reader.GetInt32(6),
              PatientTypeId = reader.GetInt32(7),
              CreatedAt = reader.GetDateTime(8),
              UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
            };




            return Ok(paciente);
        

          }else{
            return NotFound($"Paciente con ID {id} no encontrado.");
          }

        }catch(Exception ex){
          Console.WriteLine($"❌ Error al consultar paciente: {ex.Message}");
          return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }


      }

      [HttpPost]
      public IActionResult CreatePaciente([FromBody] Paciente paciente){
        Console.WriteLine("➡️ Endpoint POST /pacientes alcanzado (para crear un nuevo paciente)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        //normalizamos en género
        string? gender = paciente.Gender?.Trim().ToLower();
        if (gender == "male"){
          gender = "Male";
        }
        else if (gender == "female")
        {
            gender = "Female";  
        }else{
          gender = "No specified";
        }

        
        try{
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();
          
          var sql = "INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at, updated_at) " +
                    "VALUES (@name, @lastName, @birthdate, @address, @gender, @bloodTypeId, @patientTypeId, NOW(), NULL) RETURNING id";
          
          
          using var cmd = new NpgsqlCommand(sql, conn);

          cmd.Parameters.AddWithValue("name", paciente.Name);
          cmd.Parameters.AddWithValue("lastName", paciente.LastName);
          cmd.Parameters.AddWithValue("birthdate", paciente.Birthdate);
          cmd.Parameters.AddWithValue("address", paciente.Address);
          cmd.Parameters.AddWithValue("gender",gender);
          cmd.Parameters.AddWithValue("bloodTypeId", paciente.BloodTypeId);
          cmd.Parameters.AddWithValue("patientTypeId", paciente.PatientTypeId);

          // Ejecutar la consulta y obtener el ID del nuevo paciente
          var newPacienteId = (int)cmd.ExecuteScalar();  // Esto obtiene el ID del nuevo paciente.

          // Aquí actualizamos el ID del paciente con el valor retornado por la base de datos.
          paciente.Id = newPacienteId;  // Asignamos el ID recién creado al objeto paciente.
          paciente.CreatedAt = DateTime.Now;

          return CreatedAtAction(nameof(GetAll), new { id = paciente.Id }, paciente);


        }catch(Exception ex){
          Console.WriteLine($"❌ Error al crear paciente: {ex.Message}");
          return StatusCode(500, $"Error al crear el paciente: {ex.Message}");

        } 
      }


      [HttpPatch("{id}")]
      public IActionResult UpdatePaciente(int id, [FromBody] Paciente paciente)
      {
          Console.WriteLine($"➡️ Endpoint PATCH /pacientes/{id} alcanzado (para actualizar paciente)");

          string? connectionString = _config.GetConnectionString("DefaultConnection");

          // Limpiar y normalizar el campo "gender"
          string? gender = paciente.Gender?.Trim().ToLower();
          if (gender == "male")
          {
              gender = "Male";
          }
          else if (gender == "female")
          {
              gender = "Female";  
          }
          else
          {
              gender = "No specified";  // Si no es ni male ni female
          }

          try
          {
              using var conn = new NpgsqlConnection(connectionString);
              conn.Open();

              // Crear un diccionario para almacenar las actualizaciones
              var updateFields = new Dictionary<string, object>();

              // Agregar campos al diccionario solo si están presentes en el objeto 'paciente'
              if (!string.IsNullOrEmpty(paciente.Name)) updateFields.Add("name", paciente.Name);
              if (!string.IsNullOrEmpty(paciente.LastName)) updateFields.Add("last_name", paciente.LastName);
              if (paciente.Birthdate != null) updateFields.Add("birthdate", paciente.Birthdate);
              if (!string.IsNullOrEmpty(paciente.Address)) updateFields.Add("address", paciente.Address);
              if (!string.IsNullOrEmpty(gender)) updateFields.Add("gender", gender);
              if (paciente.BloodTypeId != 0) updateFields.Add("blood_type_id", paciente.BloodTypeId);
              if (paciente.PatientTypeId != 0) updateFields.Add("patient_type_id", paciente.PatientTypeId);
              updateFields.Add("updated_at", DateTime.Now);

              // Si el diccionario está vacío, retornar BadRequest
              if (!updateFields.Any())
              {
                  return BadRequest("No hay campos para actualizar.");
              }

              // Construir dinámicamente la consulta SQL
              var setClause = string.Join(", ", updateFields.Keys.Select(k => $"{k} = @{k}"));
              var sql = $"UPDATE patients SET {setClause} WHERE id = @id";

              using var cmd = new NpgsqlCommand(sql, conn);

              // Agregar los parámetros al comando SQL
              foreach (var field in updateFields)
              {
                  cmd.Parameters.AddWithValue(field.Key, field.Value);
              }

              // Agregar el ID del paciente
              cmd.Parameters.AddWithValue("id", id);

              // Ejecutar la consulta
              var rowsAffected = cmd.ExecuteNonQuery();

              if (rowsAffected > 0)
              {
                  return Ok($"Paciente con ID {id} actualizado.");
              }
              else
              {
                  return NotFound($"Paciente con ID {id} no encontrado.");
              }
          }
          catch (Exception ex)
          {
              Console.WriteLine($"❌ Error al actualizar el paciente: {ex.Message}");
              return StatusCode(500, $"Error al actualizar el paciente: {ex.Message}");
          }
      }

      [HttpGet("{id}/medicalrecords")]
      public IActionResult GetAllMedicalRecords(int id)
      {
        Console.WriteLine("➡️ Endpoint /pacientes alcanzado (desde DB)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var sql = "SELECT id, patient_id, weight, height, family_history, notes, created_at, updated_at FROM medical_records WHERE patient_id = @id";

          var cmd = new NpgsqlCommand(sql, conn); 

          cmd.Parameters.AddWithValue("id", id);

            
          var medicalRecords = new List<MedicalRecord>();

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            medicalRecords.Add( new MedicalRecord{
              Id = reader.GetInt32(0),
              PatientId = reader.GetInt32(1),
              Weight = reader.GetDouble(2),
              Height = reader.GetDouble(3),
              FamilyHistory = reader.GetString(4),
              Notes = reader.GetString(5),
              CreatedAt = reader.GetDateTime(6),
              UpdatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });

          } 

          if(medicalRecords.Count > 0){

            return Ok(medicalRecords);
          }else {
            return NotFound($"Paciente con ID {id} no tiene registros medicos.");
          }
          


        }
        catch (Exception ex)
        {
         Console.WriteLine($"❌ Error al consultar pacientes: {ex.Message}");
         return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }

      [HttpPost("{id}/medicalrecords")]
      public IActionResult CreateMedicalRecordByPatientId([FromBody] MedicalRecord medicalRecord){
        Console.WriteLine("➡️ Endpoint POST /pacientes alcanzado (para crear un nuevo paciente)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");
         
        try{
          
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


          // Ejecutar la consulta y obtener el ID del nuevo paciente
          var newPacienteId = (int)cmd.ExecuteScalar();  // Esto obtiene el ID del nuevo paciente.

          // Aquí actualizamos el ID del paciente con el valor retornado por la base de datos.
          medicalRecord.Id = newPacienteId;  // Asignamos el ID recién creado al objeto paciente.
          medicalRecord.CreatedAt = DateTime.Now;

          return CreatedAtAction(nameof(GetAllMedicalRecords), new { id = medicalRecord.Id }, medicalRecord);


        }catch(Exception ex){
          Console.WriteLine($"❌ Error al crear paciente: {ex.Message}");
          return StatusCode(500, $"Error al crear el paciente: {ex.Message}");

        }

      
      }

      [HttpGet("{id}/exams")]
      public IActionResult GetAllPatientExams(int id)
      {

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var sql = "SELECT id, patient_id, exam_id, result_text, result_file_path, created_at FROM patient_exams WHERE patient_id = @id";

          var cmd = new NpgsqlCommand(sql, conn); 

          cmd.Parameters.AddWithValue("id", id);

            
          List<PatientExam> patientExams = new List<PatientExam>();

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            patientExams.Add( new PatientExam{
              Id = reader.GetInt32(0),
              PatientId = reader.GetInt32(1),
              ExamId = reader.GetInt32(2),
              ResultText = reader.GetString(3),
              ResultFilePath = reader.GetString(4),
              CreatedAt = reader.GetDateTime(5),
            });

          } 

          if(patientExams.Count > 0){

            return Ok(patientExams);
          }else {
            return NotFound($"Paciente con ID {id} no tiene examenes medicos.");
          }
          


        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }

      [HttpPost("{id}/exams")]
      public IActionResult CreatePatientExam([FromBody] PatientExam patientExam){

        string? connectionString = _config.GetConnectionString("DefaultConnection"); 
        
        try{
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();
          
          var sql = "INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path, created_at) " +
                    "VALUES (@patient_id, @exam_id, @result_text, @result_file_path, NOW() ) RETURNING id";
          
          
          using var cmd = new NpgsqlCommand(sql, conn);

          cmd.Parameters.AddWithValue("patient_id", patientExam.PatientId);
          cmd.Parameters.AddWithValue("exam_id", patientExam.ExamId);
          cmd.Parameters.AddWithValue("result_text", patientExam.ResultText);
          cmd.Parameters.AddWithValue("result_file_path", patientExam.ResultFilePath);

          // Ejecutar la consulta y obtener el ID del nuevo paciente
          var newPacienteId = (int)cmd.ExecuteScalar();  // Esto obtiene el ID del nuevo paciente.

          patientExam.Id = newPacienteId;  // Asignamos el ID recién creado al objeto paciente.
          patientExam.CreatedAt = DateTime.Now;

          return CreatedAtAction(nameof(GetAllPatientExams), new { id = patientExam.Id }, patientExam);


        }catch(Exception ex){
          Console.WriteLine($"❌ Error al crear paciente: {ex.Message}");
          return StatusCode(500, $"Error al crear el paciente: {ex.Message}");

        }

      
      }
          
      [HttpGet("{id}/growthcurves")]
      public IActionResult GetAllHeightToAgeEntries(int id)
      {

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          string birthdateSql = "SELECT birthdate FROM patients WHERE id = @id";
          NpgsqlCommand birthdateCmd = new NpgsqlCommand(birthdateSql, conn); 
          birthdateCmd.Parameters.AddWithValue("id", id);
          DateTime birthdate = (DateTime)birthdateCmd.ExecuteScalar();

          string medicalRecordSql = "SELECT height, weight, created_at FROM medical_records WHERE patient_id = @id";
          NpgsqlCommand medicalRecordCmd = new NpgsqlCommand(medicalRecordSql, conn); 
          medicalRecordCmd.Parameters.AddWithValue("id", id);

            
          List<HeightToAgeEntryDTO> heightToAgeEntries = new List<HeightToAgeEntryDTO>();
          List<WeightToAgeEntryDTO> weightToAgeEntries = new List<WeightToAgeEntryDTO>();
          List<WeightToHeightEntryDTO> weightToHeightEntries = new List<WeightToHeightEntryDTO>();
          List<BodyMassIndexEntryDTO> bodyMassIndexEntries = new List<BodyMassIndexEntryDTO>();

          using var reader = medicalRecordCmd.ExecuteReader();
          while (reader.Read()){
            double height = reader.GetDouble(0);
            double weight = reader.GetDouble(1);
            int age = reader.GetDateTime(2).Subtract(birthdate).Days;

            heightToAgeEntries.Add( new HeightToAgeEntryDTO
                    {
                        Height = height, 
                        AgeInDays = age, 
                    });
            weightToAgeEntries.Add( new WeightToAgeEntryDTO 
                    {
                        Weight = weight, 
                        AgeInDays = age,

                    });
            weightToHeightEntries.Add(new WeightToHeightEntryDTO
                    {
                       Weight = weight,
                       Height = height,
                    });
            bodyMassIndexEntries.Add(new BodyMassIndexEntryDTO
                    {
                        BodyMassIndex = weight / (height * height),
                        AgeInDays = age
                    });
          } 

          

          if(heightToAgeEntries.Count > 0 || weightToAgeEntries.Count > 0){
            return Ok(new GrowthCurvesDTO(heightToAgeEntries, weightToAgeEntries, weightToHeightEntries, bodyMassIndexEntries));
          }else {
            return NotFound($"Paciente con ID {id} no cuenta con entradas para generar curvas de crecimiento");
          }
          


        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
      }
   }
}
