using NewDoor.Platform.DTO.Features.Rules.Models;
using System.Collections.Concurrent;

namespace NewDoor.Processor.Runtime.Services;

public interface IRuleConfigurationCache
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task RefreshAsync(CancellationToken cancellationToken = default);
    List<RuleResponse> GetActiveRules();
    List<RuleResponse> GetRulesByDeviceType(string deviceType);
    RuleResponse? GetRuleById(int id);
}

public class RuleConfigurationCache : IRuleConfigurationCache
{
    #region Fields
    private readonly IRuleConfigurationClient _client;
    private readonly ILogger<RuleConfigurationCache> _logger;
    private readonly ConcurrentDictionary<int, RuleResponse> _rulesById;
    private readonly ConcurrentDictionary<string, List<RuleResponse>> _rulesByDeviceType;
    private List<RuleResponse> _activeRules;
    private DateTime _lastRefreshUtc;
    private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
    #endregion

    #region Constructor
    public RuleConfigurationCache(IRuleConfigurationClient client, ILogger<RuleConfigurationCache> logger)
    {
        _client = client;
        _logger = logger;
        _rulesById = new ConcurrentDictionary<int, RuleResponse>();
        _rulesByDeviceType = new ConcurrentDictionary<string, List<RuleResponse>>();
        _activeRules = new List<RuleResponse>();
        _lastRefreshUtc = DateTime.MinValue;
    }
    #endregion

    #region Public Methods
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing RuleConfigurationCache...");
        await RefreshAsync(cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Refreshing rule configurations from API...");

            var rules = await _client.GetAllRulesAsync(cancellationToken);

            if (rules != null && rules.Any())
            {
                _rulesById.Clear();
                _rulesByDeviceType.Clear();

                foreach (var rule in rules)
                {
                    _rulesById[rule.Id] = rule;

                    if (!_rulesByDeviceType.ContainsKey(rule.DeviceType))
                    {
                        _rulesByDeviceType[rule.DeviceType] = new List<RuleResponse>();
                    }
                    _rulesByDeviceType[rule.DeviceType].Add(rule);
                }

                _activeRules = rules
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.RuleType)
                    .ToList();

                _lastRefreshUtc = DateTime.UtcNow;

                _logger.LogInformation(
                    "Successfully refreshed {TotalRules} rules ({ActiveRules} active, {DeviceTypes} device types)",
                    rules.Count,
                    _activeRules.Count,
                    _rulesByDeviceType.Count);
            }
            else
            {
                _logger.LogWarning("No rules retrieved from API during refresh");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing rule configurations");
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public List<RuleResponse> GetActiveRules()
    {
        return _activeRules.ToList();
    }

    public List<RuleResponse> GetRulesByDeviceType(string deviceType)
    {
        if (_rulesByDeviceType.TryGetValue(deviceType, out var rules))
        {
            return rules.Where(r => r.IsActive).ToList();
        }
        return new List<RuleResponse>();
    }

    public RuleResponse? GetRuleById(int id)
    {
        _rulesById.TryGetValue(id, out var rule);
        return rule;
    }
    #endregion
}
