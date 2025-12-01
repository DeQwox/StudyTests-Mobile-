using CommunityToolkit.Mvvm.ComponentModel;

namespace OnlineTestingClient.ViewModels;

public partial class SuccessViewModel : ViewModelBase
{
    [ObservableProperty]
    private string userId;

    public SuccessViewModel(string userId)
    {
        this.userId = userId;
    }
}