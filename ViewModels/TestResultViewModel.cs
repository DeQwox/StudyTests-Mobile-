using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;

namespace OnlineTestingClient.ViewModels;

public partial class TestResultViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _owner;
    private readonly string _userId;

    [ObservableProperty]
    private PassedTest? result;

    [ObservableProperty]
    private bool isSuccess;

    [ObservableProperty]
    private string statusMessage = "";

    [ObservableProperty]
    private string statusColor = "Black"; // Default color

    public TestResultViewModel(MainWindowViewModel owner, string userId, PassedTest? result)
    {
        _owner = owner;
        _userId = userId;
        if (result != null)
        {
            Result = result;
            IsSuccess = true;
            StatusMessage = "Операція успішна!";
            StatusColor = "#16A34A"; // Green
        }
        else
        {
            Result = null;
            IsSuccess = false;
            StatusMessage = "Запит не успішний. Спробуйте пізніше.";
            StatusColor = "#DC2626"; // Red
        }
    }

    [RelayCommand]
    private void GoToTests()
    {
        _owner.CurrentPage = new TestsViewModel(_owner, _userId);
    }

    [RelayCommand]
    private void GoHome()
    {
        _owner.CurrentPage = new HomeViewModel(_userId);
    }
}
