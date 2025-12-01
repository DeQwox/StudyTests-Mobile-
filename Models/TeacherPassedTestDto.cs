using System;
using System.Collections.Generic;

namespace OnlineTestingClient.Models;

public class TeacherPassedTestDto
{
    public int PassedTestId { get; set; }
    public int TestId { get; set; }
    public string TestName { get; set; } = "";
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public double Score { get; set; }
    public DateTime PassedAt { get; set; }
    public List<string> Answers { get; set; } = new();
}
