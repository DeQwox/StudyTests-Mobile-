// Services/IApiService.cs
using OnlineTestingClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineTestingClient.Services;

public interface IApiService
{
    Task<Test> CreateTestAsync(Test test);
    Task<Question> CreateQuestionAsync(Question question);
    Task<List<Test>> GetMyTestsAsync();
    Task<List<Test>> GetAvailableTestsAsync();
    Task<List<Question>> GetQuestionsAsync(int testId);
    Task<List<PassedTest>> GetStatisticsAsync();
    Task<TestWithQuestions?> GetTestByPasswordAsync(string password);
    Task<PassedTest?> SubmitTestResultAsync(PassedTestSubmission submission);
    Task<TestWithQuestions?> CreateTestFullAsync(CreateTestDto dto);
    Task<bool> UpdateTestFullAsync(int testId, CreateTestDto dto);
    Task<List<TeacherPassedTestDto>> GetPassedTestsByTeacherAsync();
    Task<List<StudentPassedTestDto>> GetPassedTestsByStudentAsync();
}