using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;

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

public class PreguntaItem
{
    public string Texto { get; set; } = "";
    public List<RespuestaItem> Opciones { get; set; } = new();
}



public partial class JuegoViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    // 🔥 AQUÍ VAN (CORREGIDO)
    private List<PreguntaItem> _preguntas = new();
    private int _indicePregunta = 0;

    public JuegoViewModel(MainWindowViewModel main)
    {
        _main = main;
        CargarPreguntaDemo();
    }

    // =========================
    // 🔹 PROPIEDADES
    // =========================

    [ObservableProperty]
    private string pregunta = "";

    [ObservableProperty]
    private int tiempoRestante = 15;

    [ObservableProperty]
    private bool puedeReproducirAudio = true;

    public ObservableCollection<RespuestaItem> Respuestas { get; } = new();

    private CancellationTokenSource? _cts;

    // =========================
    // 🔥 PREGUNTAS (3)
    // =========================

    public void CargarPreguntaDemo()
    {
        _preguntas = new List<PreguntaItem>
        {
            new PreguntaItem
            {
                Texto = "¿Cuándo fue el tratado de Versalles?",
                Opciones = new List<RespuestaItem>
                {
                    new() { Contenido = "1919", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "1925", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "avares://Quiz/Assets/sample.jpg", Tipo = TipoRespuesta.Imagen },
                    new() { Contenido = "audio.mp3", Tipo = TipoRespuesta.Audio }
                }
            },

            new PreguntaItem
            {
                Texto = "¿Capital de Francia?",
                Opciones = new List<RespuestaItem>
                {
                    new() { Contenido = "Madrid", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "París", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "Roma", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "Berlín", Tipo = TipoRespuesta.Texto }
                }
            },

            new PreguntaItem
            {
                Texto = "¿Cuánto es 2 + 2?",
                Opciones = new List<RespuestaItem>
                {
                    new() { Contenido = "3", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "4", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "5", Tipo = TipoRespuesta.Texto },
                    new() { Contenido = "22", Tipo = TipoRespuesta.Texto }
                }
            }
        };

        _indicePregunta = 0;
        CargarPreguntaActual();
    }

    private void CargarPreguntaActual()
    {
        var preguntaActual = _preguntas[_indicePregunta];

        Pregunta = preguntaActual.Texto;

        Respuestas.Clear();
        foreach (var r in preguntaActual.Opciones)
            Respuestas.Add(r);

        IniciarTimer();
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

        Console.WriteLine($"Seleccionaste: {item.Contenido}");

        await Task.Delay(800);

        _indicePregunta++;

        if (_indicePregunta < _preguntas.Count)
        {
            CargarPreguntaActual();
        }
        else
        {
            Console.WriteLine("Fin del juego");
        }
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