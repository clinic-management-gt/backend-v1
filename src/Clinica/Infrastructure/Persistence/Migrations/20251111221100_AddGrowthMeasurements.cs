using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clinica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGrowthMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .Annotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .Annotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .Annotation("Npgsql:Enum:measurement_type_enum", "Weight,Height,HeadCircumference,BodyMassIndex")
                .Annotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .OldAnnotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .OldAnnotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .OldAnnotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.CreateTable(
                name: "growth_measurements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    medical_record_id = table.Column<int>(type: "integer", nullable: true),
                    measurement_type = table.Column<int>(type: "measurement_type_enum", nullable: false),
                    value = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("growth_measurements_pkey", x => x.id);
                    table.ForeignKey(
                        name: "growth_measurements_medical_record_id_fkey",
                        column: x => x.medical_record_id,
                        principalTable: "medical_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "growth_measurements_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_growth_measurements_medical_record_id",
                table: "growth_measurements",
                column: "medical_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_growth_measurements_patient_id",
                table: "growth_measurements",
                column: "patient_id");

            migrationBuilder.Sql(@"
                INSERT INTO growth_measurements (patient_id, medical_record_id, measurement_type, value, measured_at, created_at)
                SELECT patient_id, id, 'Weight', weight, COALESCE(created_at, now()), COALESCE(created_at, now())
                FROM medical_records
                WHERE weight IS NOT NULL;

                INSERT INTO growth_measurements (patient_id, medical_record_id, measurement_type, value, measured_at, created_at)
                SELECT patient_id, id, 'Height', height, COALESCE(created_at, now()), COALESCE(created_at, now())
                FROM medical_records
                WHERE height IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "weight",
                table: "medical_records");

            migrationBuilder.DropColumn(
                name: "height",
                table: "medical_records");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "height",
                table: "medical_records",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                table: "medical_records",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE medical_records mr
                SET weight = gm.value
                FROM growth_measurements gm
                WHERE gm.medical_record_id = mr.id AND gm.measurement_type = 'Weight';

                UPDATE medical_records mr
                SET height = gm.value
                FROM growth_measurements gm
                WHERE gm.medical_record_id = mr.id AND gm.measurement_type = 'Height';
            ");

            migrationBuilder.DropTable(
                name: "growth_measurements");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .Annotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .Annotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .Annotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .OldAnnotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .OldAnnotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .OldAnnotation("Npgsql:Enum:measurement_type_enum", "Weight,Height,HeadCircumference,BodyMassIndex")
                .OldAnnotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");
        }
    }
}
