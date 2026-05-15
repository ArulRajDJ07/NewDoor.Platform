using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDoor.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAlarmRuleForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Rule_RuleId",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_RuleId",
                table: "Alarm");

            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "Alarm",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "Alarm",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_RuleId",
                table: "Alarm",
                column: "RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Rule_RuleId",
                table: "Alarm",
                column: "RuleId",
                principalTable: "Rule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
