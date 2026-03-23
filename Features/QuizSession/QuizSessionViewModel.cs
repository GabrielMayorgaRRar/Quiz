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

    // NUEVA PROPIEDAD: Categoría seleccionada desde el menú
    [ObservableProperty]
    private string? _categoriaSeleccionada;

    public Action? OnQuizFinished;

    public QuizSessionViewModel(AppDbContext context)
    {
        _context = context;
        // No cargar nada aquí, se carga cuando se establece la categoría
    }

    // Cuando cambia la categoría, cargamos las preguntas
    async partial void OnCategoriaSeleccionadaChanged(string? value)
    {
        await CargarPreguntasPorCategoriaAsync(value);
        IndiceActual = 0;
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
        
        // Shuffle the questions for better gameplay
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
            tipoRespuesta = PreguntaActual.Opciones.First().TipoRespuesta;
        }

        EsModoTexto = tipoRespuesta == TipoRespuesta.Texto;
        EsModoImagen = tipoRespuesta == TipoRespuesta.Imagen;
        EsModoAudio = tipoRespuesta == TipoRespuesta.Audio;
        JuegoTerminado = false;
        MostrarFeedback = false;
    }
    [RelayCommand]
    private void SeleccionarOpcion(Opciones opcion)
    {
        OpcionSeleccionada = opcion;
        
        // Verificar si la respuesta es correcta
        EsCorrecta = opcion.EsCorrecta;
        
        if (opcion.EsCorrecta)
        {
            MensajeFeedback = "¡Correcto!";
        }
        else
        {
            // Buscar cuál era la respuesta correcta
            var respuestaCorrecta = PreguntaActual.Opciones?.FirstOrDefault(o => o.EsCorrecta);
            RespuestaCorrectaTexto = respuestaCorrecta?.Contenido ?? "No especificada";
            MensajeFeedback = $"Incorrecto. La respuesta correcta era: {RespuestaCorrectaTexto}";
        }
        
        MostrarFeedback = true;
        
        // Esperar 2 segundos para mostrar el feedback antes de avanzar
        Task.Delay(2000).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                MostrarFeedback = false;
                
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
            });
        });
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
