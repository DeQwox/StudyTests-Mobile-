using Avalonia;
using Avalonia.ReactiveUI;

namespace OnlineTestingClient;

class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();               // це працює, бо у тебе є Avalonia.ReactiveUI 11.3.8
}