using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Services;

public interface IProcessorClient
{
    Task<ProcessorResponse> ProcessEventAsync(ProcessorRequest request, CancellationToken cancellationToken);
}

public class ProcessorClient : IProcessorClient
{
    #region Fields
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessorClient> _logger;
    #endregion

    #region Constructor
    public ProcessorClient(HttpClient httpClient, IConfiguration configuration, ILogger<ProcessorClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Methods
    public async Task<ProcessorResponse> ProcessEventAsync(ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var processorUrl = _configuration["Processor:Url"] ?? "http://localhost:5003";
            var endpoint = $"{processorUrl}/api/processor/process";

            var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProcessorResponse>(cancellationToken);

            if (result == null)
                throw new InvalidOperationException("Processor returned null response");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Processor");
            throw;
        }
    }
    #endregion
}
