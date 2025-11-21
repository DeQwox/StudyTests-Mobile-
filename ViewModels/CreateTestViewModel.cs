// ViewModels/CreateTestViewModel.cs
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class CreateTestViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string name = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string result = "";

    public CreateTestViewModel(MainWindowViewModel main) => _main = main;

    [RelayCommand]
    private async Task CreateAsync()
    {
        _main.IsLoading = true;
        Result = "";

        var api = new ApiService();
        var test = new Test
        {
            Name = Name,
            Password = string.IsNullOrWhiteSpace(Password) ? null : Password,
            TeacherId = AppState.CurrentUserId
        };

        var created = await api.CreateTestAsync(test);
        Result = created?.Id > 0
            ? $"Тест створено! ID = {created.Id}"
            : "Помилка створення";

        _main.IsLoading = false;
    }
}