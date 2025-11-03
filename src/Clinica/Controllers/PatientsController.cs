using Clinica.Models;
using Clinica.Models.EntityFramework;
using Clinica.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("patients")]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CloudflareR2Service _r2;

    public PatientsController(ApplicationDbContext context, CloudflareR2Service r2)
    {
        _context = context;
        _r2 = r2;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterPatient([FromForm] PatientCreateRequestDTO request)
    {
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            DateTime now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            // 1. Create the patient
            Patient patient = new Patient
            {
                Name = request.Name,
                LastName = request.LastName,
                Birthdate = DateOnly.FromDateTime(request.Birthdate),
                Gender = request.Gender,
                Address = request.Address,
                BloodTypeId = request.BloodTypeId,
                PatientTypeId = request.PatientTypeId,
                CreatedAt = now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(); // Needed for patient.Id

            // 2. File upload (unchanged)
            string? fileUrl = null;
            if (request.InfoSheetFile != null && request.InfoSheetFile.Length > 0)
            {
                fileUrl = await _r2.UploadDocumentToCloudflareR2(
                    request.InfoSheetFile, patient.Id, "hoja_de_informacion", medicalRecordId: null);

                PatientDocument doc = new PatientDocument
                {
                    PatientId = patient.Id,
                    Type = Clinica.Models.EntityFramework.Enums.FileType.HojaDeInformacion,
                    Description = "Hoja de información del paciente",
                    FileUrl = fileUrl,
                    UploadedBy = null,
                    UploadedAt = DateTime.UtcNow,
                    Size = request.InfoSheetFile.Length,
                    ContentType = request.InfoSheetFile.ContentType,
                    MedicalRecordId = null
                };

                _context.PatientDocuments.Add(doc);
                await _context.SaveChangesAsync();
            }

            // 3. Allergies
            if (request.Alergies != null && request.Alergies.Any())
            {
                foreach (int alergyId in request.Alergies.Distinct())
                {
                    PatientAlergy patientAlergy = new PatientAlergy
                    {
                        PatientId = patient.Id,
                        AlergyId = alergyId
                    };
                    _context.PatientAlergies.Add(patientAlergy);
                }
            }

            // 4. Syndromes (Chronic Diseases)
            if (request.Syndromes != null && request.Syndromes.Any())
            {
                foreach (int chronicDiseaseId in request.Syndromes.Distinct())
                {
                    PatientChronicDisease chronicDisease = new PatientChronicDisease
                    {
                        PatientId = patient.Id,
                        ChronicDiseaseId = chronicDiseaseId
                    };
                    _context.PatientChronicDiseases.Add(chronicDisease);
                }
            }

            // 5. Insurance link
            if (request.InsuranceId > 0)
            {
                Insurance? insurance = await _context.Insurances.FindAsync(request.InsuranceId);
                if (insurance != null)
                {
                    var patientInsurance = new PatientInsurance
                    {
                        PatientId = patient.Id,
                        InsuranceId = insurance.Id,
                        CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
                    };
                    _context.PatientInsurances.Add(patientInsurance);
                }
            }

            // 6. Contacts
            if (request.Contacts != null && request.Contacts.Any())
            {
                foreach (ContactCreateDTO contactDto in request.Contacts)
                {
                    Contact contact = new Contact
                    {
                        PatientId = patient.Id,
                        Type = contactDto.Type,
                        Name = contactDto.Name,
                        LastName = contactDto.LastName,
                        CreatedAt = now
                    };

                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync(); // Needed for contact.Id

                    // Phones
                    if (contactDto.Phones != null && contactDto.Phones.Any())
                    {
                        foreach (string phoneDto in contactDto.Phones)
                        {
                            Phone phone = new Phone
                            {
                                ContactId = contact.Id,
                                Phone1 = phoneDto,
                                CreatedAt = now
                            };
                            _context.Phones.Add(phone);
                        }
                    }

                    if (contactDto.Emails != null && contactDto.Emails.Any())
                    {
                        foreach (string emailDto in contactDto.Emails)
                        {

                            Email emailEntity = new Email
                            {
                                Value = emailDto
                            };

                            _context.Emails.Add(emailEntity);

                            await _context.SaveChangesAsync(); // Need Email.Id

                            ContactEmail contactEmail = new ContactEmail
                            {
                                ContactId = contact.Id,
                                EmailId = emailEntity.Id
                            };
                            _context.ContactEmails.Add(contactEmail);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                message = "Paciente registrado exitosamente",
                patientId = patient.Id,
            });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error al registrar el paciente." });
        }

    }

    [HttpPost("basic")]
    public async Task<IActionResult> CreateBasicPatient([FromBody] CreateBasicPatientDTO request)
    {
        try
        {
            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var patient = new Patient
            {
                Name = request.Name,
                LastName = request.LastName,
                Birthdate = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), // Valor por defecto
                Gender = request.Gender, // Usar el género del DTO
                Address = "", // Valor por defecto
                BloodTypeId = 1, // Valor por defecto (deberías ajustar según tu DB)
                PatientTypeId = 1, // Valor por defecto (deberías ajustar según tu DB)
                CreatedAt = now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = patient.Id,
                name = patient.Name,
                lastName = patient.LastName,
                message = "Paciente básico creado correctamente."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear el paciente: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? search)
    {
        IQueryable<Patient> query = _context.Patients
            .Include(p => p.BloodType)
            .Include(p => p.PatientType)
            .Include(p => p.Contacts)
            .Include(p => p.PatientInsurances)
                .ThenInclude(pi => pi.Insurance);

        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearch = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerSearch) || p.LastName.ToLower().Contains(lowerSearch));
        }

        var patients = await query
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.LastName,
                p.Birthdate,
                p.LastVisit,
                p.Address,
                p.Gender,
                p.BloodTypeId,
                BloodType = p.BloodType != null ? p.BloodType.Type : null,
                p.PatientTypeId,
                PatientType = p.PatientType != null ? p.PatientType.Name : null,
                p.CreatedAt,
                p.UpdatedAt,
                Contacts = p.Contacts.Select(c => new
                {
                    c.Id,
                    c.Type,
                    c.Name,
                    c.LastName
                }).ToList(),
                PatientInsurances = p.PatientInsurances.Select(pi => new
                {
                    InsuranceId = pi.Insurance.Id,
                    ProviderName = pi.Insurance.ProviderName,
                    PolicyNumber = pi.Insurance.PolicyNumber,
                    CoverageDetails = pi.Insurance.CoverageDetails,
                    AssignedAt = pi.CreatedAt
                }).ToList()
            })
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("pending")]
    public async Task<ActionResult> GetPendingPatients()
    {
        var pendingPatients = await _context.PendingPatients
            .Include(p => p.PendingPatientContacts)
                .ThenInclude(c => c.PendingPatientPhones)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.LastName,
                p.Birthdate,
                p.Gender,
                // Devolver array de contactos con todos los teléfonos
                Contacts = p.PendingPatientContacts.Select(c => new
                {
                    Type = c.Type,
                    Name = c.Name,  // Nombre del contacto
                    PhoneNumbers = c.PendingPatientPhones.Select(ph => ph.Phone).ToList()
                }).ToList(),
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(new { patients = pendingPatients });
    }

    /// <summary>
    /// POST: /api/patients/pending
    /// Crea un nuevo paciente pendiente de confirmar con sus contactos
    /// </summary>
    [HttpPost("pending")]
    public async Task<IActionResult> CreatePendingPatient([FromBody] CreatePendingPatientDTO request)
    {
        var nowUnspecified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        var nowUtc = DateTime.UtcNow;

        // Crear el paciente pendiente
        var pendingPatient = new PendingPatient
        {
            Name = request.Name,
            LastName = request.LastName,
            Birthdate = DateOnly.FromDateTime(request.Birthdate),
            Gender = request.Gender,
            // Mantener compatibilidad con campos legacy (primer contacto)
            ContactNumber = request.Contacts?.FirstOrDefault()?.PhoneNumber,
            ContactType = request.Contacts?.FirstOrDefault()?.Type,
            CreatedAt = nowUnspecified  // pending_patients usa timestamp without time zone
        };

        _context.PendingPatients.Add(pendingPatient);
        await _context.SaveChangesAsync();

        // Crear contactos en tablas relacionadas
        if (request.Contacts != null && request.Contacts.Any())
        {
            foreach (var contactDto in request.Contacts)
            {
                var contact = new PendingPatientContact
                {
                    PendingPatientId = pendingPatient.Id,
                    Type = contactDto.Type,
                    Name = contactDto.Name ?? "",  // Nombre completo del contacto
                    LastName = "",  // Por ahora solo usamos Name para el nombre completo
                    CreatedAt = nowUtc  // pending_patient_contacts usa timestamp with time zone
                };

                _context.PendingPatientContacts.Add(contact);
                await _context.SaveChangesAsync();

                // Agregar teléfono al contacto
                if (!string.IsNullOrEmpty(contactDto.PhoneNumber))
                {
                    var phone = new PendingPatientPhone
                    {
                        PendingPatientContactId = contact.Id,
                        Phone = contactDto.PhoneNumber,
                        CreatedAt = nowUtc  // pending_patient_phones usa timestamp with time zone
                    };

                    _context.PendingPatientPhones.Add(phone);
                }
            }

            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            id = pendingPatient.Id,
            name = pendingPatient.Name,
            lastName = pendingPatient.LastName,
            message = "Paciente pendiente creado correctamente."
        });
    }

    // DELETE /api/patients/pending/{id}
    [HttpDelete("pending/{id}")]
    public async Task<ActionResult> DeletePendingPatient(int id)
    {
        try
        {
            var pendingPatient = await _context.PendingPatients
                .Include(p => p.PendingPatientContacts)
                    .ThenInclude(c => c.PendingPatientPhones)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pendingPatient == null)
                return NotFound("No se encontró el paciente pendiente.");

            // Los contactos y teléfonos se eliminan automáticamente por CASCADE
            _context.PendingPatients.Remove(pendingPatient);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Paciente pendiente eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar el paciente pendiente: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPatientById(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Contacts).ThenInclude(c => c.Phones)
            .Include(p => p.BloodType)
            .Include(p => p.PatientType)
            .Include(p => p.PatientInsurances).ThenInclude(pi => pi.Insurance)
            .Include(p => p.PatientAlergies).ThenInclude(pa => pa.Alergy)
            .Include(p => p.PatientVaccines).ThenInclude(pv => pv.Vaccine)
            .Include(p => p.PatientChronicDiseases).ThenInclude(pc => pc.ChronicDisease)
            .Include(p => p.PatientExams).ThenInclude(pe => pe.Exam)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound($"Patient with ID {id} not found.");

        var response = new
        {
            patient.Id,
            patient.Name,
            patient.LastName,
            patient.Birthdate,
            patient.Address,
            patient.Gender,
            patient.BloodTypeId,
            BloodType = patient.BloodType?.Type,
            patient.PatientTypeId,
            PatientType = patient.PatientType?.Name,
            patient.CreatedAt,
            patient.UpdatedAt,
            Contacts = patient.Contacts.Select(c => new
            {
                c.Id,
                c.Type,
                c.Name,
                c.LastName,
                Phones = c.Phones.Select(ph => ph.Phone1).ToList(),
            }).ToList(),
            Insurances = patient.PatientInsurances.Select(pi => new
            {
                pi.Insurance.Id,
                pi.Insurance.ProviderName,
                pi.Insurance.PolicyNumber,
                pi.Insurance.CoverageDetails,
                AssignedAt = pi.CreatedAt
            }).ToList(),
            Alergies = patient.PatientAlergies.Select(pa => new
            {
                pa.Alergy.Id,
                pa.Alergy.AlergyCode,
                pa.Alergy.AlergyDescription
            }).ToList(),
            PatientVaccines = patient.PatientVaccines.Select(pv => new
            {
                pv.Id,
                VaccineName = pv.Vaccine.Name,
                Brand = pv.Vaccine.Brand,
                Dosis = pv.Dosis,
                AgeAtApplication = pv.AgeAtApplication,
                ApplicationDate = pv.ApplicationDate
            }).ToList(),
            PatientChronicDiseases = patient.PatientChronicDiseases.Select(pc => new
            {
                pc.Id,
                DiseaseCode = pc.ChronicDisease.DiseaseCode,
                Description = pc.ChronicDisease.DiseaseDescription
            }).ToList(),
            PatientExams = patient.PatientExams.Select(pe => new
            {
                pe.Id,
                ExamName = pe.Exam.Name,
                ResultText = pe.ResultText,
                ResultFilePath = pe.ResultFilePath,
                CreatedAt = pe.CreatedAt
            }).ToList()
        };

        return Ok(response);
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
        var vaccinesEntities = await _context.PatientVaccines
            .Include(pv => pv.Vaccine)
            .Where(pv => pv.PatientId == id)
            .AsNoTracking()
            .ToListAsync();

        var vaccines = vaccinesEntities
            .Select(pv => new PatientVaccineDTO
            {
                Id = pv.Id,
                VaccineName = pv.Vaccine?.Name ?? string.Empty,
                Brand = pv.Vaccine?.Brand ?? string.Empty,
                Dosis = pv.Dosis ?? string.Empty,
                AgeAtApplication = pv.AgeAtApplication,
                ApplicationDate = pv.ApplicationDate?.ToDateTime(TimeOnly.MinValue),
                CreatedAt = pv.CreatedAt ?? DateTime.MinValue
            })
            .ToList();

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
            return StatusCode(500, $"Error retrieving full medical record: {ex.Message}");
        }
    }

    [HttpPatch("{id}/medicalrecords/{recordId}")]
    public async Task<ActionResult> UpdateMedicalRecord(int id, int recordId, [FromBody] MedicalRecord updatedRecord)
    {

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
            return StatusCode(500, $"Error updating medical record: {ex.Message}");
        }
    }

    [HttpPost("{id}/medicalrecords")]
    public async Task<ActionResult> CreateMedicalRecord(int id, [FromBody] MedicalRecord newRecord)
    {
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
            return StatusCode(500, $"Error creating medical record: {ex.Message}");
        }
    }

    [HttpDelete("{id}/medicalrecords/{recordId}")]
    public async Task<ActionResult> DeleteMedicalRecord(int id, int recordId)
    {

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
            return StatusCode(500, $"Error deleting medical record: {ex.Message}");
        }
    }

    [HttpGet("{id}/chronic-diseases")]
    public async Task<ActionResult<List<PatientChronicDiseaseDTO>>> GetChronicDiseases(int id)
    {

        var patient = await _context.Patients.FindAsync(id);

        if (patient is null)
            return NotFound();

        List<PatientChronicDiseaseDTO> chronicDiseases = _context.PatientChronicDiseases
            .Where(p => p.PatientId == id)
            .Select(cd =>
                    new PatientChronicDiseaseDTO
                    {
                        Id = cd.ChronicDiseaseId,
                        DiseaseCode = cd.ChronicDisease.DiseaseCode,
                        DiseaseDescription = cd.ChronicDisease.DiseaseDescription,
                    }
                ).ToList();

        return Ok(chronicDiseases);
    }

    [HttpGet("{id}/alergies")]
    public async Task<ActionResult<List<PatientAlergyDTO>>> GetAlergies(int id)
    {
        Patient? patient = await _context.Patients.FindAsync(id);

        if (patient is null)
            return NotFound();

        List<PatientAlergyDTO> alergies = _context.PatientAlergies
            .Where(p => p.PatientId == id)
            .Select(p => new PatientAlergyDTO
            {
                Id = p.AlergyId,
                AlergyCode = p.Alergy.AlergyCode,
                AlergyDescription = p.Alergy.AlergyDescription
            }).ToList();

        return Ok(alergies);
    }

    [HttpGet("{id}/contact_info")]
    public async Task<ActionResult<List<ContactResponseDTO>>> GetPatientContactInformationByPatientId(int id)
    {
        var exists = await _context.Patients.AnyAsync(p => p.Id == id);

        if (!exists)
            return NotFound($"Patient with ID {id} not found.");

        var contacts = await _context.Contacts
            .Where(c => c.PatientId == id)
            .Select(c => new ContactResponseDTO
            {
                Id = c.Id,
                PatientId = c.PatientId,
                Type = c.Type,
                Name = c.Name,
                LastName = c.LastName,
                Phones = c.Phones
                    .OrderBy(p => p.Id)
                    .Select(p => new PhoneResponseDTO
                    {
                        ContactId = p.ContactId,
                        PhoneNumber = p.Phone1,
                    })
                    .ToList(),
                Emails = c.ContactEmails
                    .Select(ce => new EmailDTO
                    {
                        EmailId = ce.EmailId,
                        Email = ce.Email.Value,
                    })
                    .ToList(),
            })
            .ToListAsync();

        return Ok(contacts);
    }
}
