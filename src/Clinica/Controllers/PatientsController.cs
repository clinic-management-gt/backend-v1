using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll([FromQuery] string? search)
        {
            Console.WriteLine($"➡️ GET /patients with search: {search}");

            IQueryable<Patient> query = _context.Patients;

            if (!string.IsNullOrEmpty(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(lowerSearch) || p.LastName.ToLower().Contains(lowerSearch));
            }

            var patients = await query.ToListAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
        {
            Console.WriteLine("➡️ POST /patients");

            // Normalize gender
            patient.Gender = patient.Gender?.Trim().ToLower() switch
            {
                "male" => "Male",
                "female" => "Female",
                _ => "No specified"
            };

            patient.CreatedAt = DateTime.UtcNow;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            // Normalize gender
            var gender = updatedPatient.Gender?.Trim().ToLower();
            updatedPatient.Gender = gender switch
            {
                "male" => "Male",
                "female" => "Female",
                _ => "No specified"
            };

            // Only update provided fields
            patient.Name = updatedPatient.Name ?? patient.Name;
            patient.LastName = updatedPatient.LastName ?? patient.LastName;
            patient.Birthdate = updatedPatient.Birthdate != default ? updatedPatient.Birthdate : patient.Birthdate;
            patient.Address = updatedPatient.Address ?? patient.Address;
            patient.Gender = updatedPatient.Gender ?? patient.Gender;
            patient.BloodTypeId = updatedPatient.BloodTypeId != 0 ? updatedPatient.BloodTypeId : patient.BloodTypeId;
            patient.PatientTypeId = updatedPatient.PatientTypeId != 0 ? updatedPatient.PatientTypeId : patient.PatientTypeId;
            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok($"Patient with ID {id} updated.");
        }

        [HttpGet("{id}/medicalrecords")]
        public async Task<ActionResult> GetPaginatedMedicalRecords(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0)
        {
            Console.WriteLine($"➡️ GET /patients/{id}/medicalrecords - Page: {page}, Limit: {limit}, Offset: {offset}");

            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                // Calcular offset si se proporciona page
                if (page > 1 && offset == 0)
                {
                    offset = (page - 1) * limit;
                }

                var totalRecords = await _context.MedicalRecords
                    .Where(r => r.PatientId == id)
                    .CountAsync();

                var records = await _context.MedicalRecords
                    .Where(r => r.PatientId == id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip(offset)
                    .Take(limit)
                    .Select(r => new
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        Weight = r.Weight,
                        Height = r.Height,
                        FamilyHistory = r.FamilyHistory,
                        Notes = r.Notes,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    Records = records,
                    Total = totalRecords,
                    Page = page,
                    TotalPages = totalPages,
                    Limit = limit,
                    Offset = offset
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting paginated medical records: {ex.Message}");
                return StatusCode(500, $"Error retrieving medical records: {ex.Message}");
            }
        }

        [HttpGet("{id}/exams")]
        public async Task<ActionResult> GetExamsByPatient(int id)
        {
            var exams = await _context.PatientExams
                .Include(pe => pe.Exam)
                .Where(pe => pe.PatientId == id)
                .OrderByDescending(pe => pe.CreatedAt)
                .Select(pe => new
                {
                    pe.Id,
                    pe.ExamId,
                    pe.ResultText,
                    pe.ResultFilePath,
                    pe.CreatedAt,
                    Exam = new { pe.Exam.Name, pe.Exam.Description }
                })
                .ToListAsync();
            return Ok(exams);
        }

        [HttpGet("{id}/growthcurve")]
        public async Task<ActionResult<GrowthCurveDTO>> GetGrowthCurveData(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == id)
                .ToListAsync();

            if (!records.Any())
                return NotFound($"Patient with ID {id} does not have inputs to generate growth curves");

            var heightToAge = new List<HeightToAgeEntryDTO>();
            var weightToAge = new List<WeightToAgeEntryDTO>();
            var weightToHeight = new List<WeightToHeightEntryDTO>();
            var bmiEntries = new List<BodyMassIndexEntryDTO>();

            foreach (var record in records)
            {
                int ageInDays = (record.CreatedAt.HasValue)
                    ? (record.CreatedAt.Value - patient.Birthdate.ToDateTime(TimeOnly.MinValue)).Days
                    : 0;
                heightToAge.Add(new HeightToAgeEntryDTO { Height = record.Height, AgeInDays = ageInDays });
                weightToAge.Add(new WeightToAgeEntryDTO { Weight = record.Weight, AgeInDays = ageInDays });
                weightToHeight.Add(new WeightToHeightEntryDTO { Weight = record.Weight, Height = record.Height });
                bmiEntries.Add(new BodyMassIndexEntryDTO
                {
                    BodyMassIndex = record.Weight / (record.Height * record.Height),
                    AgeInDays = ageInDays
                });
            }

            var dto = new GrowthCurveDTO(heightToAge, weightToAge, weightToHeight, bmiEntries);
            return Ok(dto);
        }

        [HttpGet("{id}/recipes")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipesByPatientID(int id)
        {
            var recipes = await _context.Recipes
                .Include(r => r.Treatment)
                .ThenInclude(t => t.Appointment)
                .Where(r => r.Treatment.Appointment.PatientId == id)
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id}/vaccines")]
        public async Task<ActionResult<IEnumerable<PatientVaccineDTO>>> GetAllVaccinesByPatientID(int id)
        {
            var vaccines = await _context.PatientVaccines
                .Include(pv => pv.Vaccine)
                .Where(pv => pv.PatientId == id)
                .Select(pv => new PatientVaccineDTO
                {
                    Id = pv.Id,
                    VaccineName = pv.Vaccine.Name,
                    Brand = pv.Vaccine.Brand,
                    Dosis = pv.Dosis,
                    AgeAtApplication = pv.AgeAtApplication,
                    ApplicationDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();

            return Ok(vaccines);
        }
        [HttpGet("{id}/medicalrecords/full")]
        public async Task<IActionResult> GetFullMedicalRecords(int id)
        {
            var records = await _context.Treatments
                .Include(t => t.Appointment)
                .Include(t => t.Recipes)
                .Where(t => t.Appointment.PatientId == id)
                .Select(t => new
                {
                    TreatmentId = t.Id,
                    AppointmentId = t.Appointment.Id,
                    MedicineId = t.MedicineId,
                    Dosis = t.Dosis,
                    Duration = t.Duration,
                    Frequency = t.Frequency,
                    Observaciones = t.Observaciones,
                    Status = t.Status,
                    Recipes = t.Recipes.Select(r => new
                    {
                        RecipeId = r.Id,
                        Prescription = r.Prescription,
                        CreatedAt = r.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("{id}/medicalrecords/{recordId}/full")]
        public async Task<ActionResult> GetFullMedicalRecordByPatientAndRecord(int id, int recordId)
        {
            Console.WriteLine($"➡️ GET /patients/{id}/medicalrecords/{recordId}/full");

            try
            {
                // Verificar que el paciente existe
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                // Verificar que el medical record existe y pertenece al paciente
                var medicalRecord = await _context.MedicalRecords
                    .Include(mr => mr.Patient)
                    .Where(mr => mr.Id == recordId && mr.PatientId == id)
                    .FirstOrDefaultAsync();

                if (medicalRecord == null)
                    return NotFound($"Medical record with ID {recordId} not found for patient {id}.");

                // Obtener todos los tratamientos del paciente
                var treatments = await _context.Treatments
                    .Include(t => t.Appointment)
                    .Include(t => t.Medicine)
                    .Include(t => t.Recipes)
                    .Where(t => t.Appointment.PatientId == id)
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new
                    {
                        Id = t.Id,
                        AppointmentId = t.AppointmentId,
                        MedicineId = t.MedicineId,
                        Dosis = t.Dosis,
                        Duration = t.Duration,
                        Frequency = t.Frequency,
                        Observaciones = t.Observaciones,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        Medicine = new
                        {
                            Name = t.Medicine.Name,
                            Type = t.Medicine.Type,
                            Provider = t.Medicine.Provider
                        },
                        Appointment = new
                        {
                            Date = t.Appointment.AppointmentDate,
                            Reason = t.Appointment.Reason
                        },
                        Recipes = t.Recipes.Select(r => new
                        {
                            Id = r.Id,
                            Prescription = r.Prescription,
                            CreatedAt = r.CreatedAt
                        }).ToList()
                    })
                    .ToListAsync();

                // Obtener todos los exámenes del paciente
                var exams = await _context.PatientExams
                    .Include(pe => pe.Exam)
                    .Where(pe => pe.PatientId == id)
                    .OrderByDescending(pe => pe.CreatedAt)
                    .Select(pe => new
                    {
                        Id = pe.Id,
                        ExamId = pe.ExamId,
                        ResultText = pe.ResultText,
                        ResultFilePath = pe.ResultFilePath,
                        CreatedAt = pe.CreatedAt,
                        Exam = new
                        {
                            Name = pe.Exam.Name,
                            Description = pe.Exam.Description
                        }
                    })
                    .ToListAsync();

                // Obtener todas las recetas del paciente
                var recipes = await _context.Recipes
                    .Include(r => r.Treatment)
                        .ThenInclude(t => t.Appointment)
                    .Include(r => r.Treatment)
                        .ThenInclude(t => t.Medicine)
                    .Where(r => r.Treatment.Appointment.PatientId == id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        Id = r.Id,
                        TreatmentId = r.TreatmentId,
                        Prescription = r.Prescription,
                        CreatedAt = r.CreatedAt,
                        Treatment = new
                        {
                            Medicine = r.Treatment.Medicine.Name,
                            Dosis = r.Treatment.Dosis,
                            Duration = r.Treatment.Duration,
                            Frequency = r.Treatment.Frequency
                        }
                    })
                    .ToListAsync();

                var fullResponse = new
                {
                    Patient = new
                    {
                        Id = medicalRecord.Patient.Id,
                        Name = medicalRecord.Patient.Name,
                        LastName = medicalRecord.Patient.LastName,
                        Birthdate = medicalRecord.Patient.Birthdate,
                        Gender = medicalRecord.Patient.Gender
                    },
                    MedicalRecord = new
                    {
                        Id = medicalRecord.Id,
                        PatientId = medicalRecord.PatientId,
                        Weight = medicalRecord.Weight,
                        Height = medicalRecord.Height,
                        FamilyHistory = medicalRecord.FamilyHistory,
                        Notes = medicalRecord.Notes, // Esta es la "nota de evolución"
                        CreatedAt = medicalRecord.CreatedAt,
                        UpdatedAt = medicalRecord.UpdatedAt
                    },
                    Treatments = treatments,
                    Exams = exams,
                    Recipes = recipes,
                    Summary = new
                    {
                        TotalTreatments = treatments.Count,
                        TotalExams = exams.Count,
                        TotalRecipes = recipes.Count,
                        LastUpdate = medicalRecord.UpdatedAt ?? medicalRecord.CreatedAt
                    }
                };

                return Ok(fullResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting full medical record: {ex.Message}");
                return StatusCode(500, $"Error retrieving full medical record: {ex.Message}");
            }
        }

        [HttpPatch("{id}/medicalrecords/{recordId}")]
        public async Task<ActionResult> UpdateMedicalRecord(int id, int recordId, [FromBody] MedicalRecord updatedRecord)
        {
            Console.WriteLine($"➡️ PATCH /patients/{id}/medicalrecords/{recordId}");

            try
            {
                // Verificar que el paciente existe
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                // Verificar que el medical record existe y pertenece al paciente
                var existingRecord = await _context.MedicalRecords
                    .Where(mr => mr.Id == recordId && mr.PatientId == id)
                    .FirstOrDefaultAsync();

                if (existingRecord == null)
                    return NotFound($"Medical record with ID {recordId} not found for patient {id}.");

                // Actualizar solo los campos proporcionados
                if (updatedRecord.Weight.HasValue && updatedRecord.Weight > 0)
                    existingRecord.Weight = updatedRecord.Weight;

                if (updatedRecord.Height.HasValue && updatedRecord.Height > 0)
                    existingRecord.Height = updatedRecord.Height;

                if (!string.IsNullOrEmpty(updatedRecord.FamilyHistory))
                    existingRecord.FamilyHistory = updatedRecord.FamilyHistory;

                if (!string.IsNullOrEmpty(updatedRecord.Notes))
                    existingRecord.Notes = updatedRecord.Notes;

                existingRecord.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Medical record {recordId} updated successfully for patient {id}",
                    updatedRecord = existingRecord
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating medical record: {ex.Message}");
                return StatusCode(500, $"Error updating medical record: {ex.Message}");
            }
        }

        [HttpPost("{id}/medicalrecords")]
        public async Task<ActionResult> CreateMedicalRecord(int id, [FromBody] MedicalRecord newRecord)
        {
            Console.WriteLine($"➡️ POST /patients/{id}/medicalrecords");

            try
            {
                // Verificar que el paciente existe
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                // Asignar el ID del paciente y fecha de creación
                newRecord.PatientId = id;
                newRecord.CreatedAt = DateTime.UtcNow;
                newRecord.UpdatedAt = null;

                _context.MedicalRecords.Add(newRecord);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetFullMedicalRecordByPatientAndRecord),
                    new { id = id, recordId = newRecord.Id },
                    new
                    {
                        message = $"Medical record created successfully for patient {id}",
                        recordId = newRecord.Id,
                        record = newRecord
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating medical record: {ex.Message}");
                return StatusCode(500, $"Error creating medical record: {ex.Message}");
            }
        }

        [HttpDelete("{id}/medicalrecords/{recordId}")]
        public async Task<ActionResult> DeleteMedicalRecord(int id, int recordId)
        {
            Console.WriteLine($"➡️ DELETE /patients/{id}/medicalrecords/{recordId}");

            try
            {
                // Verificar que el paciente existe
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                // Verificar que el medical record existe y pertenece al paciente
                var recordToDelete = await _context.MedicalRecords
                    .Where(mr => mr.Id == recordId && mr.PatientId == id)
                    .FirstOrDefaultAsync();

                if (recordToDelete == null)
                    return NotFound($"Medical record with ID {recordId} not found for patient {id}.");

                // Verificar si hay registros relacionados que podrían impedir la eliminación
                var hasRelatedRecords = await _context.Treatments
                    .AnyAsync(t => t.Appointment.PatientId == id);

                if (hasRelatedRecords)
                {
                    return BadRequest(new
                    {
                        message = "Cannot delete medical record: patient has related treatments. Consider archiving instead.",
                        suggestion = "Use PATCH to update the record status or notes instead of deleting."
                    });
                }

                _context.MedicalRecords.Remove(recordToDelete);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Medical record {recordId} deleted successfully for patient {id}",
                    deletedRecordId = recordId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting medical record: {ex.Message}");
                return StatusCode(500, $"Error deleting medical record: {ex.Message}");
            }
        }
    }
}
