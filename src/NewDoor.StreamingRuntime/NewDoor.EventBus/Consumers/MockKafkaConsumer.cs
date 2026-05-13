using Microsoft.Extensions.Logging;

namespace NewDoor.EventBus.Consumers;

public class MockKafkaConsumer<T> : IKafkaConsumer, IAsyncDisposable
{
    private readonly ILogger<MockKafkaConsumer<T>> _logger;
    private CancellationTokenSource? _cts;
    private Task? _consumeTask;

    public MockKafkaConsumer(ILogger<MockKafkaConsumer<T>> logger)
    {
        _logger = logger;
        _logger.LogWarning("MockKafkaConsumer initialized - Running on ARM64, no actual Kafka connection");
    }

    public Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _logger.LogWarning("MockKafkaConsumer: Simulating subscription to topic '{Topic}' (ARM64 mock - no actual messages will be consumed)", topic);
        
        _consumeTask = Task.Run(async () => await SimulateConsumeLoopAsync(topic, _cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

    private async Task SimulateConsumeLoopAsync(string topic, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("MockKafkaConsumer: Started simulated consume loop for topic '{Topic}'", topic);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                _logger.LogDebug("MockKafkaConsumer: Still running (no actual Kafka messages on ARM64)");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("MockKafkaConsumer: Consumer cancelled");
        }
    }

    public async Task StopConsumingAsync()
    {
        _logger.LogInformation("MockKafkaConsumer: Stopping consumer");
        _cts?.Cancel();
        
        if (_consumeTask != null)
        {
            await _consumeTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopConsumingAsync();
        _cts?.Dispose();
    }
}
