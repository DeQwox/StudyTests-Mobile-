using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using System.Collections.ObjectModel;

namespace OnlineTestingClient.ViewModels;

public partial class CreateQuestionViewModel : ObservableObject
{
    [ObservableProperty] private string description = "";
    [ObservableProperty] private double score = 1;
    [ObservableProperty] private ObservableCollection<string> answers = new();
    [ObservableProperty] private int correctAnswerIndex = 0;
    [ObservableProperty] private string newAnswerText = "";

    public CreateQuestionViewModel()
    {
        // Default answers
        Answers.Add("Option 1");
        Answers.Add("Option 2");
    }

    [RelayCommand]
    private void AddAnswer()
    {
        if (!string.IsNullOrWhiteSpace(NewAnswerText))
        {
            Answers.Add(NewAnswerText);
            NewAnswerText = "";
        }
    }

    [RelayCommand]
    private void RemoveAnswer(string answer)
    {
        if (Answers.Contains(answer))
        {
            Answers.Remove(answer);
            if (CorrectAnswerIndex >= Answers.Count)
                CorrectAnswerIndex = Answers.Count > 0 ? 0 : -1;
        }
    }
}
