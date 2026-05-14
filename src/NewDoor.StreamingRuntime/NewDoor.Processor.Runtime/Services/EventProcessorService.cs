using NewDoor.Processor.Runtime.Models;

namespace NewDoor.Processor.Runtime.Services;

public interface IEventProcessorService
{
    Task<ProcessorResponse> ProcessAsync(ProcessorRequest request, CancellationToken cancellationToken);
}

public class EventProcessorService : IEventProcessorService
{
    #region Fields
    private readonly ILogger<EventProcessorService> _logger;
    private readonly IRuleConfigurationCache _ruleCache;
    private readonly IEventHistoryCache _eventHistory;
    #endregion

    #region Constructor
    public EventProcessorService(
        ILogger<EventProcessorService> logger, 
        IRuleConfigurationCache ruleCache,
        IEventHistoryCache eventHistory)
    {
        _logger = logger;
        _ruleCache = ruleCache;
        _eventHistory = eventHistory;
    }
    #endregion

    #region Public Methods
    public async Task<ProcessorResponse> ProcessAsync(ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing event {EventId}", request.Event.EventId);

            _eventHistory.AddEvent(request.Event);

            var response = new ProcessorResponse
            {
                RequestId = request.RequestId,
                CorrelationId = request.CorrelationId,
                ProcessedAtUtc = DateTime.UtcNow,
                EventType = "audit" // Default to audit; will be updated based on processing
            };

            await Task.Run(() =>
            {
                EvaluateRules(request.Event, response);
                CorrelateEvents(request.Event, response);
                CalculateSeverity(request.Event, response);
                DetectAlarms(request.Event, response);
                DetermineEventType(response); // Determine final event type
            }, cancellationToken);

            _logger.LogInformation("Event processed - Incident: {IsIncident}, Alarm: {IsAlarm}", response.IsIncident, response.IsAlarm);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event");
            throw;
        }
    }
    #endregion

    #region Rule Evaluation
    private void EvaluateRules(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        var rules = _ruleCache.GetRulesByDeviceType(telemetryEvent.DeviceType);

        if (!rules.Any())
        {
            var allRules = _ruleCache.GetActiveRules();
            _logger.LogWarning("No active rules found for device type: {DeviceType}. Total active rules in cache: {TotalRules}", 
                telemetryEvent.DeviceType, allRules.Count);

            if (allRules.Count == 0)
            {
                _logger.LogError("Rule cache is EMPTY! Cache may not have initialized properly. Check API connectivity.");
            }
            else
            {
                _logger.LogWarning("Available device types in cache: {DeviceTypes}", 
                    string.Join(", ", allRules.Select(r => r.DeviceType).Distinct()));
            }
            return;
        }

        _logger.LogDebug("Evaluating {RuleCount} rules for device type: {DeviceType}", rules.Count, telemetryEvent.DeviceType);

        bool anyRuleTriggered = false;
        foreach (var rule in rules)
        {
            if (EvaluateRule(telemetryEvent, rule))
            {
                anyRuleTriggered = true;
                _logger.LogDebug("Rule triggered: {RuleName} ({RuleType})", rule.RuleName, rule.RuleType);

                var propertyName = GetPropertyNameFromRuleType(rule.RuleType);
                var propertyValue = GetPropertyValue(telemetryEvent, propertyName);

                bool isSustained = _eventHistory.HasSustainedAnomaly(
                    telemetryEvent.DeviceId, 
                    propertyName, 
                    rule.ThresholdValue, 
                    requiredCount: 2);

                bool isMultiSensorConfirmed = false;
                if (propertyName == "SmokeLevel" || propertyName == "Temperature")
                {
                    isMultiSensorConfirmed = _eventHistory.HasMultiSensorAnomaly(telemetryEvent.DeviceId, windowSeconds: 60);
                }

                _logger.LogDebug("Anomaly check - Sustained: {IsSustained}, MultiSensor: {IsMultiSensor}", isSustained, isMultiSensorConfirmed);

                if (!isSustained && !isMultiSensorConfirmed)
                {
                    // For Critical severity, treat as incident immediately (hackathon quick fix)
                    if (rule.Severity == "Critical")
                    {
                        response.IsIncident = true;
                        response.IncidentType = rule.RuleType;
                        response.RuleTriggered = rule.RuleName;
                        response.Severity = rule.Severity;
                        response.ConfidenceScore = CalculateConfidence(propertyValue, rule.ThresholdValue, GetMaxValue(propertyName)) * 0.7; // Lower confidence for first detection
                        response.AdditionalData["UnconfirmedIncident"] = true;
                        response.AdditionalData["Reason"] = "First detection - not yet sustained";
                        _logger.LogInformation("Critical incident detected (unconfirmed) - Type: {IncidentType}, Confidence: {Confidence:F2}", 
                            response.IncidentType, response.ConfidenceScore);
                        break;
                    }

                    response.AdditionalData["SuppressedSpike"] = true;
                    response.AdditionalData["SuppressedReason"] = "Isolated anomaly - not sustained";
                    response.AdditionalData["RuleTriggered"] = rule.RuleName;
                    _logger.LogDebug("Anomaly suppressed - isolated event");
                    continue;
                }

                response.IsIncident = true;
                response.IncidentType = rule.RuleType;
                response.RuleTriggered = rule.RuleName;
                response.Severity = rule.Severity;
                response.ConfidenceScore = CalculateConfidence(propertyValue, rule.ThresholdValue, GetMaxValue(propertyName));

                if (isMultiSensorConfirmed)
                {
                    response.ConfidenceScore = Math.Min(response.ConfidenceScore * 1.2, 1.0);
                    response.AdditionalData["MultiSensorConfirmed"] = true;
                }

                response.AdditionalData["SustainedAnomaly"] = isSustained;

                _logger.LogInformation("Incident detected - Type: {IncidentType}, Severity: {Severity}, Confidence: {Confidence:F2}", 
                    response.IncidentType, response.Severity, response.ConfidenceScore);

                break;
            }
        }

        if (!anyRuleTriggered)
        {
            _logger.LogDebug("No rules triggered for device {DeviceId} - event values below thresholds", telemetryEvent.DeviceId);
        }
    }

    private bool EvaluateRule(RuntimeTelemetryEvent telemetryEvent, NewDoor.Platform.DTO.Features.Rules.Models.RuleResponse rule)
    {
        var propertyName = GetPropertyNameFromRuleType(rule.RuleType);
        var propertyValue = GetPropertyValue(telemetryEvent, propertyName);

        return propertyValue > rule.ThresholdValue;
    }

    private string GetPropertyNameFromRuleType(string ruleType)
    {
        return ruleType switch
        {
            "Fire" or "Smoke" or "SmokeDetection" => "SmokeLevel",
            "Heat" or "Temperature" or "HeatSpike" => "Temperature",
            "Battery" or "LowBattery" => "BatteryLevel",
            _ => "Temperature"
        };
    }
    #endregion

    #region Event Correlation
    private void CorrelateEvents(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (response.IsIncident && response.IncidentType == "Fire")
        {
            var recentEvents = _eventHistory.GetRecentEvents(telemetryEvent.DeviceId, windowSeconds: 300);

            var hasSmoke = recentEvents.Any(e => e.SmokeLevel > 50);
            var hasHeat = recentEvents.Any(e => e.Temperature > 70);

            if (hasSmoke && hasHeat)
            {
                response.Severity = "Critical";
                response.AdditionalData["CorrelationType"] = "Smoke+Heat";
                response.AdditionalData["CorrelationScore"] = 0.95;
            }
            else
            {
                response.AdditionalData["CorrelationScore"] = 0.65;
            }

            response.AdditionalData["NearbyDevices"] = new List<string> { "DEV-002", "DEV-003" };
        }
    }
    #endregion

    #region Severity & Alarm Detection
    private void CalculateSeverity(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (!response.IsIncident)
        {
            response.Severity = "None";
            return;
        }

        // If severity was already set by a rule, respect it
        if (!string.IsNullOrEmpty(response.Severity))
        {
            _logger.LogDebug("Severity already set by rule: {Severity}", response.Severity);
            return;
        }

        // Fallback severity calculation for rules that don't specify severity
        if (response.IncidentType == "Fire" || response.IncidentType == "SmokeDetection")
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
    }

    private void DetectAlarms(RuntimeTelemetryEvent telemetryEvent, ProcessorResponse response)
    {
        if (response.IsIncident && (response.Severity == "High" || response.Severity == "Critical"))
        {
            response.IsAlarm = true;
            response.AdditionalData["AlarmPriority"] = response.Severity;
            response.AdditionalData["RequiresImmediateAction"] = response.Severity == "Critical";
        }
    }
    #endregion

    #region Helper Methods
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

    private double CalculateConfidence(double actualValue, double threshold, double maxValue)
    {
        if (actualValue <= threshold) return 0.0;

        var range = maxValue - threshold;
        var exceeds = Math.Min(actualValue - threshold, range);
        return Math.Min(exceeds / range, 1.0);
    }
    #endregion

    #region Event Type Classification
    private void DetermineEventType(ProcessorResponse response)
    {
        if (response.IsAlarm)
        {
            response.EventType = "alarm";
        }
        else if (response.IsIncident)
        {
            response.EventType = "incident";
        }
        else if (response.AdditionalData.ContainsKey("SuppressedSpike"))
        {
            response.EventType = "notification";
        }
        else
        {
            response.EventType = "audit";
        }

        if (response.Severity == "Critical" && response.ConfidenceScore > 0.9)
        {
            response.EventType = "escalation";
            response.AdditionalData["EscalationReason"] = "Critical severity with high confidence";
        }
    }
    #endregion
}
