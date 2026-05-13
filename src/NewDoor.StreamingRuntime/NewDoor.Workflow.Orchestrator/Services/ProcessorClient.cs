using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Services;

public interface IProcessorClient
{
    Task<ProcessorResponse> ProcessEventAsync(ProcessorRequest request, CancellationToken cancellationToken);
}

public class ProcessorClient : IProcessorClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessorClient> _logger;

    public ProcessorClient(HttpClient httpClient, IConfiguration configuration, ILogger<ProcessorClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ProcessorResponse> ProcessEventAsync(ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var processorUrl = _configuration["Processor:Url"] ?? "http://localhost:5003";
            var endpoint = $"{processorUrl}/api/processor/process";

            _logger.LogInformation("Sending event to Processor: RequestId={RequestId}, EventId={EventId}", 
                request.RequestId, request.Event.EventId);

            var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProcessorResponse>(cancellationToken);
            
            if (result == null)
            {
                throw new InvalidOperationException("Processor returned null response");
            }

            _logger.LogInformation("Received response from Processor: ResponseId={ResponseId}, IsIncident={IsIncident}", 
                result.ResponseId, result.IsIncident);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Processor: RequestId={RequestId}", request.RequestId);
            throw;
        }
    }
}
