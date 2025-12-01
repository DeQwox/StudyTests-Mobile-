using System;
using System.Collections.Generic;

namespace OnlineTestingClient.Models;

public class CreateTestDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Password { get; set; } = "";
    public DateTime ValidUntil { get; set; } = DateTime.UtcNow.AddDays(1);
    public List<CreateQuestionDto> Questions { get; set; } = new();
}

public class CreateQuestionDto
{
    public string Description { get; set; } = "";
    public List<string> Answers { get; set; } = new();
    public int CorrectAnswerIndex { get; set; }
    public double Score { get; set; }
}
