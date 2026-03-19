using System;
using CommunityToolkit.Mvvm.Input;
using Quiz.ViewModels;

namespace Quiz.Features.Home;

public partial class HomeViewModel : ViewModelBase
{
    public Action? OnStartQuizRequested;

    [RelayCommand]
    private void IniciarPartida()
    {
        OnStartQuizRequested?.Invoke();
    }
}
