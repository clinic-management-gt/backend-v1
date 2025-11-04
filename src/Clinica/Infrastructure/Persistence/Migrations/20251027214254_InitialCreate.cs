using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clinica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .Annotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .Annotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.CreateTable(
                name: "alergies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alergy_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    alergy_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("alergies_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("blood_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chronic_diseases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    disease_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    disease_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chronic_diseases_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "emails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("emails_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("exams_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    old_data = table.Column<string>(type: "json", nullable: true),
                    new_data = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("history_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medicines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("medicines_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("modules_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patient_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pending_patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    contact_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contact_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pending_patients_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    can_edit = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    db_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    db_host = table.Column<string>(type: "text", nullable: false),
                    db_user = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    db_password = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vaccines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vaccines_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    LastVisit = table.Column<DateOnly>(type: "date", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    blood_type_id = table.Column<int>(type: "integer", nullable: false),
                    patient_type_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patients_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patients_blood_type_id_fkey",
                        column: x => x.blood_type_id,
                        principalTable: "blood_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patients_patient_type_id_fkey",
                        column: x => x.patient_type_id,
                        principalTable: "patient_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    can_view = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    can_edit = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    can_delete = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("permissions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "permissions_module_id_fkey",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "permissions_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                    table.ForeignKey(
                        name: "users_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contacts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "contacts_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "insurance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    provider_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    policy_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    coverage_details = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("insurance_pkey", x => x.id);
                    table.ForeignKey(
                        name: "insurance_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "medical_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    family_history = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("medical_records_pkey", x => x.id);
                    table.ForeignKey(
                        name: "medical_records_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_alergies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    alergy_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_alergies_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_alergies_alergy_id_fkey",
                        column: x => x.alergy_id,
                        principalTable: "alergies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_alergies_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_chronic_diseases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    chronic_disease_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_chronic_diseases_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_chronic_diseases_chronic_disease_id_fkey",
                        column: x => x.chronic_disease_id,
                        principalTable: "chronic_diseases",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_chronic_diseases_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_exams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    exam_id = table.Column<int>(type: "integer", nullable: false),
                    result_text = table.Column<string>(type: "text", nullable: true),
                    result_file_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_exams_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_exams_exam_id_fkey",
                        column: x => x.exam_id,
                        principalTable: "exams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_exams_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_vaccines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    vaccine_id = table.Column<int>(type: "integer", nullable: false),
                    dosis = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    age_at_application = table.Column<int>(type: "integer", nullable: true),
                    application_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_vaccines_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_vaccines_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_vaccines_vaccine_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccines",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    doctor_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<Domain.Enums.AppointmentStatus>(type: "appointment_status_enum", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "appointments_doctor_id_fkey",
                        column: x => x.doctor_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "appointments_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    old_data = table.Column<string>(type: "json", nullable: true),
                    new_data = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("logs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "logs_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    uploaded_by = table.Column<int>(type: "integer", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    medical_record_id = table.Column<int>(type: "integer", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: true),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_documents_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_documents_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_documents_uploaded_by_fkey",
                        column: x => x.uploaded_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contactemails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contact_id = table.Column<int>(type: "integer", nullable: false),
                    email_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contactemails_pkey", x => x.id);
                    table.ForeignKey(
                        name: "contactemails_contactid_fkey",
                        column: x => x.contact_id,
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contactemails_emailid_fkey",
                        column: x => x.email_id,
                        principalTable: "emails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contact_id = table.Column<int>(type: "integer", nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phones_pkey", x => x.id);
                    table.ForeignKey(
                        name: "phones_contact_id_fkey",
                        column: x => x.contact_id,
                        principalTable: "contacts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "diagnosis",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appointment_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    diagnosis_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("diagnosis_pkey", x => x.id);
                    table.ForeignKey(
                        name: "diagnosis_appointment_id_fkey",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "treatments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appointment_id = table.Column<int>(type: "integer", nullable: false),
                    medicine_id = table.Column<int>(type: "integer", nullable: false),
                    dosis = table.Column<string>(type: "text", nullable: false),
                    duration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("treatments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "treatments_appointment_id_fkey",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "treatments_medicine_id_fkey",
                        column: x => x.medicine_id,
                        principalTable: "medicines",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    treatment_id = table.Column<int>(type: "integer", nullable: false),
                    prescription = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("recipes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "recipes_treatment_id_fkey",
                        column: x => x.treatment_id,
                        principalTable: "treatments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_doctor_id",
                table: "appointments",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_patient_id",
                table: "appointments",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_contactemails_contact_id",
                table: "contactemails",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_contactemails_email_id",
                table: "contactemails",
                column: "email_id");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_patient_id",
                table: "contacts",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_diagnosis_appointment_id",
                table: "diagnosis",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_insurance_patient_id",
                table: "insurance",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_logs_user_id",
                table: "logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_medical_records_patient_id",
                table: "medical_records",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_alergies_alergy_id",
                table: "patient_alergies",
                column: "alergy_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_alergies_patient_id",
                table: "patient_alergies",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_chronic_diseases_chronic_disease_id",
                table: "patient_chronic_diseases",
                column: "chronic_disease_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_chronic_diseases_patient_id",
                table: "patient_chronic_diseases",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_documents_patient_id",
                table: "patient_documents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_documents_uploaded_by",
                table: "patient_documents",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_patient_exams_exam_id",
                table: "patient_exams",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_exams_patient_id",
                table: "patient_exams",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_vaccines_patient_id",
                table: "patient_vaccines",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_vaccines_vaccine_id",
                table: "patient_vaccines",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_blood_type_id",
                table: "patients",
                column: "blood_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_patient_type_id",
                table: "patients",
                column: "patient_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_module_id",
                table: "permissions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_role_id",
                table: "permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_phones_contact_id",
                table: "phones",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_treatment_id",
                table: "recipes",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatments_appointment_id",
                table: "treatments",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatments_medicine_id",
                table: "treatments",
                column: "medicine_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contactemails");

            migrationBuilder.DropTable(
                name: "diagnosis");

            migrationBuilder.DropTable(
                name: "history");

            migrationBuilder.DropTable(
                name: "insurance");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "medical_records");

            migrationBuilder.DropTable(
                name: "patient_alergies");

            migrationBuilder.DropTable(
                name: "patient_chronic_diseases");

            migrationBuilder.DropTable(
                name: "patient_documents");

            migrationBuilder.DropTable(
                name: "patient_exams");

            migrationBuilder.DropTable(
                name: "patient_vaccines");

            migrationBuilder.DropTable(
                name: "pending_patients");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "phones");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "emails");

            migrationBuilder.DropTable(
                name: "alergies");

            migrationBuilder.DropTable(
                name: "chronic_diseases");

            migrationBuilder.DropTable(
                name: "exams");

            migrationBuilder.DropTable(
                name: "vaccines");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "treatments");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "medicines");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "blood_types");

            migrationBuilder.DropTable(
                name: "patient_types");
        }
    }
}
