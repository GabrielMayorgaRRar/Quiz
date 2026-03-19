using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.Features.Preguntas;

public partial class PreguntaViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Pregunta> _preguntas = [];

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = [];

    [ObservableProperty]
    private string _enunciado = string.Empty;

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private Pregunta? _preguntaSeleccionada;

    public PreguntaViewModel(AppDbContext context)
    {
        _context = context;

        _ = CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        var preguntas = await _context.Preguntas
            .Include(p => p.Categoria)
            .ToListAsync();

        Preguntas = new ObservableCollection<Pregunta>(preguntas);

        var categorias = await _context.Categorias.ToListAsync();
        Categorias = new ObservableCollection<Categoria>(categorias);
    }

    [RelayCommand]
    //Agregar nueva pregunta ya funciona modifique "AgregarAsync" para que se ejecute en el click del boton
    private async Task AgregarAsync()
    {
        if (string.IsNullOrWhiteSpace(Enunciado) || CategoriaSeleccionada == null)
            return;

        var pregunta = new Pregunta
        {
            Enunciado = Enunciado,
            CategoriaId = CategoriaSeleccionada.Id
        };

        _context.Preguntas.Add(pregunta);

        await _context.SaveChangesAsync();

        // volver a cargar con categoria incluida
        var nueva = await _context.Preguntas
            .Include(p => p.Categoria)
            .FirstAsync(p => p.Id == pregunta.Id);

        Preguntas.Add(nueva);

        Enunciado = "";
        CategoriaSeleccionada = null;
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (PreguntaSeleccionada is null) return;
        _context.Preguntas.Remove(PreguntaSeleccionada);
        await _context.SaveChangesAsync();
        Preguntas.Remove(PreguntaSeleccionada);
    }
}

// Si por alguna razón borraron datos en la BD, solo ejecuten
/*
SELECT setval(
    pg_get_serial_sequence('"Preguntas"', 'Id'),
    (SELECT MAX("Id") FROM "Preguntas")
);

no le se mucho a PGadmin pero eso reinicia la secuencia
*/