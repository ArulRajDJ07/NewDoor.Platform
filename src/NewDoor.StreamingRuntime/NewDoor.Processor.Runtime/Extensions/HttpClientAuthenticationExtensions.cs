namespace NewDoor.Processor.Runtime.Extensions;

/// <summary>
/// Extension methods for adding authentication to HttpClient
/// This is a placeholder for future authentication implementation
/// </summary>
public static class HttpClientAuthenticationExtensions
{
    /// <summary>
    /// Adds authentication to the HttpClient builder (placeholder for future extension)
    /// Currently: No authentication (internal service-to-service calls)
    /// Future options:
    ///   - API Key authentication for Azure service-to-service
    ///   - JWT Bearer tokens for OAuth/OIDC
    ///   - Managed Identity for Azure resources
    /// </summary>
    /// <example>
    /// // Future usage example:
    /// builder.Services.AddHttpClient&lt;RuleConfigurationClient&gt;(...)
    ///     .AddApiAuthentication(builder.Configuration); // Enable when needed
    /// </example>
    public static IHttpClientBuilder AddApiAuthentication(
        this IHttpClientBuilder builder, 
        IConfiguration configuration)
    {
        // TODO: Implement authentication based on configuration
        // Example implementation for API Key:
        /*
        var authConfig = configuration.GetSection("ApiAuthentication");
        var authType = authConfig["Type"]; // e.g., "ApiKey", "Bearer", "None"

        if (authType == "ApiKey")
        {
            var apiKey = authConfig["ApiKey"];
            builder.AddHttpMessageHandler(() => new ApiKeyAuthHandler(apiKey));
        }
        */

        // Currently returns builder unchanged (no auth)
        return builder;
    }

    /// <summary>
    /// Adds API key authentication header to all requests (future implementation)
    /// </summary>
    public static IHttpClientBuilder AddApiKeyAuthentication(
        this IHttpClientBuilder builder,
        string apiKey,
        string headerName = "X-API-Key")
    {
        // TODO: Implement DelegatingHandler for API Key
        // builder.AddHttpMessageHandler(() => new ApiKeyAuthenticationHandler(apiKey, headerName));

        return builder;
    }

    /// <summary>
    /// Adds JWT Bearer token authentication (future implementation)
    /// </summary>
    public static IHttpClientBuilder AddBearerTokenAuthentication(
        this IHttpClientBuilder builder,
        Func<Task<string>> tokenProvider)
    {
        // TODO: Implement DelegatingHandler for JWT Bearer
        // builder.AddHttpMessageHandler(() => new BearerTokenAuthenticationHandler(tokenProvider));

        return builder;
    }
}

/* 
 * FUTURE IMPLEMENTATION GUIDE
 * ===========================
 * 
 * 1. CREATE AUTHENTICATION HANDLER (DelegatingHandler)
 * ----------------------------------------------------
 * public class ApiKeyAuthenticationHandler : DelegatingHandler
 * {
 *     private readonly string _apiKey;
 *     private readonly string _headerName;
 *     
 *     public ApiKeyAuthenticationHandler(string apiKey, string headerName = "X-API-Key")
 *     {
 *         _apiKey = apiKey;
 *         _headerName = headerName;
 *     }
 *     
 *     protected override async Task<HttpResponseMessage> SendAsync(
 *         HttpRequestMessage request, 
 *         CancellationToken cancellationToken)
 *     {
 *         request.Headers.Add(_headerName, _apiKey);
 *         return await base.SendAsync(request, cancellationToken);
 *     }
 * }
 * 
 * 2. UPDATE CONFIGURATION (appsettings.json)
 * ------------------------------------------
 * "ApiAuthentication": {
 *   "Type": "ApiKey",  // "None", "ApiKey", "Bearer"
 *   "ApiKey": "",      // Store in Azure Key Vault or User Secrets
 *   "HeaderName": "X-API-Key"
 * }
 * 
 * 3. UPDATE PROGRAM.CS
 * --------------------
 * builder.Services.AddHttpClient<RuleConfigurationClient>(...)
 *     .AddApiAuthentication(builder.Configuration);  // Uncomment when ready
 * 
 * 4. API SIDE CHANGES (NewDoor.API)
 * ---------------------------------
 * - Add API key validation middleware or filter
 * - Or add [Authorize] attribute with API key scheme
 * 
 */
