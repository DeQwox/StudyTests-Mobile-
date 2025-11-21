// Services/IApiService.cs
using OnlineTestingClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineTestingClient.Services;

public interface IApiService
{
    Task<Test> CreateTestAsync(Test test);
    Task<Question> CreateQuestionAsync(Question question);
    Task<PassedTest> SubmitPassedTestAsync(PassedTest passed);
    Task<List<Test>> GetMyTestsAsync();
    Task<List<Test>> GetAvailableTestsAsync();
    Task<List<Question>> GetQuestionsAsync(int testId);
    Task<List<PassedTest>> GetStatisticsAsync();
}