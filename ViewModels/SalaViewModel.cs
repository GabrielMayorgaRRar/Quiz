using System.Threading;
using System.Threading.Tasks;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Quiz.Services;
using System.Text.Json;

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
    
    private int _gameId;
    public WsClient Ws { get; } = new();

    public ObservableCollection<string> Jugadores { get; } = new();

    public bool PuedeIniciar => EsOwner;

    public void Inicializar(string codigoSala, int gameId, string jugador, bool owner)
    {
        Codigo = codigoSala;
        _gameId = gameId;
        EsOwner = owner;

        Jugadores.Clear();
        Jugadores.Add(jugador);

        Ws.OnMessageReceived += Ws_OnMessageReceived;
        _ = Ws.ConnectAsync(Codigo);

        IniciarAnimacion();
    }

    private void Ws_OnMessageReceived(string eventName, JsonElement data)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (eventName == "player_joined")
            {
                if (data.TryGetProperty("user", out var user) && user.TryGetProperty("nickname", out var nickname))
                {
                    Jugadores.Add(nickname.GetString() ?? "");
                }
            }
            else if (eventName == "game_started")
            {
                Console.WriteLine("El admin inició el juego.");
            }
        });
    }

    [RelayCommand]
    private async Task Iniciar()
    {
        _main.IrAJuego();
    }

    [RelayCommand]
    private void Volver()
    {
        _ = Ws.DisconnectAsync();
        Ws.OnMessageReceived -= Ws_OnMessageReceived;
        DetenerAnimacion();
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