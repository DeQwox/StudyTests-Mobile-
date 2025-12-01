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

public partial class QuestionViewModel : ObservableObject
{
    public QuestionDto Model { get; }
    
    [ObservableProperty]
    private int selectedAnswerIndex = -1;

    public QuestionViewModel(QuestionDto model)
    {
        Model = model;
    }
}

public partial class TakeTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly string _userId;
    private readonly IApiService _apiService;
    
    [ObservableProperty]
    private TestWithQuestions test;

    [ObservableProperty]
    private ObservableCollection<QuestionViewModel> questions;

    public TakeTestViewModel(MainWindowViewModel owner, string userId, TestWithQuestions test)
    {
        _owner = owner;
        _userId = userId;
        Test = test;
        _apiService = new ApiService();
        Questions = new ObservableCollection<QuestionViewModel>(
            test.Questions.Select(q => new QuestionViewModel(q))
        );
    }

    [RelayCommand]
    private async Task Submit()
    {
        Console.WriteLine("[TakeTestViewModel] Submit command triggered.");
        
        if (IsLoading)
        {
            Console.WriteLine("[TakeTestViewModel] Submit ignored because IsLoading is true.");
            return;
        }

        IsLoading = true;
        try
        {
            Console.WriteLine($"[TakeTestViewModel] Preparing submission for TestId={Test.Id}...");
            var answers = new List<string>();
            double totalScore = 0;

            foreach (var q in Questions)
            {
                if (q.SelectedAnswerIndex >= 0 && q.SelectedAnswerIndex < q.Model.Answers.Count)
                {
                    var answer = q.Model.Answers[q.SelectedAnswerIndex];
                    answers.Add(answer);
                }
                else
                {
                    answers.Add(""); // Unanswered
                }
            }

            int.TryParse(_userId, out int parsedId);
            int? studentIdToSend = parsedId == 0 ? null : parsedId;
            
            Console.WriteLine($"[TakeTestViewModel] Parsed StudentId: {parsedId}. Sending: {(studentIdToSend.HasValue ? studentIdToSend.ToString() : "null")}");

            var submission = new PassedTestSubmission
            {
                StudentId = studentIdToSend,
                TestId = Test.Id,
                Answers = answers,
                Score = 0, 
                PassedAt = DateTime.UtcNow
            };

            Console.WriteLine($"[TakeTestViewModel] Payload: StudentId={submission.StudentId}, TestId={submission.TestId}, AnswersCount={submission.Answers.Count}");

            var result = await _apiService.SubmitTestResultAsync(submission);
            
            if (result != null)
            {
                // Server response doesn't include TestName, so we set it manually from the current context
                result.TestName = Test.Name;
            }

            Console.WriteLine($"[TakeTestViewModel] Submission result: {(result != null ? "Success" : "NULL")}");

            _owner.CurrentPage = new TestResultViewModel(_owner, _userId, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TakeTestViewModel] EXCEPTION: {ex}");
             _owner.CurrentPage = new TestResultViewModel(_owner, _userId, null);
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
