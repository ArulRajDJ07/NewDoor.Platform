using NewDoor.Processor.Runtime.Models;
using System.Collections.Concurrent;

namespace NewDoor.Processor.Runtime.Services;

public interface IEventHistoryCache
{
    void AddEvent(RuntimeTelemetryEvent telemetryEvent);
    List<RuntimeTelemetryEvent> GetRecentEvents(string deviceId, int windowSeconds = 300);
    bool HasSustainedAnomaly(string deviceId, string propertyName, double threshold, int requiredCount = 2);
    bool HasMultiSensorAnomaly(string deviceId, int windowSeconds = 60);
    void Cleanup(int retentionSeconds = 600);
}

public class EventHistoryCache : IEventHistoryCache
{
    #region Fields
    private readonly ConcurrentDictionary<string, List<RuntimeTelemetryEvent>> _eventsByDevice = new();
    private readonly ILogger<EventHistoryCache> _logger;
    private readonly int _maxEventsPerDevice = 100;
    #endregion

    #region Constructor
    public EventHistoryCache(ILogger<EventHistoryCache> logger)
    {
        _logger = logger;
    }
    #endregion

    #region Public Methods
    public void AddEvent(RuntimeTelemetryEvent telemetryEvent)
    {
        var deviceEvents = _eventsByDevice.GetOrAdd(telemetryEvent.DeviceId, _ => new List<RuntimeTelemetryEvent>());

        lock (deviceEvents)
        {
            deviceEvents.Add(telemetryEvent);

            if (deviceEvents.Count > _maxEventsPerDevice)
            {
                deviceEvents.RemoveRange(0, deviceEvents.Count - _maxEventsPerDevice);
            }
        }
    }

    public List<RuntimeTelemetryEvent> GetRecentEvents(string deviceId, int windowSeconds = 300)
    {
        if (!_eventsByDevice.TryGetValue(deviceId, out var deviceEvents))
            return new List<RuntimeTelemetryEvent>();

        var cutoffTime = DateTime.UtcNow.AddSeconds(-windowSeconds);

        lock (deviceEvents)
        {
            return deviceEvents
                .Where(e => e.TimestampUtc >= cutoffTime)
                .OrderBy(e => e.TimestampUtc)
                .ToList();
        }
    }

    public bool HasSustainedAnomaly(string deviceId, string propertyName, double threshold, int requiredCount = 2)
    {
        var recentEvents = GetRecentEvents(deviceId, windowSeconds: 120);

        if (recentEvents.Count < requiredCount)
            return false;

        var lastEvents = recentEvents.TakeLast(requiredCount).ToList();
        int consecutiveViolations = 0;

        foreach (var evt in lastEvents)
        {
            var value = GetPropertyValue(evt, propertyName);
            if (value > threshold)
            {
                consecutiveViolations++;
            }
            else
            {
                consecutiveViolations = 0;
            }
        }

        return consecutiveViolations >= requiredCount;
    }

    public bool HasMultiSensorAnomaly(string deviceId, int windowSeconds = 60)
    {
        var recentEvents = GetRecentEvents(deviceId, windowSeconds);

        if (recentEvents.Count == 0)
            return false;

        bool hasSmokeAnomaly = recentEvents.Any(e => e.SmokeLevel > 50);
        bool hasHeatAnomaly = recentEvents.Any(e => e.Temperature > 70);

        return hasSmokeAnomaly && hasHeatAnomaly;
    }

    public void Cleanup(int retentionSeconds = 600)
    {
        var cutoffTime = DateTime.UtcNow.AddSeconds(-retentionSeconds);
        int totalRemoved = 0;

        foreach (var kvp in _eventsByDevice)
        {
            var deviceEvents = kvp.Value;

            lock (deviceEvents)
            {
                int originalCount = deviceEvents.Count;
                deviceEvents.RemoveAll(e => e.TimestampUtc < cutoffTime);
                totalRemoved += originalCount - deviceEvents.Count;
            }
        }

        if (totalRemoved > 0)
        {
            _logger.LogDebug("Cleaned up {Count} old events from history cache", totalRemoved);
        }
    }
    #endregion

    #region Private Methods
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
    #endregion
}
