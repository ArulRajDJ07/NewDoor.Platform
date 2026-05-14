using NewDoor.Processor.Runtime.Models;
using NewDoor.Processor.Runtime.Services;

namespace NewDoor.Processor.Runtime.Tests;

/// <summary>
/// Example test demonstrating the processor flow
/// This shows how events are processed through the pipeline
/// </summary>
public class ProcessorFlowExample
{
    public async Task<(ProcessorResponse response, AuditHistoryRecord audit)> SimulateFireIncidentProcessing()
    {
        // 1. Simulate incoming event from Kafka topic: newdoor.runtime.processing
        var processorRequest = new ProcessorRequest
        {
            RequestId = "req-" + Guid.NewGuid(),
            CorrelationId = "corr-" + Guid.NewGuid(),
            RequestedAtUtc = DateTime.UtcNow,
            Event = new RuntimeTelemetryEvent
            {
                EventId = "evt-" + Guid.NewGuid(),
                DeviceId = "DEV-001",
                DeviceName = "Smoke Detector - Floor 3",
                DeviceType = "SmokeDetector",
                BuildingId = 1,
                BuildingCode = "BLD-001",
                Floor = "3",
                Zone = "West Wing",
                EventType = "SmokeDetected",
                Temperature = 85.5,      // High temperature
                SmokeLevel = 88.0,       // High smoke level
                BatteryLevel = 95,
                SignalStrength = "Strong",
                Status = "Active",
                Source = "IoT Device",
                TimestampUtc = DateTime.UtcNow
            }
        };

        // 2. Process through EventProcessorService
        // - Rule Evaluation: Detects fire (smoke > 50)
        // - Correlation Engine: Finds nearby devices
        // - Severity Calculation: Critical (smoke > 80)
        // - Alarm Detection: Triggers alarm (Critical severity)

        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventProcessorService>.Instance;
        var historyLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventHistoryCache>.Instance;
        var eventHistory = new EventHistoryCache(historyLogger);
        var processor = new EventProcessorService(logger, null!, eventHistory);
        var response = await processor.ProcessAsync(processorRequest, CancellationToken.None);

        // 3. Create audit history record
        var auditRecord = new AuditHistoryRecord
        {
            RequestId = processorRequest.RequestId,
            CorrelationId = processorRequest.CorrelationId,
            EventId = processorRequest.Event.EventId,
            DeviceId = processorRequest.Event.DeviceId,
            DeviceName = processorRequest.Event.DeviceName,
            BuildingId = processorRequest.Event.BuildingId,
            BuildingCode = processorRequest.Event.BuildingCode,
            Floor = processorRequest.Event.Floor,
            Zone = processorRequest.Event.Zone,
            EventType = processorRequest.Event.EventType,
            ProcessingStage = "RuleEvaluation",
            Action = "ProcessEvent",
            Result = response.IsIncident ? "IncidentDetected" : "NoIncident",
            RuleTriggered = response.RuleTriggered,
            Severity = response.Severity,
            IsIncident = response.IsIncident,
            IsAlarm = response.IsAlarm,
            IncidentType = response.IncidentType,
            ConfidenceScore = response.ConfidenceScore,
            ProcessedAtUtc = response.ProcessedAtUtc
        };

        // Add telemetry metadata
        auditRecord.Metadata["Temperature"] = processorRequest.Event.Temperature;
        auditRecord.Metadata["SmokeLevel"] = processorRequest.Event.SmokeLevel;
        auditRecord.Metadata["BatteryLevel"] = processorRequest.Event.BatteryLevel;

        // 4. At this point, the handler would publish to two Kafka topics:
        //    - newdoor.audit.history (audit record)
        //    - newdoor.runtime.result (processor response)

        // Expected Results:
        // response.IsIncident = true
        // response.IsAlarm = true
        // response.IncidentType = "Fire"
        // response.Severity = "Critical"
        // response.RuleTriggered = "HighSmokeLevel_Rule"

        return (response, auditRecord);
    }

    public async Task<(ProcessorResponse response, AuditHistoryRecord audit)> SimulateLowBatteryIncidentProcessing()
    {
        var processorRequest = new ProcessorRequest
        {
            RequestId = "req-" + Guid.NewGuid(),
            CorrelationId = "corr-" + Guid.NewGuid(),
            RequestedAtUtc = DateTime.UtcNow,
            Event = new RuntimeTelemetryEvent
            {
                EventId = "evt-" + Guid.NewGuid(),
                DeviceId = "DEV-002",
                DeviceName = "Temperature Sensor - Floor 2",
                DeviceType = "TemperatureSensor",
                BuildingId = 1,
                BuildingCode = "BLD-001",
                Floor = "2",
                Zone = "East Wing",
                EventType = "StatusUpdate",
                Temperature = 22.5,      // Normal temperature
                SmokeLevel = 0,          // No smoke
                BatteryLevel = 15,       // Low battery (< 20%)
                SignalStrength = "Weak",
                Status = "Active",
                Source = "IoT Device",
                TimestampUtc = DateTime.UtcNow
            }
        };

        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventProcessorService>.Instance;
        var historyLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventHistoryCache>.Instance;
        var eventHistory = new EventHistoryCache(historyLogger);
        var processor = new EventProcessorService(logger, null!, eventHistory);
        var response = await processor.ProcessAsync(processorRequest, CancellationToken.None);

        var auditRecord = new AuditHistoryRecord
        {
            RequestId = processorRequest.RequestId,
            CorrelationId = processorRequest.CorrelationId,
            EventId = processorRequest.Event.EventId,
            DeviceId = processorRequest.Event.DeviceId,
            DeviceName = processorRequest.Event.DeviceName,
            BuildingId = processorRequest.Event.BuildingId,
            BuildingCode = processorRequest.Event.BuildingCode,
            Floor = processorRequest.Event.Floor,
            Zone = processorRequest.Event.Zone,
            EventType = processorRequest.Event.EventType,
            ProcessingStage = "RuleEvaluation",
            Action = "ProcessEvent",
            Result = response.IsIncident ? "IncidentDetected" : "NoIncident",
            RuleTriggered = response.RuleTriggered,
            Severity = response.Severity,
            IsIncident = response.IsIncident,
            IsAlarm = response.IsAlarm,
            IncidentType = response.IncidentType,
            ConfidenceScore = response.ConfidenceScore,
            ProcessedAtUtc = response.ProcessedAtUtc
        };

        auditRecord.Metadata["Temperature"] = processorRequest.Event.Temperature;
        auditRecord.Metadata["BatteryLevel"] = processorRequest.Event.BatteryLevel;

        // Expected Results:
        // response.IsIncident = true
        // response.IsAlarm = false (Low severity doesn't trigger alarm)
        // response.IncidentType = "LowBattery"
        // response.Severity = "Low"
        // response.RuleTriggered = "LowBattery_Rule"

        return (response, auditRecord);
    }

    public async Task<(ProcessorResponse response, AuditHistoryRecord audit)> SimulateNormalEventProcessing()
    {
        var processorRequest = new ProcessorRequest
        {
            RequestId = "req-" + Guid.NewGuid(),
            CorrelationId = "corr-" + Guid.NewGuid(),
            RequestedAtUtc = DateTime.UtcNow,
            Event = new RuntimeTelemetryEvent
            {
                EventId = "evt-" + Guid.NewGuid(),
                DeviceId = "DEV-003",
                DeviceName = "Temperature Sensor - Floor 1",
                DeviceType = "TemperatureSensor",
                BuildingId = 1,
                BuildingCode = "BLD-001",
                Floor = "1",
                Zone = "Lobby",
                EventType = "StatusUpdate",
                Temperature = 21.5,      // Normal temperature
                SmokeLevel = 0,          // No smoke
                BatteryLevel = 85,       // Good battery
                SignalStrength = "Strong",
                Status = "Active",
                Source = "IoT Device",
                TimestampUtc = DateTime.UtcNow
            }
        };

        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventProcessorService>.Instance;
        var historyLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EventHistoryCache>.Instance;
        var eventHistory = new EventHistoryCache(historyLogger);
        var processor = new EventProcessorService(logger, null!, eventHistory);
        var response = await processor.ProcessAsync(processorRequest, CancellationToken.None);

        var auditRecord = new AuditHistoryRecord
        {
            RequestId = processorRequest.RequestId,
            CorrelationId = processorRequest.CorrelationId,
            EventId = processorRequest.Event.EventId,
            DeviceId = processorRequest.Event.DeviceId,
            DeviceName = processorRequest.Event.DeviceName,
            BuildingId = processorRequest.Event.BuildingId,
            BuildingCode = processorRequest.Event.BuildingCode,
            Floor = processorRequest.Event.Floor,
            Zone = processorRequest.Event.Zone,
            EventType = processorRequest.Event.EventType,
            ProcessingStage = "RuleEvaluation",
            Action = "ProcessEvent",
            Result = response.IsIncident ? "IncidentDetected" : "NoIncident",
            RuleTriggered = response.RuleTriggered,
            Severity = response.Severity,
            IsIncident = response.IsIncident,
            IsAlarm = response.IsAlarm,
            IncidentType = response.IncidentType,
            ConfidenceScore = response.ConfidenceScore,
            ProcessedAtUtc = response.ProcessedAtUtc
        };

        auditRecord.Metadata["Temperature"] = processorRequest.Event.Temperature;
        auditRecord.Metadata["BatteryLevel"] = processorRequest.Event.BatteryLevel;

        // Expected Results:
        // response.IsIncident = false
        // response.IsAlarm = false
        // response.Severity = "None"
        // response.RuleTriggered = ""

        return (response, auditRecord);
    }
}
