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

    public async Task<List<Test>> GetMyTestsAsync()
        => await GetAsync<List<Test>>("api/tests/my") ?? new();

    public async Task<List<Test>> GetAvailableTestsAsync()
        => await GetAsync<List<Test>>("api/tests/available") ?? new();

    public async Task<List<Question>> GetQuestionsAsync(int testId)
        => await GetAsync<List<Question>>($"api/tests/{testId}/questions") ?? new();

    public async Task<List<PassedTest>> GetStatisticsAsync()
        => await GetAsync<List<PassedTest>>("api/passed-tests/statistics") ?? new();

    public async Task<TestWithQuestions?> GetTestByPasswordAsync(string password)
        => await GetAsync<TestWithQuestions>($"api/tests/by-password/full?password={password}");

    public async Task<PassedTest?> SubmitTestResultAsync(PassedTestSubmission submission)
    {
        try
        {
            // Use the main client which has the base address and potentially the token
            // If we need to ensure the token is set (in case it was set after ApiService creation), we can reset headers
            if (!string.IsNullOrEmpty(AppState.Token))
            {
                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
            }

            var json = System.Text.Json.JsonSerializer.Serialize(submission);
            Console.WriteLine($"[ApiService] Sending POST to api/passed-tests. Payload: {json}");

            var response = await _http.PostAsJsonAsync("api/passed-tests", submission);
            
            if (response.IsSuccessStatusCode)
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await response.Content.ReadFromJsonAsync<PassedTest>(options);
                Console.WriteLine($"[ApiService] Success. Received result: ID={result?.Id}, Score={result?.Score}");
                return result;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ApiService] Request failed. Status: {response.StatusCode}. Response: {errorBody}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiService] Exception during SubmitTestResultAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<TestWithQuestions?> CreateTestFullAsync(CreateTestDto dto)
    {
        try
        {
            if (!string.IsNullOrEmpty(AppState.Token))
            {
                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
            }

            var response = await _http.PostAsJsonAsync("api/tests/full", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<TestWithQuestions>(options);
            }
            
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ApiService] CreateTestFullAsync failed: {response.StatusCode} - {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiService] Exception in CreateTestFullAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateTestFullAsync(int testId, CreateTestDto dto)
    {
        try
        {
            if (!string.IsNullOrEmpty(AppState.Token))
            {
                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
            }

            var response = await _http.PutAsJsonAsync($"api/tests/full/{testId}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ApiService] UpdateTestFullAsync failed: {response.StatusCode} - {error}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiService] Exception in UpdateTestFullAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<List<TeacherPassedTestDto>> GetPassedTestsByTeacherAsync()
        => await GetAsync<List<TeacherPassedTestDto>>("api/passed-tests/by-teacher") ?? new();

    public async Task<List<StudentPassedTestDto>> GetPassedTestsByStudentAsync()
        => await GetAsync<List<StudentPassedTestDto>>("api/passed-tests/by-student") ?? new();

    private async Task<T?> PostAsync<T>(string url, object data)
    {
        if (!string.IsNullOrEmpty(AppState.Token))
        {
            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
        }

        var response = await _http.PostAsJsonAsync(url, data);
        if (response.IsSuccessStatusCode)
        {
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return await response.Content.ReadFromJsonAsync<T>(options);
        }
        return default;
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        if (!string.IsNullOrEmpty(AppState.Token))
        {
            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.Token);
        }

        var response = await _http.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return await response.Content.ReadFromJsonAsync<T>(options);
        }
        return default;
    }
}