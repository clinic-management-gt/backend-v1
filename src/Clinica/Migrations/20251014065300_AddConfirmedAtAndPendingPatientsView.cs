using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmedAtAndPendingPatientsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Agregar columna confirmed_at a la tabla patients
            migrationBuilder.AddColumn<DateTime>(
                name: "confirmed_at",
                table: "patients",
                type: "timestamp without time zone",
                nullable: true);

            // 2. Crear la vista SQL para pacientes pendientes de confirmación
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW pending_patients_view AS
                SELECT
                    p.id,
                    p.name,
                    p.last_name,
                    COALESCE(ph.phone, '') AS contact_phone,
                    p.created_at
                FROM patients p
                LEFT JOIN contacts c ON c.patient_id = p.id
                LEFT JOIN phones ph ON ph.contact_id = c.id
                WHERE p.confirmed_at IS NULL
                ORDER BY p.created_at DESC;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Eliminar la vista
            migrationBuilder.Sql("DROP VIEW IF EXISTS pending_patients_view;");

            // 2. Eliminar la columna confirmed_at
            migrationBuilder.DropColumn(
                name: "confirmed_at",
                table: "patients");
        }
    }
}
