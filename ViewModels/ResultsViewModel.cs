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
    [ObservableProperty] private bool isLoading;
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
        var data = await _httpClient.GetFromJsonAsync<List<TestResult>>($"api/tests/results?userId={_userId}");
        Results = new ObservableCollection<TestResult>(data ?? new List<TestResult>());
        IsLoading = false;
    }
}