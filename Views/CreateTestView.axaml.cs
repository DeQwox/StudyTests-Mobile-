using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OnlineTestingClient.Views;

public partial class CreateTestView : UserControl
{
    public CreateTestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
