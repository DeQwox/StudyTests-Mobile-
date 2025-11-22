using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OnlineTestingClient.ViewModels;
using OnlineTestingClient.Views;

namespace OnlineTestingClient;

public partial class App : Application
{
    public static string? CurrentToken { get; set; }
    public static MainWindowViewModel Services { get; internal set; } = new("");

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}