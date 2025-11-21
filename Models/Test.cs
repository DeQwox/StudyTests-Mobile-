// Models/Test.cs — додаємо DisplayName
public class Test
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Password { get; set; }
    public int TeacherId { get; set; }

    public override string ToString() => $"{Name} (ID: {Id})";
}