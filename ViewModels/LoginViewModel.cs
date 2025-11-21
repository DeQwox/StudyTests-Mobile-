// ViewModels/LoginViewModel.cs
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string login = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string errorMessage = "";

    public LoginViewModel(MainWindowViewModel main) => _main = main;

    [RelayCommand]
    private async Task LoginAsync()
    {
        _main.IsLoading = true;
        ErrorMessage = "";

        var token = await new AuthService().LoginAsync(Login, Password);

        if (token != null)
        {
            _main.IsLoggedIn = true;
            _main.ShowAppropriateView();
        }
        else
        {
            ErrorMessage = "Невірний логін або пароль";
        }

        _main.IsLoading = false;
    }
}