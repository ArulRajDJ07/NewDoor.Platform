using NewDoor.Processor.Runtime.Services;

namespace NewDoor.Processor.Runtime.BackgroundServices;

public class EventHistoryCleaner : BackgroundService
{
    #region Fields
    private readonly IEventHistoryCache _eventHistory;
    private readonly ILogger<EventHistoryCleaner> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);
    private readonly int _retentionSeconds = 600;
    #endregion

    #region Constructor
    public EventHistoryCleaner(IEventHistoryCache eventHistory, ILogger<EventHistoryCleaner> logger)
    {
        _eventHistory = eventHistory;
        _logger = logger;
    }
    #endregion

    #region Background Service
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Event History Cleaner started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_cleanupInterval, stoppingToken);

                _eventHistory.Cleanup(_retentionSeconds);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopping cleaner");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup error");
            }
        }

        _logger.LogInformation("Event History Cleaner stopped");
    }
    #endregion
}
