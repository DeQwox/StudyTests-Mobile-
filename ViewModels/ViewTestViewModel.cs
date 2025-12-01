using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OnlineTestingClient.ViewModels;

public partial class ViewTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly string _userId;

    [ObservableProperty]
    private Test test;

    [ObservableProperty]
    private ObservableCollection<QuestionDisplay> questions;

    public ViewTestViewModel(MainWindowViewModel owner, string userId, Test test, List<Question> questions)
    {
        _owner = owner;
        _userId = userId;
        Test = test;
        Questions = new ObservableCollection<QuestionDisplay>(
            questions.Select(q => new QuestionDisplay
            {
                Description = q.Description,
                Score = q.Score,
                Answers = q.Answers.Select((a, index) => new AnswerDisplay
                {
                    Text = a,
                    IsCorrect = index == q.CorrectAnswerIndex
                }).ToList()
            })
        );
    }

    [RelayCommand]
    private void GoBack()
    {
        _owner.CurrentPage = new TestsViewModel(_owner, _userId);
    }
}

public class QuestionDisplay
{
    public string Description { get; set; } = "";
    public List<AnswerDisplay> Answers { get; set; } = new();
    public double Score { get; set; }
}

public class AnswerDisplay
{
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
}
