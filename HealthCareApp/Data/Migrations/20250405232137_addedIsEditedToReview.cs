using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedIsEditedToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Appointments",
                newName: "Status");

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "PatientPhone",
                table: "Appointments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Appointments",
                newName: "status");

            migrationBuilder.AlterColumn<string>(
                name: "PatientPhone",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);
        }
    }
}
