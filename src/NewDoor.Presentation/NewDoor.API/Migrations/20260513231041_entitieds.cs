using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDoor.API.Migrations
{
    /// <inheritdoc />
    public partial class entitieds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_EventType",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_EventType_IsActive",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Operator",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Priority",
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

            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "RuleConfiguration",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "IncidentType",
                table: "RuleConfiguration",
                newName: "ConfigKey");

            migrationBuilder.AddColumn<string>(
                name: "ConfigValue",
                table: "RuleConfiguration",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RuleId",
                table: "RuleConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rule",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "Rule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Rule",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleCode",
                table: "Rule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RuleName",
                table: "Rule",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RuleType",
                table: "Rule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Rule",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ThresholdValue",
                table: "Rule",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WindowSeconds",
                table: "Rule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Incident",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedUtc",
                table: "Incident",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventCount",
                table: "Incident",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IncidentCode",
                table: "Incident",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IncidentType",
                table: "Incident",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RootCause",
                table: "Incident",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Incident",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedUtc",
                table: "Incident",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Incident",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Incident",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TriggeredByRule",
                table: "Incident",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "EventsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "EventsHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedUtc",
                table: "EventsHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProcessingResult",
                table: "EventsHistory",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessorName",
                table: "EventsHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "EventsHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "EventsHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "BatteryLevel",
                table: "Event",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "Event",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Event",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Event",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventUtc",
                table: "Event",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "Event",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedUtc",
                table: "Event",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Event",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "SignalStrength",
                table: "Event",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SmokeLevel",
                table: "Event",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Event",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Event",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcknowledgedUtc",
                table: "Alarm",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlarmCode",
                table: "Alarm",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AlarmMessage",
                table: "Alarm",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AlarmStatus",
                table: "Alarm",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Alarm",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Alarm",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IncidentId",
                table: "Alarm",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "Alarm",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedUtc",
                table: "Alarm",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuleId",
                table: "Alarm",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Alarm",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TriggeredBy",
                table: "Alarm",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TriggeredUtc",
                table: "Alarm",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RuleConfiguration_RuleId",
                table: "RuleConfiguration",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleConfiguration_RuleId_IsActive",
                table: "RuleConfiguration",
                columns: new[] { "RuleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Rule_DeviceType",
                table: "Rule",
                column: "DeviceType");

            migrationBuilder.CreateIndex(
                name: "IX_Rule_DeviceType_IsActive",
                table: "Rule",
                columns: new[] { "DeviceType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Rule_IsActive",
                table: "Rule",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Rule_RuleCode",
                table: "Rule",
                column: "RuleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incident_BuildingId",
                table: "Incident",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Incident_BuildingId_Status",
                table: "Incident",
                columns: new[] { "BuildingId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Incident_IncidentCode",
                table: "Incident",
                column: "IncidentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incident_IncidentType",
                table: "Incident",
                column: "IncidentType");

            migrationBuilder.CreateIndex(
                name: "IX_Incident_StartedUtc",
                table: "Incident",
                column: "StartedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Incident_Status",
                table: "Incident",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventsHistory_DeviceId",
                table: "EventsHistory",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsHistory_EventId",
                table: "EventsHistory",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsHistory_EventId_ProcessedUtc",
                table: "EventsHistory",
                columns: new[] { "EventId", "ProcessedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EventsHistory_EventType",
                table: "EventsHistory",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_EventsHistory_ProcessedUtc",
                table: "EventsHistory",
                column: "ProcessedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Event_BuildingId",
                table: "Event",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CorrelationId",
                table: "Event",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_DeviceId",
                table: "Event",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_DeviceId_EventUtc",
                table: "Event",
                columns: new[] { "DeviceId", "EventUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventId",
                table: "Event",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventType",
                table: "Event",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventUtc",
                table: "Event",
                column: "EventUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_AlarmCode",
                table: "Alarm",
                column: "AlarmCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_AlarmStatus",
                table: "Alarm",
                column: "AlarmStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_BuildingId",
                table: "Alarm",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_BuildingId_AlarmStatus",
                table: "Alarm",
                columns: new[] { "BuildingId", "AlarmStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_DeviceId",
                table: "Alarm",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_DeviceId_AlarmStatus",
                table: "Alarm",
                columns: new[] { "DeviceId", "AlarmStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_IncidentId",
                table: "Alarm",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_RuleId",
                table: "Alarm",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_TriggeredUtc",
                table: "Alarm",
                column: "TriggeredUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Building_BuildingId",
                table: "Alarm",
                column: "BuildingId",
                principalTable: "Building",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Device_DeviceId",
                table: "Alarm",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Incident_IncidentId",
                table: "Alarm",
                column: "IncidentId",
                principalTable: "Incident",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Alarm_Rule_RuleId",
                table: "Alarm",
                column: "RuleId",
                principalTable: "Rule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Building_BuildingId",
                table: "Event",
                column: "BuildingId",
                principalTable: "Building",
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
                name: "FK_EventsHistory_Device_DeviceId",
                table: "EventsHistory",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsHistory_Event_EventId",
                table: "EventsHistory",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_Building_BuildingId",
                table: "Incident",
                column: "BuildingId",
                principalTable: "Building",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleConfiguration_Rule_RuleId",
                table: "RuleConfiguration",
                column: "RuleId",
                principalTable: "Rule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Building_BuildingId",
                table: "Alarm");

            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Device_DeviceId",
                table: "Alarm");

            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Incident_IncidentId",
                table: "Alarm");

            migrationBuilder.DropForeignKey(
                name: "FK_Alarm_Rule_RuleId",
                table: "Alarm");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Building_BuildingId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Device_DeviceId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsHistory_Device_DeviceId",
                table: "EventsHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsHistory_Event_EventId",
                table: "EventsHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Incident_Building_BuildingId",
                table: "Incident");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleConfiguration_Rule_RuleId",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_RuleId",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_RuleConfiguration_RuleId_IsActive",
                table: "RuleConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_Rule_DeviceType",
                table: "Rule");

            migrationBuilder.DropIndex(
                name: "IX_Rule_DeviceType_IsActive",
                table: "Rule");

            migrationBuilder.DropIndex(
                name: "IX_Rule_IsActive",
                table: "Rule");

            migrationBuilder.DropIndex(
                name: "IX_Rule_RuleCode",
                table: "Rule");

            migrationBuilder.DropIndex(
                name: "IX_Incident_BuildingId",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_BuildingId_Status",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_IncidentCode",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_IncidentType",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_StartedUtc",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_Incident_Status",
                table: "Incident");

            migrationBuilder.DropIndex(
                name: "IX_EventsHistory_DeviceId",
                table: "EventsHistory");

            migrationBuilder.DropIndex(
                name: "IX_EventsHistory_EventId",
                table: "EventsHistory");

            migrationBuilder.DropIndex(
                name: "IX_EventsHistory_EventId_ProcessedUtc",
                table: "EventsHistory");

            migrationBuilder.DropIndex(
                name: "IX_EventsHistory_EventType",
                table: "EventsHistory");

            migrationBuilder.DropIndex(
                name: "IX_EventsHistory_ProcessedUtc",
                table: "EventsHistory");

            migrationBuilder.DropIndex(
                name: "IX_Event_BuildingId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_CorrelationId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_DeviceId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_DeviceId_EventUtc",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventType",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventUtc",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_AlarmCode",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_AlarmStatus",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_BuildingId",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_BuildingId_AlarmStatus",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_DeviceId",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_DeviceId_AlarmStatus",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_IncidentId",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_RuleId",
                table: "Alarm");

            migrationBuilder.DropIndex(
                name: "IX_Alarm_TriggeredUtc",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "ConfigValue",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "RuleConfiguration");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "RuleCode",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "RuleName",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "RuleType",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "ThresholdValue",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "WindowSeconds",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "EndedUtc",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "EventCount",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "IncidentCode",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "IncidentType",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "RootCause",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "StartedUtc",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "TriggeredByRule",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedUtc",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "ProcessingResult",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "ProcessorName",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "EventsHistory");

            migrationBuilder.DropColumn(
                name: "BatteryLevel",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventUtc",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Payload",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ProcessedUtc",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SignalStrength",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SmokeLevel",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "AcknowledgedUtc",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "AlarmCode",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "AlarmMessage",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "AlarmStatus",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "IncidentId",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "ResolvedUtc",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "TriggeredBy",
                table: "Alarm");

            migrationBuilder.DropColumn(
                name: "TriggeredUtc",
                table: "Alarm");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "RuleConfiguration",
                newName: "PropertyName");

            migrationBuilder.RenameColumn(
                name: "ConfigKey",
                table: "RuleConfiguration",
                newName: "IncidentType");

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
        }
    }
}
