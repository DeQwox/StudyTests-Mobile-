using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTestingClient.ViewModels;

public partial class CreateTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly IApiService _apiService;
    private readonly string _userId;

    [ObservableProperty] private string name = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private DateTimeOffset validUntil = DateTimeOffset.Now.AddDays(7);
    
    [ObservableProperty] private ObservableCollection<CreateQuestionViewModel> questions = new();
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] private string errorMessage = "";
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public CreateTestViewModel(MainWindowViewModel owner, string userId)
    {
        _owner = owner;
        _userId = userId;
        _apiService = new ApiService();
        
        // Add one default question
        Questions.Add(new CreateQuestionViewModel());
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
    private async Task CreateTest()
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

            var result = await _apiService.CreateTestFullAsync(dto);

            if (result != null)
            {
                // Success! Navigate back to tests or show success message
                _owner.CurrentPage = new TestsViewModel(_owner, _userId);
            }
            else
            {
                ErrorMessage = "Failed to create test. Check logs.";
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
