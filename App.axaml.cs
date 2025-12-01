using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OnlineTestingClient.ViewModels;
using OnlineTestingClient.Views;

namespace OnlineTestingClient;

public partial class App : Application
{
    public static string? CurrentToken { get; set; }
    public static string? CurrentUserId { get; set; }

    public static MainWindowViewModel MainViewModel { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainViewModel = new MainWindowViewModel();

            desktop.MainWindow = new MainWindow
            {
                DataContext = MainViewModel
            };

            MainViewModel.ShowLogin();
        }

        base.OnFrameworkInitializationCompleted();
    }
}