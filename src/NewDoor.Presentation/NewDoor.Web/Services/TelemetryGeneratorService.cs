using NewDoor.Web.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;

namespace NewDoor.Web.Services;

public class TelemetryGeneratorService
{
    private readonly Random _random = new();

    public DeviceTelemetryPayload GenerateTelemetry(
        DeviceResponse device, 
        BuildingWithDevicesResponse building, 
        string eventType = "Heartbeat")
    {
        var payload = new DeviceTelemetryPayload
        {
            DeviceId = device.DeviceId,
            EventType = eventType,
            TimestampUtc = DateTime.UtcNow,
            Status = GetStatusForEventType(eventType),
            BatteryLevel = GetBatteryLevel(eventType),
            Temperature = GetTemperature(eventType),
            SmokeLevel = GetSmokeLevel(eventType),
            SignalStrength = GetSignalStrength(eventType)
        };

        return payload;
    }

    private string GetStatusForEventType(string eventType)
    {
        return eventType switch
        {
            "SmokeDetected" => "Alarm",
            "HeatSpike" => "Warning",
            "DeviceOffline" => "Offline",
            "DeviceFailure" => "Failure",
            _ => "Online"
        };
    }

    private int GetBatteryLevel(string eventType)
    {
        return eventType switch
        {
            "DeviceOffline" => 0,
            "DeviceFailure" => _random.Next(0, 20),
            _ => _random.Next(70, 100)
        };
    }

    private double? GetTemperature(string eventType)
    {
        return eventType switch
        {
            "SmokeDetected" => _random.Next(45, 80),
            "HeatSpike" => _random.Next(50, 100),
            "DeviceOffline" => 0,
            "DeviceFailure" => 0,
            _ => _random.Next(20, 35)
        };
    }

    private int? GetSmokeLevel(string eventType)
    {
        return eventType switch
        {
            "SmokeDetected" => _random.Next(60, 100),
            "HeatSpike" => _random.Next(0, 30),
            "DeviceOffline" => 0,
            "DeviceFailure" => 0,
            _ => _random.Next(0, 10)
        };
    }

    private string? GetSignalStrength(string eventType)
    {
        return eventType switch
        {
            "DeviceOffline" => "None",
            "DeviceFailure" => "Weak",
            _ => GetRandomSignalStrength()
        };
    }

    private string GetRandomSignalStrength()
    {
        var strengths = new[] { "Strong", "Good", "Fair" };
        return strengths[_random.Next(strengths.Length)];
    }
}
