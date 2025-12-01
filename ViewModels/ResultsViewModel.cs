using CommunityToolkit.Mvvm.ComponentModel;
using OnlineTestingClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace OnlineTestingClient.ViewModels;

public class TestResult { public int Id { get; set; } public string TestName { get; set; } = ""; public decimal Score { get; set; } public DateTime Date { get; set; } } // Model

public partial class ResultsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<TestResult> results = new();
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] private string errorMessage = "";
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    private string _userId;

    public ResultsViewModel(string userId)
    {
        _userId = userId;
        LoadResults();
    }

    private async void LoadResults()
    {
        IsLoading = true;
        ErrorMessage = "";
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(Models.AppState.ApiBaseUrl ?? "https://localhost:5001/") };
            var data = await client.GetFromJsonAsync<List<TestResult>>($"api/tests/results?userId={_userId}");
            Results = new ObservableCollection<TestResult>(data ?? new List<TestResult>());
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load results: " + ex.Message;
            Results = new ObservableCollection<TestResult>();
        }
        finally
        {
            IsLoading = false;
        }
    }
}