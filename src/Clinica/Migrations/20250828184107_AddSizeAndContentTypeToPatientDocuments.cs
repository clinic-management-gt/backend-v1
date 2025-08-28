using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeAndContentTypeToPatientDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "PatientDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "PatientDocuments",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "PatientDocuments");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "PatientDocuments");
        }
    }
}
