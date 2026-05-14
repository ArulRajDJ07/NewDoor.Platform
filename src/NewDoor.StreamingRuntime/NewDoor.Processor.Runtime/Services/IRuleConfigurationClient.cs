using NewDoor.Platform.DTO.Features.Rules.Models;

namespace NewDoor.Processor.Runtime.Services;

public interface IRuleConfigurationClient
{
    Task<List<RuleResponse>> GetAllRulesAsync(CancellationToken cancellationToken = default);
    Task<List<RuleResponse>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
    Task<List<RuleResponse>> GetRulesByDeviceTypeAsync(string deviceType, CancellationToken cancellationToken = default);
}

public class RuleConfigurationClient : IRuleConfigurationClient
{
    #region Fields
    private readonly HttpClient _httpClient;
    private readonly ILogger<RuleConfigurationClient> _logger;
    #endregion

    #region Constructor
    public RuleConfigurationClient(HttpClient httpClient, ILogger<RuleConfigurationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    public async Task<List<RuleResponse>> GetAllRulesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetAllRulesAsync called - Fetching all rules from API");
        try
        {
            var response = await _httpClient.GetAsync("api/rule/getall", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var rules = await response.Content.ReadFromJsonAsync<List<RuleResponse>>(cancellationToken);
                _logger.LogInformation("Successfully fetched {Count} rules from API", rules?.Count ?? 0);

                // Log the actual rule data for debugging
                if (rules != null && rules.Any())
                {
                    foreach (var rule in rules)
                    {
                        _logger.LogDebug("Rule loaded: ID={Id}, Type={RuleType}, DeviceType={DeviceType}, Threshold={Threshold}, Active={IsActive}", 
                            rule.Id, rule.RuleType, rule.DeviceType, rule.ThresholdValue, rule.IsActive);
                    }
                }

                return rules ?? new List<RuleResponse>();
            }
            else
            {
                _logger.LogWarning("Failed to fetch rules from API. Status: {StatusCode}", response.StatusCode);
                return new List<RuleResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rules from API");
            return new List<RuleResponse>();
        }
    }

    public async Task<List<RuleResponse>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        var allRules = await GetAllRulesAsync(cancellationToken);
        return allRules.Where(r => r.IsActive).ToList();
    }

    public async Task<List<RuleResponse>> GetRulesByDeviceTypeAsync(string deviceType, CancellationToken cancellationToken = default)
    {
        var allRules = await GetAllRulesAsync(cancellationToken);
        return allRules.Where(r => r.IsActive && r.DeviceType == deviceType).ToList();
    }
    #endregion
}
