// Models/Test.cs — додаємо DisplayName
public class Test
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Password { get; set; }
    public int TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ValidUntil { get; set; }
    public int QuestionCount { get; set; }

    public override string ToString() => $"{Name} (ID: {Id})";
}