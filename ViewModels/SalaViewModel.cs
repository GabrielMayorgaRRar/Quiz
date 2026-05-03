using System.Threading;
using System.Threading.Tasks;
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

    [ObservableProperty]
    private string textoEstado = "Buscando jugadores";

    public ObservableCollection<string> Jugadores { get; } = new();

    public bool PuedeIniciar => EsOwner;

    public void Inicializar(string codigoSala, string jugador, bool owner)
    {
        Codigo = codigoSala;
        EsOwner = owner;

        Jugadores.Clear();
        Jugadores.Add(jugador);

        IniciarAnimacion();
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

    private CancellationTokenSource? _cts;

    public void IniciarAnimacion()
    {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        Task.Run(async () =>
        {
            int dots = 0;

            while (!token.IsCancellationRequested)
            {
                dots = (dots + 1) % 4;

                TextoEstado = "Buscando jugadores" + new string('.', dots);

                await Task.Delay(500);
            }

        }, token);
    }

    public void DetenerAnimacion()
    {
        _cts?.Cancel();
    }
}