using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Quiz.Services;

namespace Quiz.ViewModels;

public enum TipoRespuesta
{
    Texto,
    Imagen,
    Audio
}

public partial class RespuestaItem : ObservableObject
{
    public int Id { get; set; } // 🔥 ESTE ES EL QUE TE FALTA O ESTÁ MAL

    public string Contenido { get; set; } = "";
    public TipoRespuesta Tipo { get; set; }

    [ObservableProperty]
    private Bitmap? imagen;

    [ObservableProperty]
    private string background = "#444";
}

public class JugadorStats
{
    public string Nombre { get; set; } = "";
    public int Puntos { get; set; }
}

public partial class JuegoViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;
    private readonly int _gameId;
    private readonly int _departureId;
    private readonly int _categoryId;
    private readonly WsClient _ws = new();
    private static readonly HttpClient _httpClient = new();

    private const int TotalPreguntas = 12;
    private string _respuestaCorrecta = "";
    private int _questionId;
    private CancellationTokenSource? _cts;

    public JuegoViewModel(MainWindowViewModel main, int gameId, int departureId, int categoryId = 0)
    {
        _main = main;
        _gameId = gameId;
        _departureId = departureId;
        _categoryId = categoryId;

        Console.WriteLine($"WS KEY: {_main.SalaVM.Codigo}");

        _ws.OnMessageReceived += Ws_OnMessageReceived;
        _ = _ws.ConnectAsync(_main.SalaVM.Codigo);

        _ = CargarPregunta();
    }

    public int Id { get; set; }

    [ObservableProperty]
    private int preguntasRespondidas = 0;

    public int PreguntaActual => Math.Min(PreguntasRespondidas + 1, 12);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreguntaActual))]
    private bool juegoTerminado = false;

    // =========================
    // PROPIEDADES
    // =========================

    [ObservableProperty]
    private string pregunta = "";

    public ObservableCollection<RespuestaItem> Respuestas { get; } = new();

    [ObservableProperty]
    private int tiempoRestante = 15;

    [ObservableProperty]
    private bool puedeReproducirAudio = true;

    [ObservableProperty]
    private string mensajeResultado = "";

    [ObservableProperty]
    private bool mostrarResultado = false;

    [ObservableProperty]
    private string colorResultado = "#333";

    [ObservableProperty]
    private bool mostrarEstadisticas = false;

    public ObservableCollection<JugadorStats> Estadisticas { get; } = new();

    // =========================
    // API
    // =========================

    private async Task CargarPregunta()
    {
        OnPropertyChanged(nameof(PreguntaActual));

        if (PreguntasRespondidas >= TotalPreguntas)
        {
            await FinalizarJuego();
            return;
        }

        var q = await _main.ApiService.GetQuestionAsync(_gameId, _categoryId);

        if (q == null)
        {
            Console.WriteLine("No llegó pregunta");
            return;
        }

        _questionId = q.Id;

        Pregunta = q.Question;

        Respuestas.Clear();

        bool esPreguntaImagen = q.MediaType?.Equals("image", StringComparison.OrdinalIgnoreCase) == true;

        foreach (var op in q.Options)
        {
            var tipo = esPreguntaImagen || EsUrlDeImagen(op.Content)
                ? TipoRespuesta.Imagen
                : TipoRespuesta.Texto;

            var respuesta = new RespuestaItem
            {
                Id = op.Id, // 🔥 ESTA LÍNEA ES CLAVE
                Contenido = op.Content,
                Tipo = tipo,
                Background = "#444"
            };

            Respuestas.Add(respuesta);

            if (tipo == TipoRespuesta.Imagen)
            {
                _ = CargarImagenAsync(respuesta);
            }

            if (op.IsCorrect)
                _respuestaCorrecta = op.Content;
        }

        IniciarTimer();
    }

    // =========================
    // TIMER
    // =========================

    private async void IniciarTimer()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        TiempoRestante = 15;

        try
        {
            while (TiempoRestante > 0 && !token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);
                TiempoRestante--;
            }

            if (!token.IsCancellationRequested)
            {
                await PasarASiguientePregunta();
            }
        }
        catch { }
    }

    // =========================
    // AUDIO
    // =========================

    [RelayCommand]
    private async Task ReproducirAudio(string ruta)
    {
        if (!PuedeReproducirAudio) return;

        PuedeReproducirAudio = false;
        await Task.Delay(3000);
        PuedeReproducirAudio = true;
    }

    // =========================
    // RESPUESTA
    // =========================

    [RelayCommand]
    private async Task SeleccionarRespuesta(RespuestaItem item)
    {
        _cts?.Cancel();

        if (item.Tipo == TipoRespuesta.Audio)
        {
            await ReproducirAudio(item.Contenido);
            return;
        }

        bool correcta = item.Contenido == _respuestaCorrecta;

        MensajeResultado = correcta ? "Correcto" : "Incorrecto";
        ColorResultado = correcta ? "#4CAF50" : "#F44336";

        MostrarResultado = true;

        foreach (var r in Respuestas)
        {
            if (r.Contenido == _respuestaCorrecta)
                r.Background = "#4CAF50";
            else if (r == item)
                r.Background = "#F44336";
            else
                r.Background = "#555";
        }

        var req = new SubmitAnswerRequest
        {
            DepartureId = _departureId,
            QuestionId = _questionId,
            AnswerId = item.Id,
            ResponseTime = 15 - TiempoRestante,
            GameKey = _main.SalaVM.Codigo
        };

        await _main.ApiService.EnviarRespuestaAsync(req);
        
        PreguntasRespondidas++;
        OnPropertyChanged(nameof(PreguntaActual));

        await Task.Delay(500); // Tarda menos, de 1500 a 500

        MostrarResultado = false;

        if (PreguntasRespondidas >= TotalPreguntas)
        {
            await FinalizarJuego();
            return;
        }

        await CargarPregunta();
    }

    // =========================
    // WEBSOCKET & SCOREBOARD
    // =========================

    private async Task ObtenerScoreboardFinal()
    {
        var scoreboard = await _main.ApiService.GetScoreboardAsync(_gameId);
        Dispatcher.UIThread.Post(() =>
        {
            Estadisticas.Clear();
            foreach (var item in scoreboard)
            {
                Estadisticas.Add(new JugadorStats
                {
                    Nombre = item.User?.Nickname ?? "",
                    Puntos = (int)item.Score
                });
            }
        });
    }

    private async Task FinalizarJuego()
    {
        await ObtenerScoreboardFinal();
        JuegoTerminado = true;
        MostrarEstadisticas = true;
    }

    private static bool EsUrlDeImagen(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            return false;

        if (Uri.TryCreate(contenido, UriKind.Absolute, out var uri))
        {
            var lower = contenido.ToLowerInvariant();
            if (lower.Contains("drive.google.com") || lower.Contains("docs.google.com") || lower.Contains("imgur.com")
                || lower.EndsWith(".png") || lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".gif")
                || lower.EndsWith(".webp"))
            {
                return true;
            }
        }

        return false;
    }

    private async Task CargarImagenAsync(RespuestaItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Contenido))
            return;

        try
        {
            using var response = await _httpClient.GetAsync(item.Contenido, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var ms = new MemoryStream();
            await response.Content.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmap = new Bitmap(ms);
            Dispatcher.UIThread.Post(() => item.Imagen = bitmap);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando imagen: {ex.Message}");
        }
    }

    private void Ws_OnMessageReceived(string eventName, JsonElement data)
    {
        Console.WriteLine($"Evento recibido: {eventName}");

        if (eventName == "score_update")
        {
            Dispatcher.UIThread.Post(() =>
            {
                Estadisticas.Clear();

                foreach (var item in data.EnumerateArray())
                {
                    var nombre = item.GetProperty("user").GetProperty("nickname").GetString();
                    var puntos = item.GetProperty("score").GetInt32();

                    Estadisticas.Add(new JugadorStats
                    {
                        Nombre = nombre ?? "",
                        Puntos = puntos
                    });
                }
            });
        }
    }

    // =========================
    // FLUJO CENTRAL
    // =========================

    private async Task PasarASiguientePregunta()
    {
        MostrarResultado = false;
        MostrarEstadisticas = false;
        
        PreguntasRespondidas++;
        OnPropertyChanged(nameof(PreguntaActual));

        if (PreguntasRespondidas >= TotalPreguntas)
        {
            await FinalizarJuego();
            return;
        }

        await CargarPregunta();
    }

    // =========================
    // CLEANUP
    // =========================

    public void Cleanup()
    {
        _ws.OnMessageReceived -= Ws_OnMessageReceived;
        _ = _ws.DisconnectAsync();
    }

    // =========================
    // NAV
    // =========================

    [RelayCommand]
    private void Volver()
    {
        Cleanup();
        _main.IrAHome();
    }
}