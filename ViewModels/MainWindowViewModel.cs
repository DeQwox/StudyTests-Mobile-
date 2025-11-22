using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.ViewModels;

namespace OnlineTestingClient.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase? currentPage;
    private string _userId;

    public MainWindowViewModel(string userId) 
    { 
        _userId = userId; 
        CurrentPage = new HomeViewModel(_userId);
    }

    [RelayCommand] private void NavigateHome() => CurrentPage = new HomeViewModel(_userId);
    [RelayCommand] private void NavigateTests() => CurrentPage = new TestsViewModel(_userId);
    [RelayCommand] private void NavigateResults() => CurrentPage = new ResultsViewModel(_userId);
    [RelayCommand] private void NavigateStatistics() => CurrentPage = new ChartViewModel(_userId);
    [RelayCommand] private void NavigateAbout() => CurrentPage = new AboutViewModel();
    [RelayCommand] private void Logout() { /* Navigate to login or close */ }
}