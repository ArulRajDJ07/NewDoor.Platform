using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
using System.Collections.Concurrent;

namespace NewDoor.Processor.Runtime.Services;

public interface IRuleConfigurationCache
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task RefreshAsync(CancellationToken cancellationToken = default);
    List<RuleConfigurationResponse> GetActiveRules();
    List<RuleConfigurationResponse> GetRulesByEventType(string eventType);
    RuleConfigurationResponse? GetRuleById(int id);
}

public class RuleConfigurationCache : IRuleConfigurationCache
{
    private readonly IRuleConfigurationClient _client;
    private readonly ILogger<RuleConfigurationCache> _logger;
    private readonly ConcurrentDictionary<int, RuleConfigurationResponse> _rulesById;
    private readonly ConcurrentDictionary<string, List<RuleConfigurationResponse>> _rulesByEventType;
    private List<RuleConfigurationResponse> _activeRules;
    private DateTime _lastRefreshUtc;
    private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

    public RuleConfigurationCache(IRuleConfigurationClient client, ILogger<RuleConfigurationCache> logger)
    {
        _client = client;
        _logger = logger;
        _rulesById = new ConcurrentDictionary<int, RuleConfigurationResponse>();
        _rulesByEventType = new ConcurrentDictionary<string, List<RuleConfigurationResponse>>();
        _activeRules = new List<RuleConfigurationResponse>();
        _lastRefreshUtc = DateTime.MinValue;
    }

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

            var rules = await _client.GetAllRulesAsync(null, cancellationToken);

            if (rules != null && rules.Any())
            {
                // Clear existing caches
                _rulesById.Clear();
                _rulesByEventType.Clear();

                // Populate caches
                foreach (var rule in rules)
                {
                    _rulesById[rule.Id] = rule;

                    if (!_rulesByEventType.ContainsKey(rule.EventType))
                    {
                        _rulesByEventType[rule.EventType] = new List<RuleConfigurationResponse>();
                    }
                    _rulesByEventType[rule.EventType].Add(rule);
                }

                // Update active rules (sorted by priority descending)
                _activeRules = rules
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.Priority)
                    .ToList();

                _lastRefreshUtc = DateTime.UtcNow;

                _logger.LogInformation(
                    "Successfully refreshed {TotalRules} rules ({ActiveRules} active, {EventTypes} event types)",
                    rules.Count,
                    _activeRules.Count,
                    _rulesByEventType.Count);
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

    public List<RuleConfigurationResponse> GetActiveRules()
    {
        return _activeRules.ToList();
    }

    public List<RuleConfigurationResponse> GetRulesByEventType(string eventType)
    {
        if (_rulesByEventType.TryGetValue(eventType, out var rules))
        {
            return rules.Where(r => r.IsActive).OrderByDescending(r => r.Priority).ToList();
        }
        return new List<RuleConfigurationResponse>();
    }

    public RuleConfigurationResponse? GetRuleById(int id)
    {
        _rulesById.TryGetValue(id, out var rule);
        return rule;
    }
}
