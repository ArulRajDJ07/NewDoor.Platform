using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Services;

public interface IActionDispatcherClient
{
    Task<ActionDispatchResponse> DispatchActionAsync(ActionDispatchRequest request, CancellationToken cancellationToken);
}

public class ActionDispatcherClient : IActionDispatcherClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ActionDispatcherClient> _logger;

    public ActionDispatcherClient(HttpClient httpClient, IConfiguration configuration, ILogger<ActionDispatcherClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ActionDispatchResponse> DispatchActionAsync(ActionDispatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var actionDispatcherUrl = _configuration["ActionDispatcher:Url"] ?? "http://localhost:5004";
            var endpoint = $"{actionDispatcherUrl}/api/actions/dispatch";

            _logger.LogInformation("Dispatching action: ActionId={ActionId}, ActionType={ActionType}, CorrelationId={CorrelationId}", 
                request.ActionId, request.ActionType, request.CorrelationId);

            var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ActionDispatchResponse>(cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Action Dispatcher returned null response");
            }

            _logger.LogInformation("Action dispatched successfully: DispatchId={DispatchId}, Status={Status}", 
                result.DispatchId, result.Status);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching action: ActionId={ActionId}", request.ActionId);
            throw;
        }
    }
}
