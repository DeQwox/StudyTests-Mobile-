using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace OnlineTestingClient.ViewModels;

public class Test { public int Id { get; set; } public string Name { get; set; } = ""; }

public partial class TestsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Test> tests = new();
    [ObservableProperty] private bool isLoading;
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    private string _userId;

    public TestsViewModel(string userId)
    {
        _userId = userId;
        LoadTests();
    }

    private async void LoadTests()
    {
        IsLoading = true;
        var data = await _httpClient.GetFromJsonAsync<List<Test>>($"api/tests/list?userId={_userId}");
        Tests = new ObservableCollection<Test>(data ?? new List<Test>());
        IsLoading = false;
    }

    [RelayCommand]
    private void TakeTest(int testId)
    {
        // Handle taking test, e.g., navigate to TakeTestView or submit
    }
}