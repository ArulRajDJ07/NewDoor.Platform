using NewDoor.Web.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;

namespace NewDoor.Web.Services;

public class DeviceService
{
    private readonly ApiClientService _apiClient;
    private readonly ILogger<DeviceService> _logger;

    public List<BuildingWithDevicesResponse> Buildings { get; private set; } = new();
    public List<DeviceResponse> Devices { get; private set; } = new();
    public List<DeviceRuntimeStatusResponse> RuntimeStatuses { get; private set; } = new();

    public event Action? OnDataLoaded;

    public DeviceService(ApiClientService apiClient, ILogger<DeviceService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            Buildings = await _apiClient.GetAllBuildingsWithDevicesAsync();
            Devices = await _apiClient.GetAllDevicesAsync();
            RuntimeStatuses = await _apiClient.GetAllDeviceRuntimeStatusAsync();

            OnDataLoaded?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load data");
        }
    }

    public BuildingWithDevicesResponse? GetBuilding(int buildingId)
    {
        return Buildings.FirstOrDefault(b => b.Id == buildingId);
    }

    public DeviceResponse? GetDevice(int deviceId)
    {
        return Devices.FirstOrDefault(d => d.Id == deviceId);
    }

    public List<DeviceResponse> GetDevicesByBuilding(int buildingId)
    {
        return Devices.Where(d => d.BuildingId == buildingId).ToList();
    }

    public int GetTotalOnlineDevices()
    {
        return Devices.Count(d => d.Status.Equals("Online", StringComparison.OrdinalIgnoreCase));
    }

    public int GetTotalOfflineDevices()
    {
        return Devices.Count(d => d.Status.Equals("Offline", StringComparison.OrdinalIgnoreCase));
    }

    public int GetTotalActiveAlerts()
    {
        return Buildings.Sum(b => b.ActiveAlarms);
    }
}
