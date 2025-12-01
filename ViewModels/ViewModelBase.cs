using CommunityToolkit.Mvvm.ComponentModel;

namespace OnlineTestingClient.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    public bool isLoading;

    public bool IsNotLoading => !IsLoading;
}