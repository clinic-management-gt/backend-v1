using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models.EntityFramework;

/// <summary>
/// Contexto principal de la base de datos para la clínica.
/// Gestiona las entidades y la configuración de EF Core.
/// </summary>
public partial class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor sin parámetros. Usado por EF Core.
    /// </summary>
    public ApplicationDbContext()
    {
    }

    /// <summary>
    /// Constructor que recibe opciones de configuración.
    /// </summary>
    /// <param name="options">Opciones de configuración de DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Tabla de alergias.</summary>
    public virtual DbSet<Alergy> Alergies { get; set; }

    /// <summary>Tabla de citas médicas.</summary>
    public virtual DbSet<Appointment> Appointments { get; set; }

    /// <summary>Tabla de tipos de sangre.</summary>
    public virtual DbSet<BloodType> BloodTypes { get; set; }

    /// <summary>Tabla de enfermedades crónicas.</summary>
    public virtual DbSet<ChronicDisease> ChronicDiseases { get; set; }

    /// <summary>Tabla de contactos de pacientes.</summary>
    public virtual DbSet<Contact> Contacts { get; set; }

    /// <summary>Tabla de emails de contactos.</summary>
    public virtual DbSet<Email> Emails { get; set; }

    /// <summary>Tabla de cruce de emails de contactos.</summary>
    public virtual DbSet<ContactEmail> ContactEmails { get; set; }

    /// <summary>Tabla de diagnósticos.</summary>
    public virtual DbSet<Diagnosis> Diagnoses { get; set; }

    /// <summary>Tabla de exámenes médicos.</summary>
    public virtual DbSet<Exam> Exams { get; set; }

    /// <summary>Tabla de historial de cambios.</summary>
    public virtual DbSet<History> Histories { get; set; }

    /// <summary>Tabla de seguros médicos.</summary>
    public virtual DbSet<Insurance> Insurances { get; set; }

    /// <summary>Tabla de logs de acciones.</summary>
    public virtual DbSet<Log> Logs { get; set; }

    /// <summary>Tabla de expedientes médicos.</summary>
    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    /// <summary>Tabla de medicamentos.</summary>
    public virtual DbSet<Medicine> Medicines { get; set; }

    /// <summary>Tabla de módulos del sistema.</summary>
    public virtual DbSet<Module> Modules { get; set; }

    /// <summary>Tabla de pacientes.</summary>
    public virtual DbSet<Patient> Patients { get; set; }

    /// <summary>Tabla de alergias de pacientes.</summary>
    public virtual DbSet<PatientAlergy> PatientAlergies { get; set; }

    /// <summary>Tabla de enfermedades crónicas de pacientes.</summary>
    public virtual DbSet<PatientChronicDisease> PatientChronicDiseases { get; set; }

    /// <summary>Tabla de exámenes de pacientes.</summary>
    public virtual DbSet<PatientExam> PatientExams { get; set; }

    /// <summary>Tabla de tipos de pacientes.</summary>
    public virtual DbSet<PatientType> PatientTypes { get; set; }

    /// <summary>Tabla de vacunas aplicadas a pacientes.</summary>
    public virtual DbSet<PatientVaccine> PatientVaccines { get; set; }

    /// <summary>Tabla de cruce entre pacientes y seguros.</summary>
    public virtual DbSet<PatientInsurance> PatientInsurances { get; set; }

    /// <summary>Tabla de permisos.</summary>
    public virtual DbSet<Permission> Permissions { get; set; }

    /// <summary>Tabla de teléfonos.</summary>
    public virtual DbSet<Phone> Phones { get; set; }

    /// <summary>Tabla de recetas médicas.</summary>
    public virtual DbSet<Recipe> Recipes { get; set; }

    /// <summary>Tabla de roles de usuario.</summary>
    public virtual DbSet<Role> Roles { get; set; }

    /// <summary>Tabla de tenants (multiempresa).</summary>
    public virtual DbSet<Tenant> Tenants { get; set; }

    /// <summary>Tabla de tratamientos médicos.</summary>
    public virtual DbSet<Treatment> Treatments { get; set; }

    /// <summary>Tabla de usuarios.</summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>Tabla de vacunas.</summary>
    public virtual DbSet<Vaccine> Vaccines { get; set; }

    /// <summary>Tabla de documentos de pacientes.</summary>
    public virtual DbSet<PatientDocument> PatientDocuments { get; set; }

    /// <summary>Tabla de pacientes pendientes de confirmar.</summary>
    public virtual DbSet<PendingPatient> PendingPatients { get; set; }

    /// <summary>Tabla de contactos de pacientes pendientes.</summary>
    public virtual DbSet<PendingPatientContact> PendingPatientContacts { get; set; }

    /// <summary>Tabla de teléfonos de contactos de pacientes pendientes.</summary>
    public virtual DbSet<PendingPatientPhone> PendingPatientPhones { get; set; }

    /// <summary>
    /// Configura la conexión a la base de datos si no está configurada.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection", o =>
                o.MapEnum<Enums.FileType>("file_type_enum"));
        }
    }

    /// <summary>
    /// Configura las entidades y relaciones del modelo.
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos de EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("appointment_status_enum", new[] { "Confirmado", "Pendiente", "Completado", "Cancelado", "Espera" })
            .HasPostgresEnum("log_action_enum", new[] { "INSERT", "UPDATE", "DELETE" })
            .HasPostgresEnum("treatment_status_enum", new[] { "Terminado", "No Terminado" })
            .HasPostgresEnum("file_type_enum", new[] { "receta", "hoja_de_informacion", "examen", "laboratorio", "radiografia", "resultado_de_laboratorio", "consentimiento", "otro" })
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("unaccent");

        modelBuilder.Entity<Alergy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alergies_pkey");

            entity.ToTable("alergies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlergyCode)
                .HasMaxLength(10)
                .HasColumnName("alergy_code");
            entity.Property(e => e.AlergyDescription).HasColumnName("alergy_description");
        });


        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("emails_pkey");

            entity.ToTable("emails");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("value");

            entity.HasMany(e => e.ContactEmails)
                .WithOne(ce => ce.Email)
                .HasForeignKey(ce => ce.EmailId)
                .HasConstraintName("contactemails_emailid_fkey");
        });

        modelBuilder.Entity<ContactEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contactemails_pkey");

            entity.ToTable("contactemails");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.EmailId).HasColumnName("email_id");

            entity.HasOne(ce => ce.Contact)
                .WithMany(c => c.ContactEmails)
                .HasForeignKey(ce => ce.ContactId)
                .HasConstraintName("contactemails_contactid_fkey");

            entity.HasOne(ce => ce.Email)
                .WithMany(e => e.ContactEmails)
                .HasForeignKey(ce => ce.EmailId)
                .HasConstraintName("contactemails_emailid_fkey");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("appointment_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_doctor_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_patient_id_fkey");
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("blood_types_pkey");

            entity.ToTable("blood_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Type)
                .HasMaxLength(5)
                .HasColumnName("type");
        });

        modelBuilder.Entity<ChronicDisease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chronic_diseases_pkey");

            entity.ToTable("chronic_diseases");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiseaseCode)
                .HasMaxLength(10)
                .HasColumnName("disease_code");
            entity.Property(e => e.DiseaseDescription).HasColumnName("disease_description");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contacts_pkey");

            entity.ToTable("contacts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Patient).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contacts_patient_id_fkey");
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("diagnosis_pkey");

            entity.ToTable("diagnosis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiagnosisDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("diagnosis_date");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Diagnoses)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("diagnosis_appointment_id_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exams_pkey");

            entity.ToTable("exams");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("history_pkey");

            entity.ToTable("history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.NewData)
                .HasColumnType("json")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("json")
                .HasColumnName("old_data");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("insurance_pkey");

            entity.ToTable("insurance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoverageDetails).HasColumnName("coverage_details");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(50)
                .HasColumnName("policy_number");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("logs_pkey");

            entity.ToTable("logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.NewData)
                .HasColumnType("json")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("json")
                .HasColumnName("old_data");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("logs_user_id_fkey");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medical_records_pkey");

            entity.ToTable("medical_records");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FamilyHistory).HasColumnName("family_history");
            entity.Property(e => e.Height)
                .HasPrecision(5, 2)
                .HasColumnName("height");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Weight)
                .HasPrecision(5, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("medical_records_patient_id_fkey");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medicines_pkey");

            entity.ToTable("medicines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Provider)
                .HasMaxLength(100)
                .HasColumnName("provider");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modules_pkey");

            entity.ToTable("modules");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patients_pkey");

            entity.ToTable("patients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.BloodTypeId).HasColumnName("blood_type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PatientTypeId).HasColumnName("patient_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.BloodType).WithMany(p => p.Patients)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patients_blood_type_id_fkey");

            entity.HasOne(d => d.PatientType).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PatientTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patients_patient_type_id_fkey");
        });

        modelBuilder.Entity<PatientAlergy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_alergies_pkey");

            entity.ToTable("patient_alergies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlergyId).HasColumnName("alergy_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Alergy).WithMany(p => p.PatientAlergies)
                .HasForeignKey(d => d.AlergyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_alergies_alergy_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientAlergies)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_alergies_patient_id_fkey");
        });

        modelBuilder.Entity<PatientChronicDisease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_chronic_diseases_pkey");

            entity.ToTable("patient_chronic_diseases");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChronicDiseaseId).HasColumnName("chronic_disease_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.ChronicDisease).WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(d => d.ChronicDiseaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_chronic_diseases_chronic_disease_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_chronic_diseases_patient_id_fkey");
        });

        modelBuilder.Entity<PatientExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_exams_pkey");

            entity.ToTable("patient_exams");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ResultFilePath)
                .HasMaxLength(255)
                .HasColumnName("result_file_path");
            entity.Property(e => e.ResultText).HasColumnName("result_text");

            entity.HasOne(d => d.Exam).WithMany(p => p.PatientExams)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_exams_exam_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientExams)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_exams_patient_id_fkey");
        });

        modelBuilder.Entity<PatientType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_types_pkey");

            entity.ToTable("patient_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .HasColumnName("color");
        });

        modelBuilder.Entity<PatientVaccine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_vaccines_pkey");

            entity.ToTable("patient_vaccines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgeAtApplication).HasColumnName("age_at_application");
            entity.Property(e => e.ApplicationDate).HasColumnName("application_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Dosis)
                .HasMaxLength(50)
                .HasColumnName("dosis");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientVaccines)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_vaccines_patient_id_fkey");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.PatientVaccines)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_vaccines_vaccine_id_fkey");
        });

        modelBuilder.Entity<PatientInsurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_insurance_pkey");

            entity.ToTable("patient_insurance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientInsurances)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_insurance_patient_id_fkey");

            entity.HasOne(d => d.Insurance).WithMany(p => p.PatientInsurances)
                .HasForeignKey(d => d.InsuranceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_insurance_insurance_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanDelete)
                .HasDefaultValue(false)
                .HasColumnName("can_delete");
            entity.Property(e => e.CanEdit)
                .HasDefaultValue(false)
                .HasColumnName("can_edit");
            entity.Property(e => e.CanView)
                .HasDefaultValue(false)
                .HasColumnName("can_view");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Module).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_module_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_id_fkey");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("phones_pkey");

            entity.ToTable("phones");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Phone1)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Contact).WithMany(p => p.Phones)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phones_contact_id_fkey");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipes_pkey");

            entity.ToTable("recipes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Prescription).HasColumnName("prescription");
            entity.Property(e => e.TreatmentId).HasColumnName("treatment_id");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.TreatmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipes_treatment_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanEdit)
                .HasDefaultValue(false)
                .HasColumnName("can_edit");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tenants_pkey");

            entity.ToTable("tenants");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DbHost).HasColumnName("db_host");
            entity.Property(e => e.DbName)
                .HasMaxLength(50)
                .HasColumnName("db_name");
            entity.Property(e => e.DbPassword).HasColumnName("db_password");
            entity.Property(e => e.DbUser)
                .HasMaxLength(50)
                .HasColumnName("db_user");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("treatments_pkey");

            entity.ToTable("treatments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Dosis).HasColumnName("dosis");
            entity.Property(e => e.Duration)
                .HasMaxLength(50)
                .HasColumnName("duration");
            entity.Property(e => e.Frequency)
                .HasMaxLength(50)
                .HasColumnName("frequency");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Treatments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("treatments_appointment_id_fkey");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Treatments)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("treatments_medicine_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vaccines_pkey");

            entity.ToTable("vaccines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PatientDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_documents_pkey");

            entity.ToTable("patient_documents");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasColumnType("file_type_enum");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileUrl)
                .IsRequired()
                .HasColumnName("file_url");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.MedicalRecordId).HasColumnName("medical_record_id");
            entity.Property(e => e.Size).HasColumnName("size");
            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .HasColumnName("content_type");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("patient_documents_patient_id_fkey");

            entity.HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("patient_documents_uploaded_by_fkey");
        });

        modelBuilder.Entity<PendingPatient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pending_patients_pkey");

            entity.ToTable("pending_patients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .HasColumnName("contact_number");
            entity.Property(e => e.ContactType)
                .HasMaxLength(50)
                .HasColumnName("contact_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    /// <summary>
    /// Método parcial para extender la configuración del modelo.
    /// </summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
