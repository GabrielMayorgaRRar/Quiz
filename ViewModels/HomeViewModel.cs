using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quiz.ViewModels;

namespace Quiz.Features.Home;

public partial class HomeViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main = null!;

    public HomeViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    [RelayCommand]
    private void CrearSala()
    {
        _main.IrACrearSala();
    }

    [RelayCommand]
    private void Unirse()
    {
        _main.IrAUnirse();
    }
}