using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDoor.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Incident",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incident_DeviceId",
                table: "Incident",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_Device_DeviceId",
                table: "Incident",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incident_Device_DeviceId",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_DeviceId",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Incident");
        }
    }
}
