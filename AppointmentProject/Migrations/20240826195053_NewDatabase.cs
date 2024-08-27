using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentProject.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.EnsureSchema(
                name: "Appo");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Appo");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                newName: "PasswordResets",
                newSchema: "Appo");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notifications",
                newSchema: "Appo");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Appointments",
                newSchema: "Appo");

            migrationBuilder.RenameTable(
                name: "ActivityLogs",
                newName: "ActivityLogs",
                newSchema: "Appo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Appo",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                schema: "Appo",
                newName: "PasswordResets");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "Appo",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Appointments",
                schema: "Appo",
                newName: "Appointments");

            migrationBuilder.RenameTable(
                name: "ActivityLogs",
                schema: "Appo",
                newName: "ActivityLogs");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
