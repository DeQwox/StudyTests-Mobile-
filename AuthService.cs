using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace OnlineTestingClient;

public class AuthService()
{
    private readonly HttpClient _httpClient = new();
    public string AccessToken { get; private set; } = "";

    public async Task<bool> LoginAsync(string username, string password)
    {
        // Discover endpoints from metadata
        var disco = await _httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
        if (disco.IsError) throw new Exception(disco.Error);

        // Request token using Resource Owner Password flow
        var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "avalonia_client",
            ClientSecret = "secret",
            Scope = "api1 openid profile",
            UserName = username,
            Password = password
        });
        if (tokenResponse.IsError) return false;

        AccessToken = tokenResponse.AccessToken;
        return true;
    }
}
