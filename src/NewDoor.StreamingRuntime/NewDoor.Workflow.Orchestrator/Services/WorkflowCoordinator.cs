using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Services;

public interface IWorkflowCoordinator
{
    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(RuntimeTelemetryEvent telemetryEvent, CancellationToken cancellationToken);
}

public class WorkflowCoordinator : IWorkflowCoordinator
{
    #region Fields
    private readonly IProcessorClient _processorClient;
    private readonly IActionDispatcherClient _actionDispatcherClient;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkflowCoordinator> _logger;
    #endregion

    #region Constructor
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
    #endregion

    #region Workflow Execution
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
            _logger.LogInformation("Starting workflow: {WorkflowId}", workflowResult.WorkflowId);

            workflowResult.ProcessorResponse = await RouteToProcessorAsync(telemetryEvent, cancellationToken);
            workflowResult.ProcessingCompletedAtUtc = DateTime.UtcNow;

            if (workflowResult.ProcessorResponse.IsIncident || workflowResult.ProcessorResponse.IsAlarm)
            {
                workflowResult.ActionResponse = await DispatchActionsAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);
                workflowResult.ActionDispatchedAtUtc = DateTime.UtcNow;

                if (workflowResult.ProcessorResponse.IsIncident)
                    await PublishIncidentAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);

                if (workflowResult.ProcessorResponse.IsAlarm)
                    await PublishAlarmAsync(telemetryEvent, workflowResult.ProcessorResponse, cancellationToken);
            }

            await PublishResultAsync(telemetryEvent, workflowResult, cancellationToken);

            workflowResult.CompletedAtUtc = DateTime.UtcNow;
            workflowResult.Status = "Completed";

            return workflowResult;
        }
        catch (Exception ex)
        {
            workflowResult.Status = "Failed";
            workflowResult.ErrorMessage = ex.Message;
            workflowResult.CompletedAtUtc = DateTime.UtcNow;

            _logger.LogError(ex, "Workflow failed: {WorkflowId}", workflowResult.WorkflowId);
            throw;
        }
    }
    #endregion

    #region Processing
    private async Task<ProcessorResponse> RouteToProcessorAsync(RuntimeTelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        var processorRequest = new ProcessorRequest
        {
            CorrelationId = telemetryEvent.CorrelationId,
            Event = telemetryEvent,
            RequestedAtUtc = DateTime.UtcNow
        };

        return await _processorClient.ProcessEventAsync(processorRequest, cancellationToken);
    }

    private async Task<ActionDispatchResponse> DispatchActionsAsync(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {

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
    #endregion

    #region Publishing
    private async Task PublishIncidentAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {

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
    }

    private async Task PublishAlarmAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {

        var alarmEvent = new AlarmEvent
        {
            AlarmId = $"ALM-{runtimeEvent.CorrelationId}-{DateTime.UtcNow:yyyyMMddHHmmss}",
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
                { "RuleId", 1 },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var alarmTopic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
        await _kafkaProducer.PublishAsync(alarmTopic, alarmEvent.DeviceId, alarmEvent, cancellationToken);
    }

    private async Task PublishResultAsync(RuntimeTelemetryEvent runtimeEvent, WorkflowExecutionResult workflowResult, CancellationToken cancellationToken)
    {

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
    }
    #endregion
}
