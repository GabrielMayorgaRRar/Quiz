using System.Threading;
using System.Threading.Tasks;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Quiz.Services;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace Quiz.ViewModels;

public partial class SalaViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    public SalaViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    public ObservableCollection<string> Jugadores { get; } = new();

    [ObservableProperty]
    private string codigo = "";

    [ObservableProperty]
    private bool esOwner;

    [ObservableProperty]
    private string textoEstado = "Buscando jugadores";

    [ObservableProperty]
    private string textoCopiar = "COPIAR";


    private int _gameId;
    public WsClient Ws { get; } = new();

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
                if (data.TryGetProperty("user", out var user) &&
                    user.TryGetProperty("nickname", out var nickname))
                {
                    var name = nickname.GetString() ?? "";

                    // 🔥 evita duplicados
                    if (!Jugadores.Contains(name))
                    {
                        Jugadores.Add(name);
                    }
                }
            }
        });
    }

    [RelayCommand]
    private async Task Iniciar()
    {
        var ok = await _main.ApiService.StartGameAsync(_gameId);

        if (!ok)
        {
            Console.WriteLine("Error al iniciar juego");
            return;
        }

        _main.IrAJuego(_gameId);
    }
    [RelayCommand]
    private void Volver()
    {
        _ = Ws.DisconnectAsync();
        Ws.OnMessageReceived -= Ws_OnMessageReceived;
        DetenerAnimacion();
        _main.IrAHome();
    }

    [RelayCommand]
    private async Task CopiarCodigo()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow?.Clipboard is { } clipboard)
        {
            await clipboard.SetTextAsync(Codigo);

            TextoCopiar = "COPIADO ✔";
            await Task.Delay(1500);
            TextoCopiar = "COPIAR";
        }
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