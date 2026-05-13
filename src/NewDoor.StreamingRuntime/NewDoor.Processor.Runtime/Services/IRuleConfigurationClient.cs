using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

namespace NewDoor.Processor.Runtime.Services;

public interface IRuleConfigurationClient
{
    Task<List<RuleConfigurationResponse>> GetAllRulesAsync(RuleConfigurationFilterRequest? filter = null, CancellationToken cancellationToken = default);
    Task<List<RuleConfigurationResponse>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
    Task<List<RuleConfigurationResponse>> GetRulesByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
}

public class RuleConfigurationClient : IRuleConfigurationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RuleConfigurationClient> _logger;

    public RuleConfigurationClient(HttpClient httpClient, ILogger<RuleConfigurationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<RuleConfigurationResponse>> GetAllRulesAsync(RuleConfigurationFilterRequest? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = BuildQueryString(filter);
            var response = await _httpClient.GetAsync($"api/ruleconfiguration/getall{queryParams}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var rules = await response.Content.ReadFromJsonAsync<List<RuleConfigurationResponse>>(cancellationToken);
                _logger.LogInformation("Successfully fetched {Count} rules from API", rules?.Count ?? 0);
                return rules ?? new List<RuleConfigurationResponse>();
            }
            else
            {
                _logger.LogWarning("Failed to fetch rules from API. Status: {StatusCode}", response.StatusCode);
                return new List<RuleConfigurationResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rules from API");
            return new List<RuleConfigurationResponse>();
        }
    }

    public async Task<List<RuleConfigurationResponse>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        var filter = new RuleConfigurationFilterRequest { IsActive = true };
        return await GetAllRulesAsync(filter, cancellationToken);
    }

    public async Task<List<RuleConfigurationResponse>> GetRulesByEventTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        var filter = new RuleConfigurationFilterRequest 
        { 
            EventType = eventType,
            IsActive = true 
        };
        return await GetAllRulesAsync(filter, cancellationToken);
    }

    private string BuildQueryString(RuleConfigurationFilterRequest? filter)
    {
        if (filter == null)
            return string.Empty;

        var queryParams = new List<string>();

        if (filter.Id.HasValue)
            queryParams.Add($"filter.Id={filter.Id.Value}");

        if (filter.IsActive.HasValue)
            queryParams.Add($"filter.IsActive={filter.IsActive.Value}");

        if (!string.IsNullOrWhiteSpace(filter.EventType))
            queryParams.Add($"filter.EventType={Uri.EscapeDataString(filter.EventType)}");

        if (!string.IsNullOrWhiteSpace(filter.IncidentType))
            queryParams.Add($"filter.IncidentType={Uri.EscapeDataString(filter.IncidentType)}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}
