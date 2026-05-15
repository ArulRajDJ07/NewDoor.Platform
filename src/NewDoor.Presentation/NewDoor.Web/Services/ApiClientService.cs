using System.Net.Http.Json;
using NewDoor.Web.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
using NewDoor.Platform.DTO.Features.Alarms.Models;
using NewDoor.Platform.DTO.Features.Incidents.Models;
using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
using NewDoor.Platform.DTO.Features.Events.Models;
using NewDoor.Platform.DTO.Features.Rules.Models;

namespace NewDoor.Web.Services;

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

    public async Task<List<AlarmResponse>> GetAllAlarmsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AlarmResponse>>("/api/alarm/getall");
            return response ?? new List<AlarmResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load alarms");
            return new List<AlarmResponse>();
        }
    }

    public async Task<List<IncidentResponse>> GetAllIncidentsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<IncidentResponse>>("/api/incident/getall");
            return response ?? new List<IncidentResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load incidents");
            return new List<IncidentResponse>();
        }
    }

    public async Task<List<EventsHistoryResponse>> GetAllEventsHistoryAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<EventsHistoryResponse>>("/api/eventshistory/getall");
            return response ?? new List<EventsHistoryResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load events history");
            return new List<EventsHistoryResponse>();
        }
    }

    public async Task<List<EventResponse>> GetAllEventsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<EventResponse>>("/api/event/getall");
            return response ?? new List<EventResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load events");
            return new List<EventResponse>();
        }
    }

    public async Task<List<RuleResponse>> GetAllRulesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<RuleResponse>>("/api/rule/getall");
            return response ?? new List<RuleResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load rules");
            return new List<RuleResponse>();
        }
    }
}
