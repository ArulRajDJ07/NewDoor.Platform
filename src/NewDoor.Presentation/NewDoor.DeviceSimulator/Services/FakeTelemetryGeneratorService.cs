using NewDoor.DeviceSimulator.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;

namespace NewDoor.DeviceSimulator.Services;

public class FakeTelemetryGeneratorService
{
    private readonly Random _random = new();

    public DeviceTelemetryEvent GenerateTelemetry(
        DeviceResponse device, 
        BuildingWithDevicesResponse building, 
        string eventType = "Heartbeat")
    {
        var telemetry = new DeviceTelemetryEvent
        {
            EventId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            EventName = "DeviceTelemetryReceived",
            EventType = eventType,
            DeviceId = device.DeviceId,
            DeviceName = device.DeviceName,
            DeviceType = device.DeviceType,
            BuildingId = building.Id,
            BuildingCode = building.BuildingCode,
            Floor = device.Floor,
            Zone = device.Zone,
            TimestampUtc = DateTime.UtcNow,
            Payload = GeneratePayload(eventType),
            Metadata = new TelemetryMetadata
            {
                Source = "NewDoor.DeviceSimulator",
                GeneratedUtc = DateTime.UtcNow
            }
        };

        return telemetry;
    }

    private TelemetryPayload GeneratePayload(string eventType)
    {
        return eventType switch
        {
            "SmokeDetected" => new TelemetryPayload
            {
                Temperature = _random.Next(45, 80),
                SmokeLevel = _random.Next(60, 100),
                BatteryLevel = _random.Next(70, 100),
                SignalStrength = "Strong",
                Status = "Alarm"
            },
            "HeatSpike" => new TelemetryPayload
            {
                Temperature = _random.Next(50, 100),
                SmokeLevel = _random.Next(0, 30),
                BatteryLevel = _random.Next(70, 100),
                SignalStrength = "Strong",
                Status = "Warning"
            },
            "DeviceOffline" => new TelemetryPayload
            {
                Temperature = 0,
                SmokeLevel = 0,
                BatteryLevel = 0,
                SignalStrength = "None",
                Status = "Offline"
            },
            "DeviceFailure" => new TelemetryPayload
            {
                Temperature = 0,
                SmokeLevel = 0,
                BatteryLevel = _random.Next(0, 20),
                SignalStrength = "Weak",
                Status = "Failure"
            },
            _ => new TelemetryPayload
            {
                Temperature = _random.Next(20, 35),
                SmokeLevel = _random.Next(0, 10),
                BatteryLevel = _random.Next(70, 100),
                SignalStrength = GetRandomSignalStrength(),
                Status = "Online"
            }
        };
    }

    private string GetRandomSignalStrength()
    {
        var strengths = new[] { "Strong", "Good", "Fair" };
        return strengths[_random.Next(strengths.Length)];
    }
}
