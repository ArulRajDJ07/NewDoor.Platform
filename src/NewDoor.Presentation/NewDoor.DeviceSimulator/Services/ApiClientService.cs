using System.Net.Http.Json;
using NewDoor.DeviceSimulator.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;

namespace NewDoor.DeviceSimulator.Services;

public class ApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClientService> _logger;

    public ApiClientService(HttpClient httpClient, ILogger<ApiClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<BuildingResponse>> GetAllBuildingsAsync()
    {
        try
        {

            var response = await _httpClient.GetFromJsonAsync<List<BuildingResponse>>("/api/building/getall");
            return response ?? new List<BuildingResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load buildings");
            return new List<BuildingResponse>();
        }
    }

    public async Task<List<BuildingWithDevicesResponse>> GetAllBuildingsWithDevicesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BuildingWithDevicesResponse>>("/api/building/getallwithdevices");
            return response ?? new List<BuildingWithDevicesResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load buildings with devices");
            return new List<BuildingWithDevicesResponse>();
        }
    }

    public async Task<List<DeviceResponse>> GetAllDevicesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DeviceResponse>>("/api/device/getall");
            return response ?? new List<DeviceResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load devices");
            return new List<DeviceResponse>();
        }
    }

    public async Task<List<DeviceRuntimeStatusResponse>> GetAllDeviceRuntimeStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DeviceRuntimeStatusResponse>>("/api/deviceruntimestatus/getall");
            return response ?? new List<DeviceRuntimeStatusResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load device runtime status");
            return new List<DeviceRuntimeStatusResponse>();
        }
    }
}
