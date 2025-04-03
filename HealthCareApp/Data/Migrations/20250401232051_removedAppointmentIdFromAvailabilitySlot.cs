using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedAppointmentIdFromAvailabilitySlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "AvailabilitySlots");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments",
                column: "SlotId",
                principalTable: "AvailabilitySlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "AvailabilitySlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments",
                column: "SlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments",
                column: "SlotId",
                principalTable: "AvailabilitySlots",
                principalColumn: "Id");
        }
    }
}
