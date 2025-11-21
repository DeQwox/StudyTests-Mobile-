// Models/Question.cs
using System.Collections.Generic;

namespace OnlineTestingClient.Models;

public class Question
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string Description { get; set; } = "";
    public List<string> Answers { get; set; } = new();
    public int CorrectAnswerIndex { get; set; }
    public double Score { get; set; }
}