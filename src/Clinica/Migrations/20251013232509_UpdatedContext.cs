using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactEmail_Email_EmailId",
                table: "ContactEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactEmail_contacts_ContactId",
                table: "ContactEmail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Email",
                table: "Email");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEmail",
                table: "ContactEmail");

            migrationBuilder.RenameTable(
                name: "Email",
                newName: "emails");

            migrationBuilder.RenameTable(
                name: "ContactEmail",
                newName: "contactemails");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "emails",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "emails",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contactemails",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EmailId",
                table: "contactemails",
                newName: "email_id");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "contactemails",
                newName: "contact_id");

            migrationBuilder.RenameIndex(
                name: "IX_ContactEmail_EmailId",
                table: "contactemails",
                newName: "IX_contactemails_email_id");

            migrationBuilder.RenameIndex(
                name: "IX_ContactEmail_ContactId",
                table: "contactemails",
                newName: "IX_contactemails_contact_id");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "emails",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "emails_pkey",
                table: "emails",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "contactemails_pkey",
                table: "contactemails",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "contactemails_contactid_fkey",
                table: "contactemails",
                column: "contact_id",
                principalTable: "contacts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "contactemails_emailid_fkey",
                table: "contactemails",
                column: "email_id",
                principalTable: "emails",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "contactemails_contactid_fkey",
                table: "contactemails");

            migrationBuilder.DropForeignKey(
                name: "contactemails_emailid_fkey",
                table: "contactemails");

            migrationBuilder.DropPrimaryKey(
                name: "emails_pkey",
                table: "emails");

            migrationBuilder.DropPrimaryKey(
                name: "contactemails_pkey",
                table: "contactemails");

            migrationBuilder.RenameTable(
                name: "emails",
                newName: "Email");

            migrationBuilder.RenameTable(
                name: "contactemails",
                newName: "ContactEmail");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Email",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Email",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ContactEmail",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "email_id",
                table: "ContactEmail",
                newName: "EmailId");

            migrationBuilder.RenameColumn(
                name: "contact_id",
                table: "ContactEmail",
                newName: "ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_contactemails_email_id",
                table: "ContactEmail",
                newName: "IX_ContactEmail_EmailId");

            migrationBuilder.RenameIndex(
                name: "IX_contactemails_contact_id",
                table: "ContactEmail",
                newName: "IX_ContactEmail_ContactId");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Email",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Email",
                table: "Email",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEmail",
                table: "ContactEmail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactEmail_Email_EmailId",
                table: "ContactEmail",
                column: "EmailId",
                principalTable: "Email",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactEmail_contacts_ContactId",
                table: "ContactEmail",
                column: "ContactId",
                principalTable: "contacts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
