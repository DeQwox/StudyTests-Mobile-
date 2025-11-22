using CommunityToolkit.Mvvm.ComponentModel;
using OnlineTestingClient.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace OnlineTestingClient.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string summaryMessage = "Loading...";
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    private string _userId;

    public HomeViewModel(string userId)
    {
        _userId = userId;
        LoadData();
    }

    private /*async*/ void LoadData()
    {
        IsLoading = true;
        // Fetch summary from API, e.g., var summary = await _httpClient.GetFromJsonAsync<Summary>("api/summary?userId=" + _userId);
        // For now, mock or add endpoint
        SummaryMessage = "Your average score: 85% (from last 10 tests)"; // Replace with real data
        IsLoading = false;
    }
}