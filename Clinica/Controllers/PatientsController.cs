using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Clinica.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class PatientsController : ControllerBase
   {
      private readonly IConfiguration _config;

      public PatientsController(IConfiguration config)
      {
         _config = config;
      }
      
      // GET: api/pacientes
      [HttpGet]
      public IActionResult GetAll()
      {
        Console.WriteLine("➡️ Endpoint /patients reached (from DB)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var patients = new List<Patients>();


          var sql = "SELECT id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at, updated_at FROM patients";

          var cmd = new NpgsqlCommand(sql, conn);

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            patients.Add(new Patients{
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

          return Ok(patients);
        }
        catch (Exception ex)
        {
         Console.WriteLine($"❌ Error when consulting patients: {ex.Message}");
         return StatusCode(500, $"Error when consulting patients: {ex.Message}");
        }
      }


      //GetById
      [HttpGet("{id}")]
      public IActionResult GetPatientById(int id){
        Console.WriteLine($"➡️ Endpoint GET /patients/{id} reached (to get a patient)");
        
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
            var patient = new Patients  {
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
            return Ok(patient);
          }else{
            return NotFound($"Patient with ID {id} not found.");
          }

        }catch(Exception ex){
          Console.WriteLine($"❌ Error when consulting a patient: {ex.Message}");
          return StatusCode(500, $"Error querying the database: {ex.Message}");
        }


      }

      [HttpPost]
      public IActionResult CreatePatient([FromBody] Patients patient){
        Console.WriteLine("➡️ Endpoint POST /patients reached (to create a new patient)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        //normalizamos en género
        string? gender = patient.Gender?.Trim().ToLower();
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

          cmd.Parameters.AddWithValue("name", patient.Name);
          cmd.Parameters.AddWithValue("lastName", patient.LastName);
          cmd.Parameters.AddWithValue("birthdate", patient.Birthdate);
          cmd.Parameters.AddWithValue("address", patient.Address);
          cmd.Parameters.AddWithValue("gender",gender);
          cmd.Parameters.AddWithValue("bloodTypeId", patient.BloodTypeId);
          cmd.Parameters.AddWithValue("patientTypeId", patient.PatientTypeId);

           // Ejecutar la consulta y obtener el ID del nuevo paciente
          var result = cmd.ExecuteScalar();
          if (result == null || result == DBNull.Value)
          {
              return StatusCode(500, "No se pudo obtener el ID insertado.");
          }

          var newPacienteId = Convert.ToInt32(result);// Esto obtiene el ID del nuevo paciente.

          // Aquí actualizamos el ID del paciente con el valor retornado por la base de datos.
          patient.Id = newPacienteId;  // Asignamos el ID recién creado al objeto paciente.
          patient.CreatedAt = DateTime.Now;

          return CreatedAtAction(nameof(GetAll), new { id = patient.Id }, patient);


        }catch(Exception ex){
          Console.WriteLine($"❌ Error creating patient: {ex.Message}");
          return StatusCode(500, $"Error creating patient: {ex.Message}");
        } 
      }


      [HttpPatch("{id}")]
      public IActionResult UpdatePatient(int id, [FromBody] Patients patient)
      {
          Console.WriteLine($"➡️ Endpoint PATCH /patients/{id} reached (to update patient)");

          string? connectionString = _config.GetConnectionString("DefaultConnection");

          // Limpiar y normalizar el campo "gender"
          string? gender = patient.Gender?.Trim().ToLower();
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
              if (!string.IsNullOrEmpty(patient.Name)) updateFields.Add("name", patient.Name);
              if (!string.IsNullOrEmpty(patient.LastName)) updateFields.Add("last_name", patient.LastName);
              updateFields.Add("birthdate", patient.Birthdate);
              if (!string.IsNullOrEmpty(patient.Address)) updateFields.Add("address", patient.Address);
              if (!string.IsNullOrEmpty(gender)) updateFields.Add("gender", gender);
              if (patient.BloodTypeId != 0) updateFields.Add("blood_type_id", patient.BloodTypeId);
              if (patient.PatientTypeId != 0) updateFields.Add("patient_type_id", patient.PatientTypeId);
              updateFields.Add("updated_at", DateTime.Now);

              // Si el diccionario está vacío, retornar BadRequest
              if (!updateFields.Any())
              {
                  return BadRequest("There are no fields to update.");
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
                  return Ok($"Patient with ID {id} updated.");
              }
              else
              {
                  return NotFound($"Patient with ID {id} not found.");
              }
          }
          catch (Exception ex)
          {
              Console.WriteLine($"❌ Error updating patient: {ex.Message}");
              return StatusCode(500, $"Error updating patient: {ex.Message}");
          }
      }

      [HttpGet("{id}/medicalrecords")]
      public IActionResult GetAllMedicalRecords(int id)
      {
        Console.WriteLine("➡️ Endpoint /patients reached (from DB)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var sql = "SELECT id, patient_id, weight, height, family_history, notes, created_at, updated_at FROM medical_records WHERE patient_id = @id";

          var cmd = new NpgsqlCommand(sql, conn); 

          cmd.Parameters.AddWithValue("id", id);

          var medicalRecords = new List<MedicalRecords>();

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            medicalRecords.Add( new MedicalRecords{
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
            return NotFound($"Patient with ID {id} has no medical records.");
          }
          


        }
        catch (Exception ex)
        {
         Console.WriteLine($"❌ Error when consulting patients: {ex.Message}");
         return StatusCode(500, $"Error when consulting patients: {ex.Message}");
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

            
          List<PatientExams> patientExams = new List<PatientExams>();

          using var reader = cmd.ExecuteReader();
          while (reader.Read()){
            patientExams.Add( new PatientExams{
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
            return NotFound($"Patient with ID {id} has no medical examinations.");
          }
          


        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error querying the database: {ex.Message}");
        }
      }
      //TODO: REVISAR ESTE ENDPOIT
      [HttpPost("{id}/exams")]
      public IActionResult CreatePatientExam([FromBody] PatientExams patientExam){

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
          var result = cmd.ExecuteScalar();
          if (result == null || result == DBNull.Value)
          {
              return StatusCode(500, "No se pudo obtener el ID insertado.");
          }

          var newPacienteId = Convert.ToInt32(result);// Esto obtiene el ID del nuevo paciente.

          patientExam.Id = newPacienteId;  // Asignamos el ID recién creado al objeto paciente.
          patientExam.CreatedAt = DateTime.Now;

          return CreatedAtAction(nameof(GetAllPatientExams), new { id = patientExam.Id }, patientExam);


        }catch(Exception ex){
          Console.WriteLine($"❌ Error creating patient: {ex.Message}");
          return StatusCode(500, $"Error creating patient: {ex.Message}");

        }

      
      }
          
      [HttpGet("{id}/growthcurve")]
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
          var result = birthdateCmd.ExecuteScalar();

          if (result == null || result == DBNull.Value)
          {
              return NotFound($"No se encontró la fecha de nacimiento del paciente con ID {id}.");
          }

          DateTime birthdate = Convert.ToDateTime(result);

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
            return Ok(new GrowthCurveDTO(heightToAgeEntries, weightToAgeEntries, weightToHeightEntries, bodyMassIndexEntries));
          }else {
            return NotFound($"Patient with ID {id} does not have inputs to generate growth curves");
          }
          


        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error querying the database: {ex.Message}");
        }
      }

      // patients/{id}/recipes
      [HttpGet("{id}/recipes")]
      public IActionResult GetAllRecipesByPatientID(int id)
      {

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var recipes = new List<Recipes>();

          var sql = @"
            SELECT r.id, r.treatment_id, r.prescription, r.created_at
            FROM recipes r
            JOIN treatments t ON r.treatment_id = t.id
            JOIN appointments a ON t.appointment_id = a.id
            WHERE a.patient_id = @id";

          using var cmd = new NpgsqlCommand(sql, conn);
          cmd.Parameters.AddWithValue("id",id);
          using var reader = cmd.ExecuteReader();

          while (reader.Read())
          {
            recipes.Add(new Recipes
            {
                Id = reader.GetInt32(0),
                TreatmentId = reader.GetInt32(1),
                Prescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            });


          }
          
          return Ok(recipes);


        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error querying the database: {ex.Message}");
        }
      }

            // patients/{id}/recipes
      [HttpGet("{id}/vaccines")]
      public IActionResult GetAllVaccinesByPatientID(int id)
      {

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          var vaccines = new List<PatientVaccineDTO>();

          var sql = @"
            SELECT 
              pv.id,
              v.name AS vaccine_name,
              v.brand,
              pv.dosis,
              pv.age_at_application,
              pv.application_date,
              pv.created_at
          FROM 
              patient_vaccines pv
          JOIN 
              vaccines v ON pv.vaccine_id = v.id
          WHERE 
              pv.patient_id = @id;
          ";

          using var cmd = new NpgsqlCommand(sql, conn);
          cmd.Parameters.AddWithValue("id",id);
          using var reader = cmd.ExecuteReader();

          while (reader.Read())
          {
            vaccines.Add(new PatientVaccineDTO
            {
                Id = reader.GetInt32(0),
                VaccineName = reader.GetString(1),
                Brand = reader.GetString(2),
                Dosis = reader.IsDBNull(3) ? "" : reader.GetString(3),
                AgeAtApplication = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                ApplicationDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                CreatedAt = reader.GetDateTime(6)
            });


          }
          
          return Ok(vaccines);
        }
        catch (Exception ex)
        {
         return StatusCode(500, $"Error querying the database: {ex.Message}");
        }
      }

   }
}
