using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quiz.ViewModels;

namespace Quiz.Features.QuizSession;

public enum TipoPregunta
{
    Texto,
    Imagen,
    Audio
}

public class MockPregunta
{
    public string Enunciado { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public ObservableCollection<MockOpcion> Opciones { get; set; } = [];
}

public class MockOpcion
{
    public string Contenido { get; set; } = string.Empty;
    public bool EsCorrecta { get; set; }
}

public partial class QuizSessionViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MockPregunta> _preguntas = [];

    [ObservableProperty]
    private MockPregunta _preguntaActual = null!;

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

    public Action? OnQuizFinished;

    public QuizSessionViewModel()
    {
        CargarMocks();
        ActualizarEstadoFase();
    }

    private void CargarMocks()
    {
        Preguntas = [
            new MockPregunta 
            {
                Enunciado = "¿Qué elemento químico tiene el símbolo 'Au'?",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Plata", EsCorrecta = false },
                    new() { Contenido = "Oro", EsCorrecta = true },
                    new() { Contenido = "Cobre", EsCorrecta = false },
                    new() { Contenido = "Aluminio", EsCorrecta = false }
                ]
            },
            new MockPregunta 
            {
                Enunciado = "¿Cuál de estas maravillas arquitectónicas se encuentra en Roma?",
                Tipo = TipoPregunta.Imagen,
                Opciones = [
                    new() { Contenido = "avares://Quiz/Assets/img_1.webp", EsCorrecta = false },
                    new() { Contenido = "avares://Quiz/Assets/img_2.webp", EsCorrecta = false },
                    new() { Contenido = "avares://Quiz/Assets/img_3.webp", EsCorrecta = true },
                    new() { Contenido = "avares://Quiz/Assets/img_4.webp", EsCorrecta = false }
                ]
            },
            new MockPregunta 
            {
                Enunciado = "Identifica la obra maestra clásica que está sonando...",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Sinfonía No. 5 - Beethoven", EsCorrecta = true },
                    new() { Contenido = "Las Cuatro Estaciones - Vivaldi", EsCorrecta = false },
                    new() { Contenido = "Claro de Luna - Debussy", EsCorrecta = false },
                    new() { Contenido = "El Lago de los Cisnes - Tchaikovsky", EsCorrecta = false }
                ]
            }
        ];
    }

    private void ActualizarEstadoFase()
    {
        if (Preguntas.Count == 0) return;
        
        PreguntaActual = Preguntas[IndiceActual];
        ProgresoTexto = $"{IndiceActual + 1} / {Preguntas.Count}";
        
        EsModoTexto = PreguntaActual.Tipo == TipoPregunta.Texto;
        EsModoImagen = PreguntaActual.Tipo == TipoPregunta.Imagen;
        EsModoAudio = PreguntaActual.Tipo == TipoPregunta.Audio;
        JuegoTerminado = false;
    }

    [RelayCommand]
    private void SeleccionarOpcion(MockOpcion opcion)
    {
        // Advance to next question
        if (IndiceActual < Preguntas.Count - 1)
        {
            IndiceActual++;
            ActualizarEstadoFase();
        }
        else
        {
            // Quiz finished
            JuegoTerminado = true;
            EsModoTexto = false;
            EsModoImagen = false;
            EsModoAudio = false;
            ProgresoTexto = "COMPLETADO";
        }
    }

    [RelayCommand]
    private void ReiniciarPartida()
    {
        IndiceActual = 0;
        ActualizarEstadoFase();
    }

    [RelayCommand]
    private void FinalizarPartida()
    {
        OnQuizFinished?.Invoke();
    }
}
