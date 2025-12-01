using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OnlineTestingClient.Views;

public partial class ViewTestView : UserControl
{
    public ViewTestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
