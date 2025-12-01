using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OnlineTestingClient.Views;

public partial class EditTestView : UserControl
{
    public EditTestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
