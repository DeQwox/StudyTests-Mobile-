// Services/ApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OnlineTestingClient.Models;

namespace OnlineTestingClient.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;

    // Allow injecting a base URL for flexibility (tests, different envs).
    public ApiService(string? token = null, string? baseUrl = null)
    {
        var baseAddress = baseUrl ?? OnlineTestingClient.Models.AppState.ApiBaseUrl ?? "https://localhost:5001";
        _http = new HttpClient { BaseAddress = new Uri(baseAddress) };

        var useToken = token ?? OnlineTestingClient.Models.AppState.Token;
        if (!string.IsNullOrEmpty(useToken))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", useToken);
        }
    }

    public async Task<Test> CreateTestAsync(Test test)
    {
        var result = await PostAsync<Test>("api/tests", test);
        return result ?? test;
    }

    public async Task<Question> CreateQuestionAsync(Question question)
    {
        var result = await PostAsync<Question>("api/questions", question);
        return result ?? question;
    }

    public async Task<PassedTest> SubmitPassedTestAsync(PassedTest passed)
    {
        var result = await PostAsync<PassedTest>("api/passedtests", passed);
        return result ?? passed;
    }

    public async Task<List<Test>> GetMyTestsAsync()
        => await GetAsync<List<Test>>("api/tests") ?? new();

    public async Task<List<Test>> GetAvailableTestsAsync()
        => await GetAsync<List<Test>>("api/tests/available") ?? new();

    public async Task<List<Question>> GetQuestionsAsync(int testId)
        => await GetAsync<List<Question>>($"api/tests/{testId}/questions") ?? new();

    public async Task<List<PassedTest>> GetStatisticsAsync()
        => await GetAsync<List<PassedTest>>("api/passed-tests/statistics") ?? new();

    private async Task<T?> PostAsync<T>(string url, object data)
    {
        var response = await _http.PostAsJsonAsync(url, data);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : default;
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        var response = await _http.GetAsync(url);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : default;
    }
}