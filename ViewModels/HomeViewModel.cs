using CommunityToolkit.Mvvm.ComponentModel;
using OnlineTestingClient.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace OnlineTestingClient.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    // [ObservableProperty] private bool isLoading; // Inherited from ViewModelBase
    [ObservableProperty] private string summaryMessage = "Loading...";
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    private string _userId;

    public HomeViewModel(string userId)
    {
        _userId = userId;
        LoadData();
    }

    private async void LoadData()
    {
        IsLoading = true;
        try
        {
            // Simulate data loading or fetch real data
            await System.Threading.Tasks.Task.Delay(1000); 
            
            // Example: Fetch available tests count if we had a service method
            // var service = new Services.ApiService();
            // var tests = await service.GetAvailableTestsAsync();
            // SummaryMessage = $"Available tests: {tests.Count}";

            SummaryMessage = "Welcome back! You have access to the testing system.";
        }
        catch (Exception ex)
        {
            SummaryMessage = "Failed to load data: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}