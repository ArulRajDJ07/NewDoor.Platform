using NewDoor.DeviceSimulator.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;

namespace NewDoor.DeviceSimulator.Services;

public class SimulationEngineService
{
    private readonly DeviceService _deviceService;
    private readonly TelemetryGeneratorService _telemetryGenerator;
    private readonly EventBufferService _eventBuffer;
    private readonly TelemetryClientService _telemetryClient;
    private readonly ILogger<SimulationEngineService> _logger;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _simulationTask;
    private bool _isRunning;
    private long _eventsGenerated;
    private DateTime _startTime;
    private double _currentEventsPerSecond;

    public bool IsRunning => _isRunning;
    public long EventsGenerated => _eventsGenerated;
    public double CurrentEventsPerSecond => _currentEventsPerSecond;

    public event Action? OnSimulationStateChanged;
    public event Action? OnMetricsUpdated;

    public SimulationEngineService(
        DeviceService deviceService,
        TelemetryGeneratorService telemetryGenerator,
        EventBufferService eventBuffer,
        TelemetryClientService telemetryClient,
        ILogger<SimulationEngineService> logger)
    {
        _deviceService = deviceService;
        _telemetryGenerator = telemetryGenerator;
        _eventBuffer = eventBuffer;
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task StartSimulationAsync(SimulationSettings settings)
    {
        if (_isRunning)
        {
            return;
        }

        _logger.LogInformation("Starting simulation: {EventsPerSecond} events/sec for {Duration} seconds", 
            settings.EventsPerSecond, settings.DurationSeconds);

        _cancellationTokenSource = new CancellationTokenSource();
        _eventsGenerated = 0;
        _startTime = DateTime.UtcNow;
        _isRunning = true;

        OnSimulationStateChanged?.Invoke();

        _simulationTask = Task.Run(() => RunSimulationAsync(settings, _cancellationTokenSource.Token));

        await Task.CompletedTask;
    }

    public async Task StopSimulationAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        
        if (_simulationTask != null)
        {
            await _simulationTask;
        }

        _isRunning = false;
        _currentEventsPerSecond = 0;

        OnSimulationStateChanged?.Invoke();
        OnMetricsUpdated?.Invoke();
    }

    public async Task TriggerEventAsync(int deviceId, string eventType)
    {
        var device = _deviceService.GetDevice(deviceId);
        if (device == null)
        {
            return;
        }

        var building = _deviceService.GetBuilding(device.BuildingId);
        if (building == null)
        {
            return;
        }

        var payload = _telemetryGenerator.GenerateTelemetry(device, building, eventType);
        await _telemetryClient.PublishTelemetryAsync(payload);

        var eventLog = new EventLogModel
        {
            Timestamp = DateTime.Now,
            BuildingName = building.Name,
            DeviceId = device.DeviceId,
            EventType = eventType,
            Status = payload.Status
        };

        _eventBuffer.AddEvent(eventLog);
        Interlocked.Increment(ref _eventsGenerated);
    }

    public async Task TriggerPeakLoadAsync()
    {
        if (_isRunning)
        {
            return;
        }

        var settings = new SimulationSettings
        {
            SelectedBuildingId = null,
            SelectedDeviceId = null,
            TrafficMode = TrafficMode.PeakLoad,
            EventsPerSecond = 50000,
            DurationSeconds = 60
        };

        _logger.LogInformation("Starting peak load test: 50,000 events/sec for 60 seconds");
        await StartSimulationAsync(settings);
    }

    private async Task RunSimulationAsync(SimulationSettings settings, CancellationToken cancellationToken)
    {
        try
        {
            var endTime = DateTime.UtcNow.AddSeconds(settings.DurationSeconds);
            var eventsSinceLastCheck = 0;
            var lastCheckTime = DateTime.UtcNow;

            if (settings.EventsPerSecond > 100)
            {
                var batchSize = Math.Min(100, settings.EventsPerSecond / 10);
                var batchIntervalMs = (batchSize * 1000.0) / settings.EventsPerSecond;

                while (DateTime.UtcNow < endTime && !cancellationToken.IsCancellationRequested)
                {
                    for (int i = 0; i < batchSize && DateTime.UtcNow < endTime; i++)
                    {
                        await GenerateEventAsync(settings);
                        eventsSinceLastCheck++;
                    }

                    var now = DateTime.UtcNow;
                    if ((now - lastCheckTime).TotalSeconds >= 1.0)
                    {
                        _currentEventsPerSecond = eventsSinceLastCheck / (now - lastCheckTime).TotalSeconds;
                        eventsSinceLastCheck = 0;
                        lastCheckTime = now;
                        OnMetricsUpdated?.Invoke();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(batchIntervalMs), cancellationToken);
                }
            }
            else
            {
                var intervalMs = 1000.0 / settings.EventsPerSecond;
                using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));

                while (DateTime.UtcNow < endTime && !cancellationToken.IsCancellationRequested)
                {
                    await timer.WaitForNextTickAsync(cancellationToken);
                    await GenerateEventAsync(settings);
                    eventsSinceLastCheck++;

                    var now = DateTime.UtcNow;
                    if ((now - lastCheckTime).TotalSeconds >= 1.0)
                    {
                        _currentEventsPerSecond = eventsSinceLastCheck / (now - lastCheckTime).TotalSeconds;
                        eventsSinceLastCheck = 0;
                        lastCheckTime = now;
                        OnMetricsUpdated?.Invoke();
                    }
                }
            }

            _logger.LogInformation("Simulation completed: {EventCount} events", _eventsGenerated);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Simulation stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Simulation error occurred");
        }
        finally
        {
            _isRunning = false;
            _currentEventsPerSecond = 0;
            OnSimulationStateChanged?.Invoke();
            OnMetricsUpdated?.Invoke();
        }
    }

    private async Task GenerateEventAsync(SimulationSettings settings)
    {
        var devices = settings.SelectedDeviceId.HasValue
            ? new List<DeviceResponse?> { _deviceService.GetDevice(settings.SelectedDeviceId.Value) }
            : settings.SelectedBuildingId.HasValue
                ? _deviceService.GetDevicesByBuilding(settings.SelectedBuildingId.Value).Cast<DeviceResponse?>().ToList()
                : _deviceService.Devices.Cast<DeviceResponse?>().ToList();

        if (!devices.Any() || devices.All(d => d == null))
        {
            return;
        }

        var device = devices.Where(d => d != null).OrderBy(_ => Random.Shared.Next()).FirstOrDefault();
        if (device == null) 
        {
            return;
        }

        var building = _deviceService.GetBuilding(device.BuildingId);
        if (building == null) 
        {
            return;
        }

        string? eventType = null;
        if (settings.TrafficMode == TrafficMode.Burst)
        {
            var randomValue = Random.Shared.Next(100);
            if (randomValue < 40)
            {
                var alertTypes = new[] { "SmokeDetected", "HeatSpike", "DeviceOffline", "LowBattery" };
                eventType = alertTypes[Random.Shared.Next(alertTypes.Length)];
            }
        }

        var payload = eventType != null 
            ? _telemetryGenerator.GenerateTelemetry(device, building, eventType)
            : _telemetryGenerator.GenerateTelemetry(device, building);

        _ = Task.Run(async () => 
        {
            try 
            { 
                await _telemetryClient.PublishTelemetryAsync(payload); 
            } 
            catch 
            { 
                // Errors already logged in TelemetryClientService
            }
        });

        var eventLog = new EventLogModel
        {
            Timestamp = DateTime.Now,
            BuildingName = building.Name,
            DeviceId = device.DeviceId,
            EventType = eventType ?? payload.EventType,
            Status = payload.Status
        };

        _eventBuffer.AddEvent(eventLog);
        Interlocked.Increment(ref _eventsGenerated);
    }
}
