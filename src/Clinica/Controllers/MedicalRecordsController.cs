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

        // POST: /medicalRecords  ⭐ MÉTODO PRINCIPAL CORREGIDO
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] MedicalRecord medicalRecord)
        {
            Console.WriteLine($"🎯 NUEVO CONTROLLER - Datos recibidos en CreateMedicalRecord:");
            Console.WriteLine($"   PatientId: {medicalRecord.PatientId}");
            Console.WriteLine($"   Weight: {medicalRecord.Weight}");
            Console.WriteLine($"   Height: {medicalRecord.Height}");
            Console.WriteLine($"   FamilyHistory: {medicalRecord.FamilyHistory}");
            Console.WriteLine($"   Notes: {medicalRecord.Notes}");

            try
            {
                // Validación básica
                if (medicalRecord.PatientId <= 0)
                {
                    Console.WriteLine($"❌ PatientId inválido: {medicalRecord.PatientId}");
                    return BadRequest("PatientId es requerido y debe ser mayor a 0");
                }

                // Verificar que el paciente existe
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == medicalRecord.PatientId);
                if (!patientExists)
                {
                    Console.WriteLine($"❌ Paciente no encontrado: {medicalRecord.PatientId}");
                    return NotFound($"Paciente con ID {medicalRecord.PatientId} no encontrado");
                }

                // Crear un nuevo objeto limpio para evitar problemas de tracking
                var newRecord = new MedicalRecord
                {
                    PatientId = medicalRecord.PatientId,
                    Weight = medicalRecord.Weight,
                    Height = medicalRecord.Height,
                    FamilyHistory = medicalRecord.FamilyHistory,
                    Notes = medicalRecord.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                    // NO incluir Patient - lo manejará EF automáticamente
                };

                Console.WriteLine($"💾 Guardando medical record para paciente {newRecord.PatientId}");

                _context.MedicalRecords.Add(newRecord);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Medical record creado exitosamente con ID: {newRecord.Id}");

                // Retornar respuesta limpia
                var response = new
                {
                    Id = newRecord.Id,
                    PatientId = newRecord.PatientId,
                    Weight = newRecord.Weight,
                    Height = newRecord.Height,
                    FamilyHistory = newRecord.FamilyHistory,
                    Notes = newRecord.Notes,
                    CreatedAt = newRecord.CreatedAt,
                    UpdatedAt = newRecord.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating medical record: {ex.Message}");
                Console.WriteLine($"📋 Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"🔍 Inner exception: {ex.InnerException.Message}");
                    return StatusCode(500, $"Error de validación: {ex.InnerException.Message}");
                }

                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: /medicalRecords/test  ⭐ ENDPOINT DE PRUEBA
        [HttpPost("test")]
        public async Task<IActionResult> TestCreate([FromBody] dynamic data)
        {
            Console.WriteLine($"🧪 TEST ENDPOINT - Ejecutándose correctamente!");
            Console.WriteLine($"   PatientId: {data.PatientId}");
            Console.WriteLine($"   Weight: {data.Weight}");

            try
            {
                var record = new MedicalRecord
                {
                    PatientId = (int)data.PatientId,
                    Weight = data.Weight != null ? (decimal?)Convert.ToDecimal(data.Weight) : null,
                    Height = data.Height != null ? (decimal?)Convert.ToDecimal(data.Height) : null,
                    FamilyHistory = data.FamilyHistory?.ToString(),
                    Notes = data.Notes?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.MedicalRecords.Add(record);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ TEST - Record creado con ID: {record.Id}");

                return Ok(new { 
                    success = true, 
                    id = record.Id, 
                    message = "✅ Created via TEST endpoint - Controller is working!" 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TEST Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // PATCH: api/medicalRecords/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] MedicalRecord medicalRecords)
        {
            try
            {
                Console.WriteLine($"📝 Actualizando medical record ID: {id}");

                var existingRecord = await _context.MedicalRecords.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound($"Medical record con ID {id} no encontrado.");
                }

                // Actualizar campos
                if (medicalRecords.Weight.HasValue)
                    existingRecord.Weight = medicalRecords.Weight;

                if (medicalRecords.Height.HasValue)
                    existingRecord.Height = medicalRecords.Height;

                if (medicalRecords.FamilyHistory != null)
                    existingRecord.FamilyHistory = medicalRecords.FamilyHistory;

                if (medicalRecords.Notes != null)
                    existingRecord.Notes = medicalRecords.Notes;

                existingRecord.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Medical record {id} actualizado exitosamente");

                return Ok(new
                {
                    Id = existingRecord.Id,
                    PatientId = existingRecord.PatientId,
                    Weight = existingRecord.Weight,
                    Height = existingRecord.Height,
                    FamilyHistory = existingRecord.FamilyHistory,
                    Notes = existingRecord.Notes,
                    CreatedAt = existingRecord.CreatedAt,
                    UpdatedAt = existingRecord.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar medical record: {ex.Message}");
                return StatusCode(500, $"Error al actualizar medical record: {ex.Message}");
            }
        }

        // GET: /medicalRecords/{id}/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult> GetMedicalRecordDetails(int id)
        {
            Console.WriteLine($"➡️ GET /medicalrecords/{id}/details");

            try
            {
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

                return Ok(medicalRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting medical record details: {ex.Message}");
                return StatusCode(500, $"Error retrieving medical record details: {ex.Message}");
            }
        }
    }
}