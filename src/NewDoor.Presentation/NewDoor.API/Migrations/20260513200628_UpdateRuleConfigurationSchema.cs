using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDoor.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRuleConfigurationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RuleConfiguration",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "RuleConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IncidentType",
                table: "RuleConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RuleConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Operator",
                table: "RuleConfiguration",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "RuleConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PropertyName",
                table: "RuleConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RuleName",
                table: "RuleConfiguration",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "RuleConfiguration",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Threshold",
                table: "RuleConfiguration",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_RuleConfiguration_EventType",
                table: "RuleConfiguration",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_RuleConfiguration_EventType_IsActive",
                table: "RuleConfiguration",
                columns: new[] { "EventType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RuleConfiguration_IsActive",
                table: "RuleConfiguration",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_EventType",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_EventType_IsActive",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_IsActive",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "IncidentType",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Operator",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "PropertyName",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "RuleName",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Threshold",
                table: "RuleConfiguration");
        }
    }
}
