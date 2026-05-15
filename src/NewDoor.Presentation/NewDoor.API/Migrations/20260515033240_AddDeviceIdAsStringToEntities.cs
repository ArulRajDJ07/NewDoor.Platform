using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDoor.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdAsStringToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Device_DeviceId",
                table: "Alarm");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Device_DeviceId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Incident_Device_DeviceId",
                table: "Incident");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Incident",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Event",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Alarm",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Incident",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Event",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Alarm",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Device_DeviceId",
                table: "Alarm",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Device_DeviceId",
                table: "Event",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_Device_DeviceId",
                table: "Incident",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }
    }
}
