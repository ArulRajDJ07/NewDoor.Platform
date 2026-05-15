namespace NewDoor.Web.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private bool _isAuthenticated = false;
    private string? _phoneNumber;

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;
    }

    public event Action? OnAuthenticationStateChanged;

    public bool IsAuthenticated => _isAuthenticated;
    public string? PhoneNumber => _phoneNumber;

    public async Task<bool> LoginAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        try
        {
            // Simulate API call delay
            await Task.Delay(500);

            // For now, accept any non-empty phone number
            _isAuthenticated = true;
            _phoneNumber = phoneNumber;

            OnAuthenticationStateChanged?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for phone number: {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public void Logout()
    {
        _isAuthenticated = false;
        _phoneNumber = null;
        OnAuthenticationStateChanged?.Invoke();
    }
}
