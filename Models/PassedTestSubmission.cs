using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OnlineTestingClient.Models;

public class PassedTestSubmission
{
    [JsonPropertyName("studentId")]
    public int? StudentId { get; set; }

    [JsonPropertyName("testId")]
    public int TestId { get; set; }

    [JsonPropertyName("answers")]
    public List<string> Answers { get; set; } = new();

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("passedAt")]
    public DateTime? PassedAt { get; set; }
}
