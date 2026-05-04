using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Quiz.ViewModels;

public enum TipoRespuesta
{
    Texto,
    Imagen,
    Audio
}

public class RespuestaItem
{
    public string Contenido { get; set; } = "";
    public TipoRespuesta Tipo { get; set; }
}

public partial class JuegoViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;
    private readonly int _gameId;

    private string _respuestaCorrecta = "";
    private CancellationTokenSource? _cts;

    public JuegoViewModel(MainWindowViewModel main, int gameId)
    {
        _main = main;
        _gameId = gameId;

        _ = CargarPregunta();
    }

    // =========================
    // 🔹 PROPIEDADES
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
    private string colorResultado = "#333"; // default

    // =========================
    // 🌐 API
    // =========================

    private async Task CargarPregunta()
    {
        var q = await _main.ApiService.GetQuestionAsync(_gameId);

        if (q == null)
        {
            Console.WriteLine("No llegó pregunta");
            return;
        }

        Console.WriteLine($"✅ Pregunta: {q.Question}");

        Pregunta = q.Question;

        Respuestas.Clear();

        foreach (var op in q.Options)
        {
            Respuestas.Add(new RespuestaItem
            {
                Contenido = op.Content,
                Tipo = TipoRespuesta.Texto
            });

            if (op.IsCorrect)
                _respuestaCorrecta = op.Content;
        }

        IniciarTimer();
    }

    private TipoRespuesta DetectarTipo(string contenido)
    {
        if (contenido.EndsWith(".jpg") || contenido.EndsWith(".png"))
            return TipoRespuesta.Imagen;

        if (contenido.EndsWith(".mp3") || contenido.EndsWith(".wav"))
            return TipoRespuesta.Audio;

        return TipoRespuesta.Texto;
    }

    // =========================
    // ⏱️ TIMER
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
                await CargarPregunta(); // tiempo agotado → siguiente
            }
        }
        catch (TaskCanceledException) { }
    }

    // =========================
    // 🔊 AUDIO
    // =========================

    [RelayCommand]
    private async Task ReproducirAudio(string ruta)
    {
        if (!PuedeReproducirAudio)
            return;

        PuedeReproducirAudio = false;

        Console.WriteLine($"Reproduciendo audio: {ruta}");
        await Task.Delay(3000);
        Console.WriteLine("Audio detenido");

        PuedeReproducirAudio = true;
    }

    // =========================
    // 🎯 RESPUESTA
    // =========================

    [RelayCommand]
    private async Task SeleccionarRespuesta(RespuestaItem item)
    {
        if (item.Tipo == TipoRespuesta.Audio)
        {
            await ReproducirAudio(item.Contenido);
            return;
        }

        bool correcta = item.Contenido == _respuestaCorrecta;

        MensajeResultado = correcta ? "✔ Correcto" : "❌ Incorrecto";

        // 🔥 COLOR AQUÍ
        ColorResultado = correcta ? "#4CAF50" : "#F44336";

        MostrarResultado = true;

        await Task.Delay(1500);

        MostrarResultado = false;

        await CargarPregunta();
    }

    // =========================
    // 🔙 NAV
    // =========================

    [RelayCommand]
    private void Volver()
    {
        _cts?.Cancel();
        _main.IrAHome();
    }
}