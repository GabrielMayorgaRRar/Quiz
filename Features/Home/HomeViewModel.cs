using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Quiz.ViewModels;

namespace Quiz.Features.Home;

public partial class HomeViewModel : ViewModelBase
{
    public Action<string>? OnStartQuizWithCategory;

    [RelayCommand]
    private void IniciarPartida()
    {
        OnStartQuizWithCategory?.Invoke(null);
    }

    [RelayCommand]
    private void IniciarPartidaConCategoria(string categoria)
    {
        OnStartQuizWithCategory?.Invoke(categoria);
    }
}
