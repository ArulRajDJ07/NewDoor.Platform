namespace NewDoor.DeviceSimulator.Models;

public class SimulationSettings
{
    public int? SelectedBuildingId { get; set; }
    public int? SelectedDeviceId { get; set; }
    public TrafficMode TrafficMode { get; set; } = TrafficMode.Normal;
    public int EventsPerSecond { get; set; } = 10;
    public int DurationSeconds { get; set; } = 60;
}

public enum TrafficMode
{
    Normal,
    Burst,
    Heavy,
    Extreme,
    PeakLoad
}
