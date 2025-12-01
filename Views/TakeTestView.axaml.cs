using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OnlineTestingClient.Views;

public partial class TakeTestView : UserControl
{
    public TakeTestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
