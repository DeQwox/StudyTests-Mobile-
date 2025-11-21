// ViewModels/AddQuestionViewModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class AddQuestionViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private List<Test> myTests = new();
    [ObservableProperty] private Test? selectedTest;
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string answers = "";
    [ObservableProperty] private int correctIndex;
    [ObservableProperty] private string result = "";

    public AddQuestionViewModel(MainWindowViewModel main)
    {
        _main = main;
        _ = LoadTestsAsync();
    }

    private async Task LoadTestsAsync()
    {
        var api = new ApiService();
        MyTests = await api.GetMyTestsAsync();
        SelectedTest = MyTests.FirstOrDefault();
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (SelectedTest == null)
        {
            Result = "Оберіть тест";
            return;
        }

        var api = new ApiService();
        var q = new Question
        {
            TestId = SelectedTest.Id,
            Description = Description,
            Answers = Answers.Split(';', StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => s.Trim()).ToList(),
            CorrectAnswerIndex = CorrectIndex,
            Score = 1.0
        };

        var created = await api.CreateQuestionAsync(q);
        Result = created?.Id > 0 ? "Питання додано!" : "Помилка";
    }
}