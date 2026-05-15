using Microsoft.AspNetCore.SignalR.Client;
using NewDoor.Web.Models;
using System.Text.Json;

namespace NewDoor.Web.Services;

public class SignalRService : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SignalRService> _logger;
    private HubConnection? _hubConnection;
    private bool _isStarted = false;
    private int _reconnectAttempts = 0;
    private const int MaxReconnectAttempts = 10;

    public SignalRService(IConfiguration configuration, ILogger<SignalRService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    #region Events

    public event EventHandler<IncidentNotification>? OnIncidentReceived;
    public event EventHandler<AlarmNotification>? OnAlarmReceived;
    public event EventHandler<AuditHistoryNotification>? OnAuditHistoryReceived;
    public event EventHandler<ConnectionStateChangedEventArgs>? OnConnectionStateChanged;

    #endregion

    #region Properties

    public ConnectionState CurrentState { get; private set; } = ConnectionState.Disconnected;
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    #endregion

    #region Public Methods

    public async Task StartAsync()
    {
        if (_isStarted)
        {
            return;
        }

        try
        {
            var apiBaseUrl = _configuration["Api:BaseUrl"] ?? "https://newdoor-api.azurewebsites.net";
            var hubUrl = $"{apiBaseUrl}/notificationHub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.SkipNegotiation = false;
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Use original property names
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                })
                .Build();

            // Register event handlers
            RegisterHubHandlers();

            // Handle reconnection events
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;
            _hubConnection.Closed += OnClosed;

            // Start connection
            UpdateConnectionState(ConnectionState.Connecting, "Connecting to notification hub...");
            await _hubConnection.StartAsync();

            _isStarted = true;
            _reconnectAttempts = 0;
            UpdateConnectionState(ConnectionState.Connected, "Connected to notification hub");

            _logger.LogInformation("SignalR connected successfully to {HubUrl}", hubUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
            UpdateConnectionState(ConnectionState.Failed, $"Connection failed: {ex.Message}");

            // Retry connection after delay
            await Task.Delay(TimeSpan.FromSeconds(5));
            if (_reconnectAttempts < MaxReconnectAttempts)
            {
                _reconnectAttempts++;
                await StartAsync();
            }
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.StopAsync();
                UpdateConnectionState(ConnectionState.Disconnected, "Disconnected from notification hub");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
            }
        }

        _isStarted = false;
    }

    public async Task SubscribeToBuildingAsync(int buildingId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("SubscribeToBuilding", buildingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to building {BuildingId}", buildingId);
            }
        }
    }

    public async Task UnsubscribeFromBuildingAsync(int buildingId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("UnsubscribeFromBuilding", buildingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe from building {BuildingId}", buildingId);
            }
        }
    }

    #endregion

    #region Private Methods

    private void RegisterHubHandlers()
    {
        if (_hubConnection == null) return;

        // Handle generic alert notifications (primary handler)
        _hubConnection.On<DashboardAlert>("ReceiveAlert", (alert) =>
        {
            try
            {
                _logger.LogInformation("ReceiveAlert event triggered - DeviceId: {DeviceId}, Message: {Message}", alert?.DeviceId, alert?.Message);

                if (alert != null)
                {
                    _logger.LogInformation("Alert received: {DeviceId} - {Message}", alert.DeviceId, alert.Message);

                    // Route to alarm handler
                    var notification = new AlarmNotification
                    {
                        Alert = alert,
                        ReceivedAt = DateTime.UtcNow,
                        IsNew = true
                    };

                    OnAlarmReceived?.Invoke(this, notification);
                    _logger.LogInformation("OnAlarmReceived event invoked");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alert notification");
            }
        });

        // Handle incident notifications
        _hubConnection.On<DashboardAlert>("ReceiveIncident", (alert) =>
        {
            try
            {
                _logger.LogInformation("ReceiveIncident event triggered - DeviceId: {DeviceId}, Message: {Message}", alert?.DeviceId, alert?.Message);

                if (alert != null)
                {
                    var notification = new IncidentNotification
                    {
                        Alert = alert,
                        ReceivedAt = DateTime.UtcNow,
                        IsNew = true
                    };

                    OnIncidentReceived?.Invoke(this, notification);
                    _logger.LogInformation("OnIncidentReceived event invoked successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing incident notification");
            }
        });

        // Handle alarm notifications
        _hubConnection.On<DashboardAlert>("ReceiveAlarm", (alert) =>
        {
            try
            {
                _logger.LogInformation("ReceiveAlarm event triggered - DeviceId: {DeviceId}, Message: {Message}", alert?.DeviceId, alert?.Message);

                if (alert != null)
                {
                    var notification = new AlarmNotification
                    {
                        Alert = alert,
                        ReceivedAt = DateTime.UtcNow,
                        IsNew = true
                    };

                    OnAlarmReceived?.Invoke(this, notification);
                    _logger.LogInformation("OnAlarmReceived event invoked successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alarm notification");
            }
        });

        // Handle audit history notifications
        _hubConnection.On<AuditHistoryNotification>("ReceiveAuditHistory", (audit) =>
        {
            try
            {
                _logger.LogInformation("ReceiveAuditHistory event triggered - AuditId: {AuditId}", audit?.AuditId);

                if (audit != null)
                {
                    audit.ReceivedAt = DateTime.UtcNow;
                    audit.IsNew = true;

                    OnAuditHistoryReceived?.Invoke(this, audit);
                    _logger.LogInformation("OnAuditHistoryReceived event invoked successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing audit history notification");
            }
        });
    }

    private Task OnReconnecting(Exception? exception)
    {
        _logger.LogWarning(exception, "SignalR connection lost, attempting to reconnect...");
        UpdateConnectionState(ConnectionState.Reconnecting, "Reconnecting...");
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        _reconnectAttempts = 0;
        UpdateConnectionState(ConnectionState.Connected, "Reconnected to notification hub");
        return Task.CompletedTask;
    }

    private async Task OnClosed(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "SignalR connection closed unexpectedly");
            UpdateConnectionState(ConnectionState.Disconnected, $"Connection closed: {exception.Message}");

            // Attempt to reconnect if not intentionally stopped
            if (_isStarted && _reconnectAttempts < MaxReconnectAttempts)
            {
                _reconnectAttempts++;
                await Task.Delay(TimeSpan.FromSeconds(5 * _reconnectAttempts));
                await StartAsync();
            }
        }
        else
        {
            UpdateConnectionState(ConnectionState.Disconnected, "Connection closed");
        }
    }

    private void UpdateConnectionState(ConnectionState state, string? message = null)
    {
        CurrentState = state;
        OnConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
        {
            State = state,
            Message = message
        });
    }

    #endregion

    #region Disposal

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }

        _isStarted = false;
    }

    #endregion
}
