using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Services;

/// <summary>
/// Central orchestrator that coordinates the workflow between:
/// - Listener (Event Consumer)
/// - Processor (Event Processing)
/// - Action Dispatcher (Action Execution)
/// </summary>
public interface IWorkflowCoordinator
{
    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(RuntimeTelemetryEvent telemetryEvent, CancellationToken cancellationToken);
}

public class WorkflowCoordinator : IWorkflowCoordinator
{
    private readonly IProcessorClient _processorClient;
    private readonly IActionDispatcherClient _actionDispatcherClient;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkflowCoordinator> _logger;

    public WorkflowCoordinator(
        IProcessorClient processorClient,
        IActionDispatcherClient actionDispatcherClient,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<WorkflowCoordinator> logger)
    {
        _processorClient = processorClient;
        _actionDispatcherClient = actionDispatcherClient;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WorkflowExecutionResult> ExecuteWorkflowAsync(RuntimeTelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        var workflowResult = new WorkflowExecutionResult
        {
            WorkflowId = Guid.NewGuid().ToString(),
            CorrelationId = telemetryEvent.CorrelationId,
            StartedAtUtc = DateTime.UtcNow
        };

        try
        {
            _logger.LogInformation("=== Starting Workflow Orchestration === WorkflowId={WorkflowId}, EventId={EventId}, DeviceId={DeviceId}",
                workflowResult.WorkflowId, telemetryEvent.EventId, telemetryEvent.DeviceId);

            // Step 1: Route to Processor for analysis
            workflowResult.ProcessorResponse = await RouteToProcessorAsync(telemetryEvent, cancellationToken);
            workflowResult.ProcessingCompletedAtUtc = DateTime.UtcNow;

            // Step 2: Publish audit history
            await PublishAuditHistoryAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);

            // Step 3: Dispatch actions if incident or alarm detected
            if (workflowResult.ProcessorResponse.IsIncident || workflowResult.ProcessorResponse.IsAlarm)
            {
                workflowResult.ActionResponse = await DispatchActionsAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);
                workflowResult.ActionDispatchedAtUtc = DateTime.UtcNow;

                // Step 4: Publish incident event
                if (workflowResult.ProcessorResponse.IsIncident)
                {
                    await PublishIncidentAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);
                }

                // Step 5: Publish alarm event
                if (workflowResult.ProcessorResponse.IsAlarm)
                {
                    await PublishAlarmAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);
                }
            }

            // Step 6: Publish result back to result topic for listener
            await PublishResultAsync(telemetryEvent, workflowResult, cancellationToken);

            workflowResult.CompletedAtUtc = DateTime.UtcNow;
            workflowResult.Status = "Completed";

            _logger.LogInformation("=== Workflow Orchestration Completed === WorkflowId={WorkflowId}, Status={Status}, Duration={Duration}ms",
                workflowResult.WorkflowId, workflowResult.Status, (workflowResult.CompletedAtUtc - workflowResult.StartedAtUtc)?.TotalMilliseconds);

            return workflowResult;
        }
        catch (Exception ex)
        {
            workflowResult.Status = "Failed";
            workflowResult.ErrorMessage = ex.Message;
            workflowResult.CompletedAtUtc = DateTime.UtcNow;

            _logger.LogError(ex, "=== Workflow Orchestration Failed === WorkflowId={WorkflowId}, EventId={EventId}",
                workflowResult.WorkflowId, telemetryEvent.EventId);

            throw;
        }
    }

    private async Task<ProcessorResponse> RouteToProcessorAsync(RuntimeTelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Step 1: Routing to Processor for analysis - EventId={EventId}", telemetryEvent.EventId);

        var processorRequest = new ProcessorRequest
        {
            CorrelationId = telemetryEvent.CorrelationId,
            Event = telemetryEvent,
            RequestedAtUtc = DateTime.UtcNow
        };

        var response = await _processorClient.ProcessEventAsync(processorRequest, cancellationToken);

        _logger.LogInformation("← Step 1 Complete: Processor analysis finished - IsIncident={IsIncident}, IsAlarm={IsAlarm}, Severity={Severity}",
            response.IsIncident, response.IsAlarm, response.Severity);

        return response;
    }

    private async Task<ActionDispatchResponse> DispatchActionsAsync(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Step 2: Dispatching actions - IncidentType={IncidentType}, Severity={Severity}",
            processorResponse.IncidentType, processorResponse.Severity);

        var actionRequest = new ActionDispatchRequest
        {
            CorrelationId = telemetryEvent.CorrelationId,
            ActionType = DetermineActionType(processorResponse),
            Severity = processorResponse.Severity,
            IncidentType = processorResponse.IncidentType,
            DeviceId = telemetryEvent.DeviceId,
            BuildingId = telemetryEvent.BuildingId,
            BuildingCode = telemetryEvent.BuildingCode,
            Floor = telemetryEvent.Floor,
            Zone = telemetryEvent.Zone,
            Context = new Dictionary<string, object>
            {
                { "Temperature", telemetryEvent.Temperature },
                { "SmokeLevel", telemetryEvent.SmokeLevel },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var response = await _actionDispatcherClient.DispatchActionAsync(actionRequest, cancellationToken);

        _logger.LogInformation("← Step 2 Complete: Actions dispatched - DispatchId={DispatchId}, Status={Status}",
            response.DispatchId, response.Status);

        return response;
    }

    private string DetermineActionType(ProcessorResponse processorResponse)
    {
        if (processorResponse.IsAlarm && processorResponse.Severity == "Critical")
        {
            return "EmergencyAlert";
        }
        else if (processorResponse.IsAlarm)
        {
            return "StandardAlert";
        }
        else if (processorResponse.IsIncident)
        {
            return "IncidentNotification";
        }

        return "Notification";
    }

    private async Task PublishAuditHistoryAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Audit History - EventId={EventId}", runtimeEvent.EventId);

        var auditEvent = new AuditHistoryEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            EventType = runtimeEvent.EventType,
            DeviceId = runtimeEvent.DeviceId,
            EntityType = "TelemetryEvent",
            EntityId = runtimeEvent.EventId,
            Action = "Processed",
            Details = $"Event processed with result: IsIncident={processorResponse.IsIncident}, Severity={processorResponse.Severity}",
            CreatedAtUtc = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "BuildingId", runtimeEvent.BuildingId },
                { "ProcessorResponseId", processorResponse.ResponseId }
            }
        };

        var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
        await _kafkaProducer.PublishAsync(auditTopic, auditEvent.DeviceId, auditEvent, cancellationToken);

        _logger.LogInformation("← Audit History Published - AuditId={AuditId}, Topic={Topic}",
            auditEvent.AuditId, auditTopic);
    }

    private async Task PublishIncidentAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Incident Event - IncidentType={IncidentType}", processorResponse.IncidentType);

        var incidentEvent = new IncidentEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            DeviceId = runtimeEvent.DeviceId,
            DeviceName = runtimeEvent.DeviceName,
            BuildingId = runtimeEvent.BuildingId,
            BuildingCode = runtimeEvent.BuildingCode,
            IncidentType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            ConfidenceScore = processorResponse.ConfidenceScore,
            RuleTriggered = processorResponse.RuleTriggered,
            DetectedAtUtc = DateTime.UtcNow,
            TelemetryData = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "BatteryLevel", runtimeEvent.BatteryLevel },
                { "Floor", runtimeEvent.Floor },
                { "Zone", runtimeEvent.Zone }
            }
        };

        var incidentTopic = _configuration["Kafka:IncidentDetectedTopic"] ?? "newdoor.incident.detected";
        await _kafkaProducer.PublishAsync(incidentTopic, incidentEvent.DeviceId, incidentEvent, cancellationToken);

        _logger.LogInformation("← Incident Event Published - IncidentId={IncidentId}, Topic={Topic}",
            incidentEvent.IncidentId, incidentTopic);
    }

    private async Task PublishAlarmAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Alarm Event - AlarmType={AlarmType}", processorResponse.IncidentType);

        var alarmEvent = new AlarmEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            DeviceId = runtimeEvent.DeviceId,
            DeviceName = runtimeEvent.DeviceName,
            BuildingId = runtimeEvent.BuildingId,
            BuildingCode = runtimeEvent.BuildingCode,
            Floor = runtimeEvent.Floor,
            Zone = runtimeEvent.Zone,
            AlarmType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            Message = $"High severity {processorResponse.IncidentType} detected at {runtimeEvent.BuildingCode} - {runtimeEvent.Floor}/{runtimeEvent.Zone}",
            TriggeredAtUtc = DateTime.UtcNow,
            Context = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var alarmTopic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
        await _kafkaProducer.PublishAsync(alarmTopic, alarmEvent.DeviceId, alarmEvent, cancellationToken);

        _logger.LogInformation("← Alarm Event Published - AlarmId={AlarmId}, Topic={Topic}",
            alarmEvent.AlarmId, alarmTopic);
    }

    private async Task PublishResultAsync(RuntimeTelemetryEvent runtimeEvent, WorkflowExecutionResult workflowResult, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Workflow Result - WorkflowId={WorkflowId}", workflowResult.WorkflowId);

        var resultEvent = new RuntimeResultEvent
        {
            WorkflowId = workflowResult.WorkflowId,
            CorrelationId = runtimeEvent.CorrelationId,
            EventId = runtimeEvent.EventId,
            DeviceId = runtimeEvent.DeviceId,
            ProcessingStatus = workflowResult.Status,
            IsIncident = workflowResult.ProcessorResponse.IsIncident,
            IsAlarm = workflowResult.ProcessorResponse.IsAlarm,
            Severity = workflowResult.ProcessorResponse.Severity,
            ActionDispatched = workflowResult.ActionResponse != null,
            ProcessedAtUtc = workflowResult.CompletedAtUtc ?? DateTime.UtcNow,
            ProcessingDurationMs = (workflowResult.CompletedAtUtc - workflowResult.StartedAtUtc)?.TotalMilliseconds ?? 0
        };

        var resultTopic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
        await _kafkaProducer.PublishAsync(resultTopic, resultEvent.DeviceId, resultEvent, cancellationToken);

        _logger.LogInformation("← Workflow Result Published - ResultId={ResultId}, Topic={Topic}",
            resultEvent.ResultId, resultTopic);
    }
}
