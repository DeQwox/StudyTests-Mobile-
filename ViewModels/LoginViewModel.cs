using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Services;
using OnlineTestingClient.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace OnlineTestingClient.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly IAuthService _authService;

    public LoginViewModel(MainWindowViewModel owner)
    {
        _owner = owner;
        _authService = new OnlineTestingClient.Services.AuthService();
    }

    [ObservableProperty]
    private string username = "";

    [ObservableProperty]
    private string password = "";

    // Registration fields
    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string phoneNumber = "";
    [ObservableProperty] private string selectedRole = "Student";
    
    public List<string> Roles { get; } = new() { "Student", "Teacher" };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeButtonText))]
    [NotifyPropertyChangedFor(nameof(TitleText))]
    private bool isRegisterMode = false;

    public string ModeButtonText => IsRegisterMode ? "Вже є акаунт? Увійти" : "Немає акаунту? Зареєструватися";
    public string TitleText => IsRegisterMode ? "Реєстрація" : "Вхід";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSuccess))]
    private string successMessage = "";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegisterMode = !IsRegisterMode;
        ErrorMessage = "";
        SuccessMessage = "";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var token = await _authService.LoginAsync(Username, Password);
            if (!string.IsNullOrEmpty(token))
            {
                var userId = AppState.CurrentUserId.ToString();
                _owner.LoginSuccess(userId);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                IsLoading = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Login, Password and Email are required";
            return;
        }

        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";

        try
        {
            var dto = new RegisterApiDto
            {
                Login = Username,
                Password = Password,
                Email = Email,
                FullName = FullName,
                PhoneNumber = PhoneNumber,
                Role = SelectedRole
            };

            var (success, message) = await _authService.RegisterAsync(dto);
            if (success)
            {
                SuccessMessage = "Registration successful! Please login.";
                IsRegisterMode = false; // Switch back to login
                // Clear sensitive fields if needed, but keeping username might be helpful
                Password = ""; 
            }
            else
            {
                ErrorMessage = message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}