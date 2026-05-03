using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Quiz.ViewModels;

public partial class SalaViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    public SalaViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    [ObservableProperty]
    private string codigo = "";

    [ObservableProperty]
    private bool esOwner;

    public ObservableCollection<string> Jugadores { get; } = new();

    public bool PuedeIniciar => EsOwner;

    public void Inicializar(string codigoSala, string jugador, bool owner)
    {
        Codigo = codigoSala;
        EsOwner = owner;

        Jugadores.Clear();
        Jugadores.Add(jugador);
    }

    [RelayCommand]
    private void Iniciar()
    {
        Console.WriteLine("Juego iniciado");
    }

    [RelayCommand]
    private void Volver()
    {
        _main.IrAHome();
    }
}