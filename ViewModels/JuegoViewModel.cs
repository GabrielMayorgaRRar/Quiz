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
    public int Id { get; set; }
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
        bool esPreguntaAudio = q.MediaType?.Equals("audio", StringComparison.OrdinalIgnoreCase) == true;

        foreach (var op in q.Options)
        {
            var tipo = TipoRespuesta.Texto;
            if (esPreguntaAudio || EsUrlDeAudio(op.Content))
                tipo = TipoRespuesta.Audio;
            else if (esPreguntaImagen || EsUrlDeImagen(op.Content))
                tipo = TipoRespuesta.Imagen;

            var respuesta = new RespuestaItem
            {
                Id = op.Id,
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

    private System.Diagnostics.Process? _currentAudioProcess;

    [RelayCommand]
    private void StopAudio()
    {
        try
        {
            if (_currentAudioProcess != null && !_currentAudioProcess.HasExited)
                _currentAudioProcess.Kill();
        }
        catch { }
    }

    [RelayCommand]
    private void ReproducirAudio(string ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta)) return;
        
        string absolutePath = GetGoogleDriveUrl(ruta, false);

        if (absolutePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || absolutePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            Task.Run(async () => {
                try
                {
                    var uri = new Uri(absolutePath);
                    string tempFile = Path.Combine(Path.GetTempPath(), "quiz_audio_" + Guid.NewGuid().ToString() + ".mp3");
                    using var client = new HttpClient();
                    var response = await client.GetAsync(uri);
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    
                    if (bytes.Length > 2000) 
                    {
                        await File.WriteAllBytesAsync(tempFile, bytes);
                        ReproducirArchivo(tempFile);
                    }
                    else
                    {
                        Console.WriteLine("El archivo parece ser HTML y no el audio.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error descargando audio: {ex.Message}");
                }
            });
        }
        else
        {
            ReproducirArchivo(absolutePath);
        }
    }

    private void ReproducirArchivo(string absolutePath)
    {
        StopAudio();
        Task.Run(() => 
        {
            try
            {
                if (OperatingSystem.IsLinux())
                {
                    _currentAudioProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ffplay",
                        Arguments = $"-nodisp -autoexit \"{absolutePath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                else if (OperatingSystem.IsWindows())
                {
                    _currentAudioProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-c (New-Object System.Media.SoundPlayer '{absolutePath}').PlaySync()",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    _currentAudioProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "afplay",
                        Arguments = $"\"{absolutePath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        });
    }

    [RelayCommand]
    private async Task SeleccionarRespuesta(RespuestaItem item)
    {
        _cts?.Cancel();
        StopAudio();

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

        await Task.Delay(500);

        MostrarResultado = false;

        if (PreguntasRespondidas >= TotalPreguntas)
        {
            await FinalizarJuego();
            return;
        }

        await CargarPregunta();
    }

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

    private static string GetGoogleDriveUrl(string input, bool isImage)
    {
        if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;
        var url = input.Trim().Trim('"', '\'').Replace("\\u0026", "&");
        var match = System.Text.RegularExpressions.Regex.Match(url, @"(?:/d/|id=)([a-zA-Z0-9_-]{10,})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (match.Success)
        {
            if (isImage)
                return $"https://drive.google.com/uc?export=view&id={match.Groups[1].Value}";
            else
                return $"https://drive.google.com/uc?export=download&id={match.Groups[1].Value}&confirm=t";
        }
        return input;
    }

    private static bool EsUrlDeImagen(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido)) return false;
        var lower = contenido.ToLowerInvariant();
        return lower.Contains("imgur.com") || lower.EndsWith(".png") || lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".gif") || lower.EndsWith(".webp");
    }

    private static bool EsUrlDeAudio(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido)) return false;
        var lower = contenido.ToLowerInvariant();
        return lower.EndsWith(".mp3") || lower.EndsWith(".wav") || lower.EndsWith(".ogg") || lower.Contains("drive.google.com");
    }

    private async Task CargarImagenAsync(RespuestaItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Contenido))
            return;

        try
        {
            var url = GetGoogleDriveUrl(item.Contenido, true);
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
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

    public void Cleanup()
    {
        _ws.OnMessageReceived -= Ws_OnMessageReceived;
        _ = _ws.DisconnectAsync();
    }

    [RelayCommand]
    private void Volver()
    {
        Cleanup();
        _main.IrAHome();
    }
}