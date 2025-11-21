using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OnlineTestingClient.Views.Controls;

public partial class LoadingOverlay : UserControl
{
    public LoadingOverlay()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
