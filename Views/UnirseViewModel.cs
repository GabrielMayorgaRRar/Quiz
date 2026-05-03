using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Quiz.ViewModels;

public partial class UnirseViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    public UnirseViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    [ObservableProperty]
    private string nombre = "";

    [ObservableProperty]
    private string apodo = "";

    [ObservableProperty]
    private string codigoSala = "";

    public bool PuedeUnirse =>
        !string.IsNullOrWhiteSpace(Nombre) &&
        !string.IsNullOrWhiteSpace(Apodo) &&
        !string.IsNullOrWhiteSpace(CodigoSala);

    partial void OnNombreChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));
    partial void OnApodoChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));
    partial void OnCodigoSalaChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));

    [RelayCommand]
    private void Unirse()
    {
        Console.WriteLine($"Uniéndose a sala: {CodigoSala}");
    }

    [RelayCommand]
    private void Volver()
    {
        _main.IrAHome();
    }
}