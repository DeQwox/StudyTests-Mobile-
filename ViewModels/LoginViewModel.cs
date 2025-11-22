using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OnlineTestingClient.ViewModels;

public partial class LoginViewModel(MainWindowViewModel main) : ViewModelBase
{
    private readonly MainWindowViewModel _main = main;
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    public string UserId { get; private set; } = "";
    [ObservableProperty] private bool isLoading;

    private async Task FetchData()
    {
        IsLoading = true;
        await Login();
        IsLoading = false;
    }

    [RelayCommand]
    private async Task Login()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username, Password });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            UserId = result!.userId;
        }
    }
}