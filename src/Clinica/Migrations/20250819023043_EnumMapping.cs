using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class EnumMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "treatments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<AppointmentStatus>(
                name: "status",
                table: "appointments",
                type: "appointment_status_enum",
                nullable: false,
                defaultValue: AppointmentStatus.Confirmado);
                */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "status",
                table: "treatments");

            migrationBuilder.DropColumn(
                name: "status",
                table: "appointments");
                */
        }
    }
}
