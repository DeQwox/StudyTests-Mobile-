using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Net.Http.Json;
using System.Collections.ObjectModel;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class ChartViewModel : ViewModelBase
{
    [ObservableProperty] private ISeries[] series = [];
    [ObservableProperty] private Axis[] xAxes = [new Axis { Labels = new ObservableCollection<string>() }];
    [ObservableProperty] private Axis[] yAxes = [new Axis { MinLimit = 0, MaxLimit = 100 }];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] private string errorMessage = "";
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty] private ObservableCollection<TeacherPassedTestDto> teacherPassedTests = new();
    [ObservableProperty] private ObservableCollection<StudentPassedTestDto> studentPassedTests = new();

    [ObservableProperty] private bool isEmpty = false;

    private readonly IApiService _apiService;
    private string _userId;

    public bool IsTeacher => string.Equals(AppState.CurrentUserRole, "Teacher", StringComparison.OrdinalIgnoreCase);
    public bool IsStudent => string.Equals(AppState.CurrentUserRole, "Student", StringComparison.OrdinalIgnoreCase);

    public ChartViewModel(string userId)
    {
        _userId = userId;
        _apiService = new ApiService();
        LoadData();
    }

    private async void LoadData()
    {
        IsLoading = true;
        ErrorMessage = "";
        IsEmpty = false;
        try
        {
            if (IsTeacher)
            {
                var results = await _apiService.GetPassedTestsByTeacherAsync();
                TeacherPassedTests = new ObservableCollection<TeacherPassedTestDto>(results);
                IsEmpty = !results.Any();
            }
            else
            {
                var results = await _apiService.GetPassedTestsByStudentAsync();
                StudentPassedTests = new ObservableCollection<StudentPassedTestDto>(results);

                if (results.Any())
                {
                    // Sort by date for the chart
                    var sorted = results.OrderBy(r => r.PassedAt).ToList();
                    var values = sorted.Select(s => s.Score).ToArray();
                    var labels = sorted.Select(s => s.PassedAt.ToString("MM/dd HH:mm")).ToArray();
                    
                    XAxes = [new Axis { Labels = labels }];
                    Series = [ new LineSeries<double> { Values = values, Name = "Score" } ];
                }
                else
                {
                    IsEmpty = true;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load statistics: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}