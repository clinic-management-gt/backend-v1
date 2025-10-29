using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class AddFileTypeEnumAndMakeMedicalRecordIdOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .Annotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .Annotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .Annotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .OldAnnotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .OldAnnotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            // Usar SQL raw para convertir correctamente de varchar a enum
            migrationBuilder.Sql(@"
                ALTER TABLE patient_documents
                ALTER COLUMN type TYPE file_type_enum
                USING type::file_type_enum;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convertir de enum de vuelta a varchar
            migrationBuilder.Sql(@"
                ALTER TABLE patient_documents
                ALTER COLUMN type TYPE character varying(100)
                USING type::text;
            ");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .Annotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .Annotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:Enum:appointment_status_enum", "Confirmado,Pendiente,Completado,Cancelado,Espera")
                .OldAnnotation("Npgsql:Enum:file_type_enum", "receta,hoja_de_informacion,examen,laboratorio,radiografia,resultado_de_laboratorio,consentimiento,otro")
                .OldAnnotation("Npgsql:Enum:log_action_enum", "INSERT,UPDATE,DELETE")
                .OldAnnotation("Npgsql:Enum:treatment_status_enum", "Terminado,No Terminado")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            // Eliminar el enum si no se usa en ninguna otra parte
            migrationBuilder.Sql("DROP TYPE IF EXISTS file_type_enum;");
        }
    }
}
