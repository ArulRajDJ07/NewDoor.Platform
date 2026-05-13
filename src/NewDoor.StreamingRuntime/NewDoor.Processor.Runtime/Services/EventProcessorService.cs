using NewDoor.Processor.Runtime.Models;

namespace NewDoor.Processor.Runtime.Services;

public interface IEventProcessorService
{
    Task<ProcessorResponse> ProcessAsync(ProcessorRequest request, CancellationToken cancellationToken);
}

public class EventProcessorService : IEventProcessorService
{
    private readonly ILogger<EventProcessorService> _logger;

    public EventProcessorService(ILogger<EventProcessorService> logger)
    {
        _logger = logger;
    }

    public async Task<ProcessorResponse> ProcessAsync(ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing event: RequestId={RequestId}, EventId={EventId}, EventType={EventType}", 
                request.RequestId, request.Event.EventId, request.Event.EventType);

            var response = new ProcessorResponse
            {
                RequestId = request.RequestId,
                CorrelationId = request.CorrelationId,
                ProcessedAtUtc = DateTime.UtcNow
            };

            await Task.Run(() =>
            {
                EvaluateRules(request.Event, response);
                CorrelateEvents(request.Event, response);
                CalculateSeverity(request.Event, response);
                DetectAlarms(request.Event, response);
            }, cancellationToken);

            _logger.LogInformation("Event processing completed: ResponseId={ResponseId}, IsIncident={IsIncident}, IsAlarm={IsAlarm}", 
                response.ResponseId, response.IsIncident, response.IsAlarm);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event: RequestId={RequestId}", request.RequestId);
            throw;
        }
    }

    private void EvaluateRules(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (telemetryEvent.EventType == "SmokeDetected" && telemetryEvent.SmokeLevel > 50)
        {
            response.IsIncident = true;
            response.IncidentType = "Fire";
            response.RuleTriggered = "HighSmokeLevel_Rule";
            response.ConfidenceScore = CalculateConfidence(telemetryEvent.SmokeLevel, 50, 100);
            
            _logger.LogWarning("Fire incident detected: DeviceId={DeviceId}, SmokeLevel={SmokeLevel}", 
                telemetryEvent.DeviceId, telemetryEvent.SmokeLevel);
        }
        else if (telemetryEvent.EventType == "HeatDetected" && telemetryEvent.Temperature > 60)
        {
            response.IsIncident = true;
            response.IncidentType = "Fire";
            response.RuleTriggered = "HighTemperature_Rule";
            response.ConfidenceScore = CalculateConfidence(telemetryEvent.Temperature, 60, 100);
            
            _logger.LogWarning("Fire incident detected: DeviceId={DeviceId}, Temperature={Temperature}", 
                telemetryEvent.DeviceId, telemetryEvent.Temperature);
        }
        else if (telemetryEvent.BatteryLevel < 20)
        {
            response.IsIncident = true;
            response.IncidentType = "LowBattery";
            response.RuleTriggered = "LowBattery_Rule";
            response.ConfidenceScore = 1.0;
            
            _logger.LogInformation("Low battery detected: DeviceId={DeviceId}, BatteryLevel={BatteryLevel}", 
                telemetryEvent.DeviceId, telemetryEvent.BatteryLevel);
        }
    }

    private void CorrelateEvents(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (response.IsIncident && response.IncidentType == "Fire")
        {
            response.AdditionalData["CorrelationScore"] = 0.85;
            response.AdditionalData["NearbyDevices"] = new List<string> { "DEV-002", "DEV-003" };
            
            _logger.LogInformation("Correlated fire incident with nearby devices: DeviceId={DeviceId}", 
                telemetryEvent.DeviceId);
        }
    }

    private void CalculateSeverity(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (!response.IsIncident)
        {
            response.Severity = "None";
            return;
        }

        if (response.IncidentType == "Fire")
        {
            if (telemetryEvent.SmokeLevel > 80 || telemetryEvent.Temperature > 80)
            {
                response.Severity = "Critical";
            }
            else if (telemetryEvent.SmokeLevel > 65 || telemetryEvent.Temperature > 70)
            {
                response.Severity = "High";
            }
            else
            {
                response.Severity = "Medium";
            }
        }
        else if (response.IncidentType == "LowBattery")
        {
            response.Severity = telemetryEvent.BatteryLevel < 10 ? "High" : "Low";
        }
        else
        {
            response.Severity = "Medium";
        }

        _logger.LogInformation("Severity calculated: DeviceId={DeviceId}, Severity={Severity}", 
            telemetryEvent.DeviceId, response.Severity);
    }

    private void DetectAlarms(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (response.IsIncident && (response.Severity == "High" || response.Severity == "Critical"))
        {
            response.IsAlarm = true;
            response.AdditionalData["AlarmPriority"] = response.Severity;
            response.AdditionalData["RequiresImmediateAction"] = response.Severity == "Critical";
            
            _logger.LogWarning("Alarm triggered: DeviceId={DeviceId}, Severity={Severity}, IncidentType={IncidentType}", 
                telemetryEvent.DeviceId, response.Severity, response.IncidentType);
        }
    }

    private double CalculateConfidence(double actualValue, double threshold, double maxValue)
    {
        if (actualValue <= threshold) return 0.0;
        
        var range = maxValue - threshold;
        var exceeds = Math.Min(actualValue - threshold, range);
        return Math.Min(exceeds / range, 1.0);
    }
}
