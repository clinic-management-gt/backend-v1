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
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetAllMedicalRecords(int id)
        {
            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == id)
                .ToListAsync();

            if (!records.Any())
                return NotFound($"Patient with ID {id} has no medical records.");

            return Ok(records);
        }

        [HttpGet("{id}/exams")]
        public async Task<ActionResult<IEnumerable<PatientExam>>> GetAllPatientExams(int id)
        {
            var exams = await _context.PatientExams
                .Where(e => e.PatientId == id)
                .ToListAsync();

            if (!exams.Any())
                return NotFound($"Patient with ID {id} has no exams.");

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

    }
}
