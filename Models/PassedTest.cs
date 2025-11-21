// Models/PassedTest.cs
namespace OnlineTestingClient.Models;

public class PassedTest
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public double Score { get; set; }
    public string? TestName { get; set; }
}