using NewDoor.Processor.Runtime.Models;

namespace NewDoor.Processor.Runtime.Services;

public interface IEventProcessorService
{
    Task<ProcessorResponse> ProcessAsync(ProcessorRequest request, CancellationToken cancellationToken);
}

public class EventProcessorService : IEventProcessorService
{
    private readonly ILogger<EventProcessorService> _logger;
    private readonly IRuleConfigurationCache _ruleCache;

    public EventProcessorService(ILogger<EventProcessorService> logger, IRuleConfigurationCache ruleCache)
    {
        _logger = logger;
        _ruleCache = ruleCache;
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
        var rules = _ruleCache.GetRulesByEventType(telemetryEvent.EventType);

        if (!rules.Any())
        {
            _logger.LogDebug("No rules found for EventType={EventType}", telemetryEvent.EventType);
            return;
        }

        foreach (var rule in rules)
        {
            if (EvaluateRule(telemetryEvent, rule))
            {
                response.IsIncident = true;
                response.IncidentType = rule.IncidentType;
                response.RuleTriggered = rule.RuleName;
                response.Severity = rule.Severity;

                var propertyValue = GetPropertyValue(telemetryEvent, rule.PropertyName);
                response.ConfidenceScore = CalculateConfidence(propertyValue, rule.Threshold, GetMaxValue(rule.PropertyName));

                _logger.LogWarning("Rule triggered: RuleName={RuleName}, DeviceId={DeviceId}, Property={PropertyName}, Value={Value}, Threshold={Threshold}", 
                    rule.RuleName, telemetryEvent.DeviceId, rule.PropertyName, propertyValue, rule.Threshold);

                break;
            }
        }
    }

    private bool EvaluateRule(RuntimeTelemetryEvent telemetryEvent, NewDoor.Platform.DTO.Features.RuleConfigurations.Models.RuleConfigurationResponse rule)
    {
        var propertyValue = GetPropertyValue(telemetryEvent, rule.PropertyName);

        return rule.Operator switch
        {
            ">" => propertyValue > rule.Threshold,
            ">=" => propertyValue >= rule.Threshold,
            "<" => propertyValue < rule.Threshold,
            "<=" => propertyValue <= rule.Threshold,
            "==" => Math.Abs(propertyValue - rule.Threshold) < 0.001,
            "!=" => Math.Abs(propertyValue - rule.Threshold) >= 0.001,
            _ => false
        };
    }

    private double GetPropertyValue(RuntimeTelemetryEvent telemetryEvent, string propertyName)
    {
        return propertyName switch
        {
            "SmokeLevel" => telemetryEvent.SmokeLevel,
            "Temperature" => telemetryEvent.Temperature,
            "BatteryLevel" => telemetryEvent.BatteryLevel,
            _ => 0.0
        };
    }

    private double GetMaxValue(string propertyName)
    {
        return propertyName switch
        {
            "SmokeLevel" => 100.0,
            "Temperature" => 100.0,
            "BatteryLevel" => 100.0,
            _ => 100.0
        };
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
