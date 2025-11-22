using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Net.Http.Json;
using System.Collections.ObjectModel;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace OnlineTestingClient.ViewModels;

public partial class ChartViewModel : ViewModelBase
{
    public ISeries[] Series { get; set; } = [];

    public Axis[] XAxes { get; set; } = [new Axis { Labels = new ObservableCollection<string>() }];
    public Axis[] YAxes { get; set; } = [new Axis { MinLimit = 0, MaxLimit = 100 }];

    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:5001/") };
    private string _userId;

    public ChartViewModel(string userId)
    {
        _userId = userId;
        LoadData();
    }

    private async void LoadData()
    {
        var scores = await _httpClient.GetFromJsonAsync<List<dynamic>>($"api/tests/last10?userId={_userId}");

        var values = scores!.Select(s => (double)s.score).ToArray();

        var labels = scores!.Select(s => ((DateTime)s.date).ToString("MM/dd")).ToArray();
        XAxes = [new Axis { Labels = labels }];

        Series =
        [
            new LineSeries<double>
            {
                Values = values
            }
        ];
    }
}