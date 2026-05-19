using Microsoft.Identity.Client;
using System.Net.Http.Headers;

public class ApimService
{
    private readonly HttpClient _httpClient;
    private readonly IConfidentialClientApplication _msalApp;
    private readonly string[] _scopes;

    public ApimService(IConfiguration configuration)
    {
        string tenantId = configuration["TenantId"];
        string clientId = configuration["CallerApiClientId"];
        string clientSecret = configuration["CallerApiClientSecret"];
        string apimApiClientId = configuration["ApimApiClientId"];

        _scopes = new[] { $"api://{apimApiClientId}/.default" };

        _msalApp = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .Build();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://lubeanalyst-apim.azure-api.net/")
        };
    }

    public async Task<string> GetUserListAsync()
    {
        // 1. Get Azure AD token
        var tokenResult = await _msalApp
            .AcquireTokenForClient(_scopes)
            .ExecuteAsync();

        // 2. Call APIM with Bearer token
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);

        var token = tokenResult.AccessToken;

        var response = await _httpClient.GetAsync(
            "ProcessLabTestFunction-LubeAnalyst/GetUserList");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
