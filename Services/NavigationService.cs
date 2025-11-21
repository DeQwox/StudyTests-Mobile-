using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using OnlineTestingClient.ViewModels;

namespace OnlineTestingClient.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    Control CurrentView { get; }
}

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, ViewModelBase> _vmFactory;
    private Control? _currentView;

    public Control CurrentView
    {
        get => _currentView!;
        private set => SetProperty(ref _currentView, value);
    }

    public NavigationService(Func<Type, ViewModelBase> vmFactory) => _vmFactory = vmFactory;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var vm = _vmFactory(typeof(TViewModel));
        var view = new ViewLocator().Build(vm) as Control;
        if (view != null)
            CurrentView = view;
    }
}