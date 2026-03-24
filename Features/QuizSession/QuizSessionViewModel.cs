using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.ViewModels;

namespace Quiz.Features.QuizSession;

using Quiz.Models;

public partial class QuizSessionViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Pregunta> _preguntas = [];

    [ObservableProperty]
    private Pregunta _preguntaActual = null!;

    [ObservableProperty]
    private int _indiceActual = 0;

    [ObservableProperty]
    private string _progresoTexto = string.Empty;

    [ObservableProperty]
    private bool _esModoTexto;

    [ObservableProperty]
    private bool _esModoImagen;

    [ObservableProperty]
    private bool _esModoAudio;

    [ObservableProperty]
    private bool _juegoTerminado;

    [ObservableProperty]
    private Opciones? _opcionSeleccionada;

    [ObservableProperty]
    private bool _mostrarFeedback;

    [ObservableProperty]
    private bool _esCorrecta;

    [ObservableProperty]
    private string _mensajeFeedback = string.Empty;

    [ObservableProperty]
    private string _respuestaCorrectaTexto = string.Empty;

    [ObservableProperty]
    private int _preguntasCorrectas;

    [ObservableProperty]
    private string? _categoriaSeleccionada;

    public Action? OnQuizFinished;

    public QuizSessionViewModel(AppDbContext context)
    {
        _context = context;
    }

    async partial void OnCategoriaSeleccionadaChanged(string? value)
    {
        await CargarPreguntasPorCategoriaAsync(value);
        IndiceActual = 0;
        PreguntasCorrectas = 0;
        ActualizarEstadoFase();
    }

    private async Task CargarPreguntasPorCategoriaAsync(string? categoria)
    {
        IQueryable<Pregunta> query = _context.Preguntas
            .Include(p => p.Categoria)
            .Include(p => p.Opciones);

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(p => p.Categoria != null && p.Categoria.Nombre == categoria);
        }

        var pregList = await query.ToListAsync();
        
        var rng = new Random();
        int n = pregList.Count;
        while (n > 1) 
        {
            n--;
            int k = rng.Next(n + 1);
            var value = pregList[k];
            pregList[k] = pregList[n];
            pregList[n] = value;
        }

        Preguntas = new ObservableCollection<Pregunta>(pregList);
    }

    private void ActualizarEstadoFase()
    {
        if (Preguntas == null || Preguntas.Count == 0)
        {
            EsModoTexto = false;
            EsModoImagen = false;
            EsModoAudio = false;
            JuegoTerminado = true;
            ProgresoTexto = "Sin preguntas";
            return;
        }
        
        PreguntaActual = Preguntas[IndiceActual];
        ProgresoTexto = $"{IndiceActual + 1} / {Preguntas.Count}";
        
        var tipoRespuesta = TipoRespuesta.Texto;
        if (PreguntaActual.Opciones != null && PreguntaActual.Opciones.Any())
        {
            var primeraOpcion = PreguntaActual.Opciones.First();
            tipoRespuesta = primeraOpcion.TipoRespuesta;
            
            if (tipoRespuesta == TipoRespuesta.Texto && !string.IsNullOrWhiteSpace(primeraOpcion.Contenido))
            {
                var content = primeraOpcion.Contenido.Trim().ToLowerInvariant();
                if (content.EndsWith(".jpg") || content.EndsWith(".jpeg") || content.EndsWith(".png") || content.EndsWith(".webp") || content.EndsWith(".gif"))
                {
                    tipoRespuesta = TipoRespuesta.Imagen;
                }
            }
        }

        EsModoTexto = tipoRespuesta == TipoRespuesta.Texto;
        EsModoImagen = tipoRespuesta == TipoRespuesta.Imagen;
        EsModoAudio = tipoRespuesta == TipoRespuesta.Audio;
        JuegoTerminado = false;
        MostrarFeedback = false;
        
        StopAudio();
    }

    private System.Diagnostics.Process? _currentAudioProcess;

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
    private void PlayAudio(string? audioPath)
    {
        if (string.IsNullOrWhiteSpace(audioPath)) return;
        
        string absolutePath = audioPath;
        if (!System.IO.Path.IsPathRooted(absolutePath))
        {
            absolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioPath.TrimStart('/'));
        }

        if (!System.IO.File.Exists(absolutePath))
        {
            System.Diagnostics.Debug.WriteLine($"Audio no encontrado en: {absolutePath}");
            return;
        }

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
                System.Diagnostics.Debug.WriteLine($"Error playing audio: {ex.Message}");
            }
        });
    }
    [RelayCommand]
    private void SeleccionarOpcion(Opciones opcion)
    {
        StopAudio();
        OpcionSeleccionada = opcion;
        
        EsCorrecta = opcion.EsCorrecta;
        
        if (opcion.EsCorrecta)
        {
            MensajeFeedback = "¡Correcto!";
            PreguntasCorrectas++;
        }
        else
        {
            var respuestaCorrecta = PreguntaActual.Opciones?.FirstOrDefault(o => o.EsCorrecta);
            var contenido = respuestaCorrecta?.Contenido ?? "No especificada";
            
            if (EsModoImagen || EsModoAudio)
            {
                try { contenido = System.IO.Path.GetFileNameWithoutExtension(contenido); } catch { }
            }

            RespuestaCorrectaTexto = contenido;
            MensajeFeedback = $"Incorrecto. La respuesta correcta era: {RespuestaCorrectaTexto}";
        }
        
        MostrarFeedback = true;
        
        Task.Delay(2000).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                MostrarFeedback = false;
                
                if (IndiceActual < Preguntas.Count - 1)
                {
                    IndiceActual++;
                    ActualizarEstadoFase();
                }
                else
                {
                    JuegoTerminado = true;
                    EsModoTexto = false;
                    EsModoImagen = false;
                    EsModoAudio = false;
                    ProgresoTexto = "COMPLETADO";
                }
            });
        });
    }

    [RelayCommand]
    private void ReiniciarPartida()
    {
        IndiceActual = 0;
        PreguntasCorrectas = 0;
        ActualizarEstadoFase();
    }

    [RelayCommand]
    private void FinalizarPartida()
    {
        StopAudio();
        OnQuizFinished?.Invoke();
    }
}
