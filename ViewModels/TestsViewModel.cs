using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OnlineTestingClient.ViewModels;

public partial class TestsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Test> tests = new();
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] private string errorMessage = "";
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty] private string testCode = "";

    private readonly IApiService _apiService;
    private readonly MainWindowViewModel _owner;
    private string _userId;

    public TestsViewModel(MainWindowViewModel owner, string userId)
    {
        _owner = owner;
        _userId = userId;
        _apiService = new ApiService();
        LoadTests();
    }

    private async void LoadTests()
    {
        IsLoading = true;
        ErrorMessage = "";
        try
        {
            List<Test> data;
            if (AppState.CurrentUserRole == "Student")
            {
                data = await _apiService.GetAvailableTestsAsync();
            }
            else if (AppState.CurrentUserRole == "Teacher")
            {
                data = await _apiService.GetMyTestsAsync();
            }
            else
            {
                // Fallback for other roles (e.g. "User" if used for viewing all tests)
                // We can use the manual client here or add another method to ApiService
                using var client = new HttpClient { BaseAddress = new Uri(AppState.ApiBaseUrl ?? "https://localhost:5001/") };
                if (!string.IsNullOrEmpty(AppState.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
                }
                data = await client.GetFromJsonAsync<List<Test>>("api/tests") ?? new List<Test>();
            }
            
            Tests = new ObservableCollection<Test>(data);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load tests: " + ex.Message;
            Tests = new ObservableCollection<Test>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task JoinTest()
    {
        if (string.IsNullOrWhiteSpace(TestCode))
        {
            ErrorMessage = "Введіть код тесту";
            return;
        }

        IsLoading = true;
        ErrorMessage = "";

        try
        {
            var test = await _apiService.GetTestByPasswordAsync(TestCode);
            if (test != null)
            {
                _owner.CurrentPage = new TakeTestViewModel(_owner, _userId, test);
            }
            else
            {
                ErrorMessage = "Тест не знайдено або код невірний";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public bool IsStudent => AppState.CurrentUserRole == "Student";
    public bool IsUser => AppState.CurrentUserRole == "User";
    public bool IsTeacher => AppState.CurrentUserRole == "Teacher";

    [RelayCommand]
    private async Task ViewTest(Test test)
    {
        IsLoading = true;
        try
        {
            var questions = await _apiService.GetQuestionsAsync(test.Id);
            _owner.CurrentPage = new ViewTestViewModel(_owner, _userId, test, questions);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load test details: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task EditTest(Test test)
    {
        IsLoading = true;
        try
        {
            var questions = await _apiService.GetQuestionsAsync(test.Id);
            _owner.CurrentPage = new EditTestViewModel(_owner, _userId, test, questions);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load test for editing: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task TakeTest(Test test)
    {
        IsLoading = true;
        try
        {
             // Fetch full test details if needed, or just navigate
             // Assuming we need questions to take the test
             var questions = await _apiService.GetQuestionsAsync(test.Id);
             
             // Convert Test to TestWithQuestions if needed by TakeTestViewModel
             // TakeTestViewModel constructor: (MainWindowViewModel owner, string userId, TestWithQuestions test)
             
             var testWithQuestions = new TestWithQuestions
             {
                 Id = test.Id,
                 Name = test.Name,
                 Description = test.Description,
                 TeacherID = test.TeacherId,
                 CreatedAt = DateTime.Now,
                 ValidUntil = DateTime.Now.AddDays(1),
                 Questions = questions.ConvertAll(q => new QuestionDto 
                 { 
                     Id = q.Id, 
                     TestId = q.TestId, 
                     Description = q.Description, 
                     Answers = q.Answers, 
                     Score = q.Score 
                 })
             };

             _owner.CurrentPage = new TakeTestViewModel(_owner, _userId, testWithQuestions);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to start test: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}