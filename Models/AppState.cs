namespace OnlineTestingClient.Models;

public static class AppState
{
    public static string? Token { get; set; }
    public static int CurrentUserId { get; set; }       // отримуємо після логіну
    public static string? CurrentUserRole { get; set; } // "Teacher" або "Student"
    public static string? CurrentUserName { get; set; }
    // Configurable API base URL (host + port). Can be changed at runtime or from settings.
    // Default uses localhost with the server's default port used during development.
    public static string ApiBaseUrl { get; set; } = "https://localhost:5001";
}