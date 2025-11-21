// Services/AuthService.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IdentityModel.Client;
using OnlineTestingClient.Models;

namespace OnlineTestingClient.Services;

public class AuthService : IAuthService
{
    private const string IdentityUrl = "https://localhost:5001";

    // Returns access token string on success, null on failure
    public async Task<string?> LoginAsync(string login, string password)
    {
        using var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync(IdentityUrl);
        if (disco.IsError) return null;

        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "avalonia.client",
            ClientSecret = "secret",
            UserName = login,
            Password = password,
            Scope = "api1 openid profile role"
        });

        if (tokenResponse.IsError) return null;

        AppState.Token = tokenResponse.AccessToken;

        var userInfoResponse = await client.GetUserInfoAsync(new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token = tokenResponse.AccessToken
        });

        if (!userInfoResponse.IsError)
        {
            var sub = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (int.TryParse(sub, out var id)) AppState.CurrentUserId = id;
            AppState.CurrentUserName = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? login;
            AppState.CurrentUserRole = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }

        return tokenResponse.AccessToken;
    }
}