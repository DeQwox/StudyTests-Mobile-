using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class PassTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private List<Test> availableTests = new();
    [ObservableProperty] private Test? selectedTest;
    [ObservableProperty] private List<Question> questions = new();
    [ObservableProperty] private List<int> selectedAnswers = new();
    [ObservableProperty] private string result = "";
    [ObservableProperty] private bool isTestStarted;

    public PassTestViewModel(MainWindowViewModel main)
    {
        _main = main;
        _ = LoadTestsAsync();
    }

    private async Task LoadTestsAsync()
    {
        var api = new ApiService();
        AvailableTests = await api.GetAvailableTestsAsync();
    }

    [RelayCommand]
    private async Task StartTest()
    {
        if (SelectedTest == null) return;

        IsTestStarted = true;
        var api = new ApiService();
        Questions = await api.GetQuestionsAsync(SelectedTest.Id);
        SelectedAnswers = Enumerable.Repeat(-1, Questions.Count).ToList();
    }

    [RelayCommand]
    private async Task SubmitTest()
    {
        if (SelectedTest == null) return;

        double earned = 0;
        double max = Questions.Sum(q => q.Score);

        for (int i = 0; i < Questions.Count; i++)
        {
            if (SelectedAnswers[i] == Questions[i].CorrectAnswerIndex)
                earned += Questions[i].Score;
        }

        var passed = new PassedTest
        {
            TestId = SelectedTest.Id,
            StudentId = AppState.CurrentUserId,
            Score = earned
        };

        var api = new ApiService();
        await api.SubmitPassedTestAsync(passed);

        Result = $"Тест здано! Бал: {earned:F1} з {max}";
        IsTestStarted = false;
    }
}