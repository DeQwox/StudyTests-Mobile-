// ViewModels/MainWindowViewModel.cs
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase currentView = null!;
    [ObservableProperty] private bool isLoggedIn;
    [ObservableProperty] private bool isLoading;

    public LoginViewModel LoginView { get; }
    public CreateTestViewModel CreateTestView { get; }
    public AddQuestionViewModel AddQuestionView { get; }
    public PassTestViewModel PassTestView { get; }
    private StatisticsViewModel? _statisticsView;
    public StatisticsViewModel StatisticsView => _statisticsView ??= new StatisticsViewModel(this);
    public AboutViewModel AboutView { get; }

    public MainWindowViewModel()
    {
        LoginView = new(this);
        CreateTestView = new(this);
        AddQuestionView = new(this);
        PassTestView = new(this);
        // StatisticsView is created lazily when requested to avoid loading heavy data at startup.
        AboutView = new();

        CurrentView = LoginView;
    }

    public void ShowAppropriateView()
    {
        CurrentView = AppState.CurrentUserRole == "Teacher" ? CreateTestView : PassTestView;
    }
    public void ShowCreateTest() => CurrentView = CreateTestView;
    public void ShowAddQuestion() => CurrentView = AddQuestionView;
    public void ShowPassTest() => CurrentView = PassTestView;
    public void ShowStatistics()
    {
        CurrentView = StatisticsView;
        // Start loading statistics in background when the view is shown.
        _ = StatisticsView.LoadStatsAsync();
    }
    public void ShowAbout() => CurrentView = AboutView;
}
