using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class RemovePatientIdFromInsuranceAndCreatePatientInsuranceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "insurance_patient_id_fkey",
                table: "insurance");

            migrationBuilder.DropIndex(
                name: "IX_insurance_patient_id",
                table: "insurance");

            migrationBuilder.DropColumn(
                name: "patient_id",
                table: "insurance");

            migrationBuilder.CreateTable(
                name: "patient_insurance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    insurance_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_insurance_pkey", x => x.id);
                    table.ForeignKey(
                        name: "patient_insurance_insurance_id_fkey",
                        column: x => x.insurance_id,
                        principalTable: "insurance",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "patient_insurance_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_patient_insurance_insurance_id",
                table: "patient_insurance",
                column: "insurance_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_insurance_patient_id",
                table: "patient_insurance",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_insurance");

            migrationBuilder.AddColumn<int>(
                name: "patient_id",
                table: "insurance",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_insurance_patient_id",
                table: "insurance",
                column: "patient_id");

            migrationBuilder.AddForeignKey(
                name: "insurance_patient_id_fkey",
                table: "insurance",
                column: "patient_id",
                principalTable: "patients",
                principalColumn: "id");
        }
    }
}
