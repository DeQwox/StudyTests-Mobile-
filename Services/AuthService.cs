// Services/AuthService.cs
using System.Net.Http.Json;
using OnlineTestingClient.Models;
using System.Text.Json;
using System.Text;

namespace OnlineTestingClient.Services;

public class AuthService : IAuthService
{
    private const string BaseUrl = "https://localhost:5001";

    public async Task<string?> LoginAsync(string login, string password)
    {
        Console.WriteLine($"[AuthService] LoginAsync called for {login}");
        
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        
        using var client = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

        try 
        {
            // Simple API Login
            var payload = new { login = login, password = password };
            var response = await client.PostAsJsonAsync("api/auth/login", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    Console.WriteLine($"[AuthService] Login success. Role: {result.Role}");
                    AppState.Token = result.AccessToken;
                    AppState.CurrentUserRole = result.Role;
                    AppState.CurrentUserName = login;

                    // Try to extract UserID from token if it's a JWT
                    var (sub, _, _) = ParseJwt(result.AccessToken);
                    if (int.TryParse(sub, out int uid))
                    {
                        AppState.CurrentUserId = uid;
                        Console.WriteLine($"[AuthService] UserID from token: {uid}");
                    }
                    else
                    {
                        Console.WriteLine("[AuthService] Token is not a JWT or missing 'sub'. Fetching UserInfo...");
                        try 
                        {
                            // Re-use the handler for SSL bypass
                            using var userInfoClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
                            userInfoClient.DefaultRequestHeaders.Authorization = 
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
                            
                            // Try standard OIDC UserInfo endpoint
                            var userInfo = await userInfoClient.GetFromJsonAsync<JsonElement>("connect/userinfo");
                            
                            if (userInfo.TryGetProperty("sub", out var subProp))
                            {
                                var subVal = subProp.GetString();
                                if (int.TryParse(subVal, out int subId))
                                {
                                    AppState.CurrentUserId = subId;
                                    Console.WriteLine($"[AuthService] UserID from UserInfo: {subId}");
                                }
                                else
                                {
                                    Console.WriteLine($"[AuthService] 'sub' claim found but not an int: {subVal}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[AuthService] UserInfo response did not contain 'sub'.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[AuthService] Failed to fetch UserInfo: {ex.Message}");
                        }
                    }

                    return result.AccessToken;
                }
            }
            
            Console.WriteLine($"[AuthService] Login failed. Status: {response.StatusCode}");
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[AuthService] Response: {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterApiDto dto)
    {
        Console.WriteLine($"[AuthService] RegisterAsync called for {dto.Login}");
        
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        
        using var client = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

        try 
        {
            var response = await client.PostAsJsonAsync("api/auth/register", dto);
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RegisterResponse>(content, options);
                if (result != null && result.Success)
                {
                    Console.WriteLine($"[AuthService] Registration success. UserID: {result.UserId}");
                    return (true, "Registration successful.");
                }
                return (false, "Registration failed (invalid response).");
            }
            
            Console.WriteLine($"[AuthService] Registration failed. Status: {response.StatusCode}");
            
            try 
            {
                var result = JsonSerializer.Deserialize<RegisterResponse>(content, options);
                if (result != null && result.Errors != null && result.Errors.Count > 0)
                {
                    var errorMsg = string.Join(". ", result.Errors);
                    return (false, errorMsg);
                }
            }
            catch { }

            return (false, $"Registration failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Exception: {ex.Message}");
            return (false, $"Error: {ex.Message}");
        }
    }

    private class RegisterResponse
    {
        public bool Success { get; set; }
        public int? UserId { get; set; }
        public List<string>? Errors { get; set; }
    }

    private class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string Role { get; set; } = "";
    }

    private (string? sub, string? role, string? name) ParseJwt(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return (null, null, null);

            var payload = parts[1];
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            
            var bytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            var json = Encoding.UTF8.GetString(bytes);
            using var doc = JsonDocument.Parse(json);
            
            string? sub = null, role = null, name = null;
            
            if (doc.RootElement.TryGetProperty("sub", out var s)) sub = s.GetString();
            if (doc.RootElement.TryGetProperty("name", out var n)) name = n.GetString();
            if (doc.RootElement.TryGetProperty("role", out var r)) role = r.ToString();
            
            return (sub, role, name);
        }
        catch 
        {
            return (null, null, null);
        }
    }
}