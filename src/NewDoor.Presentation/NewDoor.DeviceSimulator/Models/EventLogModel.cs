namespace NewDoor.DeviceSimulator.Models;

public class EventLogModel
{
    public DateTime Timestamp { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
