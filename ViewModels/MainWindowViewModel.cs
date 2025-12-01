using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;

namespace OnlineTestingClient.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? currentPage;

    [ObservableProperty]
    private bool isLoggedIn;

    [ObservableProperty]
    private bool isTeacher;

    [ObservableProperty]
    private bool isStudent;

    private string? _userId;

    public MainWindowViewModel()
    {
        ShowLogin();
    }

    public void ShowLogin()
    {
        _userId = null;
        IsLoggedIn = false;
        CurrentPage = new LoginViewModel(this);
    }

    public void LoginSuccess(string userId)
    {
        _userId = userId;
        IsLoggedIn = true;

        var role = AppState.CurrentUserRole;
        IsTeacher = role == "Teacher";
        IsStudent = role == "Student";

        CurrentPage = new HomeViewModel(userId);
    }

    [RelayCommand]
    private void NavigateHome() => CurrentPage = new HomeViewModel(_userId!);

    [RelayCommand]
    private void NavigateTests() => CurrentPage = new TestsViewModel(this, _userId!);

    [RelayCommand]
    private void NavigateResults() => CurrentPage = new ChartViewModel(_userId!);

    [RelayCommand]
    private void NavigateStatistics() => CurrentPage = new ChartViewModel(_userId!);

    [RelayCommand]
    private void NavigateCreateTest() => CurrentPage = new CreateTestViewModel(this, _userId!);

    [RelayCommand]
    private void NavigateAbout() => CurrentPage = new AboutViewModel();

    [RelayCommand]
    private void Logout()
    {
        AppState.Token = null;
        IsTeacher = false;
        IsStudent = false;
        ShowLogin();
    }
}