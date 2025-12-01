using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTestingClient.ViewModels;

public partial class EditTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly IApiService _apiService;
    private readonly string _userId;
    private readonly int _testId;

    [ObservableProperty] private string name = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private DateTimeOffset validUntil = DateTimeOffset.Now.AddDays(7);
    
    [ObservableProperty] private ObservableCollection<CreateQuestionViewModel> questions = new();
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] private string errorMessage = "";
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasSuccess))] private string successMessage = "";
    public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

    public EditTestViewModel(MainWindowViewModel owner, string userId, Test test, List<Question> existingQuestions)
    {
        _owner = owner;
        _userId = userId;
        _testId = test.Id;
        _apiService = new ApiService();
        
        // Initialize with existing data
        Name = test.Name;
        Description = test.Description;
        Password = test.Password ?? "";
        ValidUntil = test.ValidUntil > DateTime.MinValue ? test.ValidUntil : DateTime.Now.AddDays(7);

        foreach (var q in existingQuestions)
        {
            var qVm = new CreateQuestionViewModel
            {
                Description = q.Description,
                CorrectAnswerIndex = q.CorrectAnswerIndex,
                Score = q.Score
            };
            foreach (var ans in q.Answers)
            {
                qVm.Answers.Add(ans);
            }
            Questions.Add(qVm);
        }
    }

    [RelayCommand]
    private void AddQuestion()
    {
        Questions.Add(new CreateQuestionViewModel());
    }

    [RelayCommand]
    private void RemoveQuestion(CreateQuestionViewModel question)
    {
        if (Questions.Contains(question))
        {
            Questions.Remove(question);
        }
    }

    [RelayCommand]
    private async Task SaveTest()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Test name is required";
            return;
        }

        if (Questions.Count == 0)
        {
            ErrorMessage = "Add at least one question";
            return;
        }

        IsLoading = true;
        ErrorMessage = "";

        try
        {
            var dto = new CreateTestDto
            {
                Name = Name,
                Description = Description,
                Password = Password,
                ValidUntil = ValidUntil.DateTime,
                Questions = Questions.Select(q => new CreateQuestionDto
                {
                    Description = q.Description,
                    Answers = q.Answers.ToList(),
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Score = q.Score
                }).ToList()
            };

            var success = await _apiService.UpdateTestFullAsync(_testId, dto);

            if (success)
            {
                SuccessMessage = "Зміни збережено!";
                ErrorMessage = "";
                await Task.Delay(1500);
                _owner.CurrentPage = new TestsViewModel(_owner, _userId);
            }
            else
            {
                ErrorMessage = "Failed to update test. Check logs.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _owner.CurrentPage = new TestsViewModel(_owner, _userId);
    }
}
