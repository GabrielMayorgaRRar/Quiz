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
    }

    private async Task CargarDatosAsync()
    {
        var preguntas = await _context.Preguntas.ToListAsync();
        Preguntas = new ObservableCollection<Pregunta>(preguntas);

        var categorias = await _context.Categorias.ToListAsync();
        Categorias = new ObservableCollection<Categoria>(categorias);
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        if (string.IsNullOrWhiteSpace(Enunciado) || CategoriaSeleccionada is null) return;
        var pregunta = new Pregunta { Enunciado = Enunciado, CategoriaId = CategoriaSeleccionada.Id };
        _context.Preguntas.Add(pregunta);
        await _context.SaveChangesAsync();
        Preguntas.Add(pregunta);
        Enunciado = string.Empty;
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
