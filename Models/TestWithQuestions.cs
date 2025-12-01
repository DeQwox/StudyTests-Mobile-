using System;
using System.Collections.Generic;

namespace OnlineTestingClient.Models;

public class TestWithQuestions
{
    public int Id { get; set; }
    public int TeacherID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime ValidUntil { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string Description { get; set; } = "";
    public List<string> Answers { get; set; } = new();
    public double Score { get; set; }
}
