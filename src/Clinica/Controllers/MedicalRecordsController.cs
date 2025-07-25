using Clinica.Models;
using Clinica.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/medicalRecords/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetById(int id)
        {
            try
            {
                var medicalRecord = await _context.MedicalRecords.FindAsync(id);

                if (medicalRecord == null)
                {
                    return NotFound($"Medical record con ID {id} no encontrado.");
                }

                return Ok(medicalRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al consultar medical record: {ex.Message}");
                return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
            }
        }

        // PATCH: api/medicalRecords/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] MedicalRecord medicalRecords)
        {
            try
            {
                var existingRecord = await _context.MedicalRecords.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound($"Medical record con ID {id} no encontrado.");
                }

                // Actualizar solo los campos proporcionados
                if (medicalRecords.PatientId > 0)
                    existingRecord.PatientId = medicalRecords.PatientId;

                if (medicalRecords.Weight > 0)
                    existingRecord.Weight = medicalRecords.Weight;

                if (medicalRecords.Height > 0)
                    existingRecord.Height = medicalRecords.Height;

                if (!string.IsNullOrEmpty(medicalRecords.FamilyHistory))
                    existingRecord.FamilyHistory = medicalRecords.FamilyHistory;

                if (!string.IsNullOrEmpty(medicalRecords.Notes))
                    existingRecord.Notes = medicalRecords.Notes;

                existingRecord.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok($"Medical record con ID {id} actualizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar el medical record: {ex.Message}");
                return StatusCode(500, $"Error al actualizar el medical record: {ex.Message}");
            }
        }

        // POST: /medicalRecords
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] MedicalRecord medicalRecord)
        {
            try
            {
                medicalRecord.CreatedAt = DateTime.UtcNow;

                _context.MedicalRecords.Add(medicalRecord);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = medicalRecord.Id }, medicalRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating medical record: {ex.Message}");
                return StatusCode(500, $"Error creating medical record: {ex.Message}");
            }
        }

        // GET: /medicalRecords/{id}/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult> GetMedicalRecordDetails(int id)
        {
            Console.WriteLine($"➡️ GET /medicalrecords/{id}/details");

            try
            {
                // Obtener el registro médico con información del paciente
                var medicalRecord = await _context.MedicalRecords
                    .Include(mr => mr.Patient)
                    .Where(mr => mr.Id == id)
                    .Select(mr => new
                    {
                        Id = mr.Id,
                        PatientId = mr.PatientId,
                        Weight = mr.Weight,
                        Height = mr.Height,
                        FamilyHistory = mr.FamilyHistory,
                        Notes = mr.Notes,
                        CreatedAt = mr.CreatedAt,
                        UpdatedAt = mr.UpdatedAt,
                        Patient = new
                        {
                            Name = mr.Patient.Name,
                            LastName = mr.Patient.LastName,
                            Birthdate = mr.Patient.Birthdate,
                            Gender = mr.Patient.Gender
                        }
                    })
                    .FirstOrDefaultAsync();

                if (medicalRecord == null)
                {
                    return NotFound($"Medical record with ID {id} not found.");
                }

                // Obtener tratamientos asociados con citas del mismo paciente
                var treatments = await _context.Treatments
                    .Include(t => t.Appointment)
                    .Include(t => t.Medicine)
                    .Where(t => t.Appointment.PatientId == medicalRecord.PatientId)
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
                            Type = t.Medicine.Type
                        },
                        Appointment = new
                        {
                            Date = t.Appointment.AppointmentDate,
                            Reason = t.Appointment.Reason
                        }
                    })
                    .ToListAsync();

                // Obtener exámenes del paciente
                var exams = await _context.PatientExams
                    .Include(pe => pe.Exam)
                    .Where(pe => pe.PatientId == medicalRecord.PatientId)
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

                // Obtener recetas relacionadas con tratamientos del paciente
                var recipes = await _context.Recipes
                    .Include(r => r.Treatment)
                        .ThenInclude(t => t.Appointment)
                    .Where(r => r.Treatment.Appointment.PatientId == medicalRecord.PatientId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        Id = r.Id,
                        TreatmentId = r.TreatmentId,
                        Prescription = r.Prescription,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                var detailedResponse = new
                {
                    MedicalRecord = medicalRecord,
                    Treatments = treatments,
                    Exams = exams,
                    Recipes = recipes,
                    Summary = new
                    {
                        TotalTreatments = treatments.Count,
                        TotalExams = exams.Count,
                        TotalRecipes = recipes.Count
                    }
                };

                return Ok(detailedResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting medical record details: {ex.Message}");
                return StatusCode(500, $"Error retrieving medical record details: {ex.Message}");
            }
        }
    }
}
