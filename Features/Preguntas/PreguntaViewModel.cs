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
    // Propiedades para la pregunta seleccionada y su respuesta
    [ObservableProperty] private string _opcionA = "";
    [ObservableProperty] private string _opcionB = "";
    [ObservableProperty] private string _opcionC = "";

    private bool _aCorrecta;
public bool ACorrecta
{
    get => _aCorrecta;
    set
    {
        SetProperty(ref _aCorrecta, value);
        if (value)
        {
            BCorrecta = false;
            CCorrecta = false;
        }
    }
}

private bool _bCorrecta;
public bool BCorrecta
{
    get => _bCorrecta;
    set
    {
        SetProperty(ref _bCorrecta, value);
        if (value)
        {
            ACorrecta = false;
            CCorrecta = false;
        }
    }
}

private bool _cCorrecta;
public bool CCorrecta
{
    get => _cCorrecta;
    set
    {
        SetProperty(ref _cCorrecta, value);
        if (value)
        {
            ACorrecta = false;
            BCorrecta = false;
        }
    }
}

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
private async Task AgregarAsync()
{
    if (string.IsNullOrWhiteSpace(Enunciado) || CategoriaSeleccionada == null)
        return;

    // validar duplicado
    bool existe = await _context.Preguntas.AnyAsync(p =>
        p.Enunciado.ToLower().Trim() == Enunciado.ToLower().Trim()
        && p.CategoriaId == CategoriaSeleccionada.Id);

    if (existe)
        return;

    // validar opciones
    if (string.IsNullOrWhiteSpace(OpcionA) ||
        string.IsNullOrWhiteSpace(OpcionB) ||
        string.IsNullOrWhiteSpace(OpcionC))
        return;

    // validar solo una correcta
    int correctas = 0;
    if (ACorrecta) correctas++;
    if (BCorrecta) correctas++;
    if (CCorrecta) correctas++;

    if (correctas != 1)
        return;

    // crear pregunta
    var pregunta = new Pregunta
    {
        Enunciado = Enunciado.Trim(),
        CategoriaId = CategoriaSeleccionada.Id
    };

    _context.Preguntas.Add(pregunta);
    await _context.SaveChangesAsync();

    // guardar opciones
    _context.Opciones.AddRange(
        new Opciones { Contenido = OpcionA, EsCorrecta = ACorrecta, PreguntaId = pregunta.Id },
        new Opciones { Contenido = OpcionB, EsCorrecta = BCorrecta, PreguntaId = pregunta.Id },
        new Opciones { Contenido = OpcionC, EsCorrecta = CCorrecta, PreguntaId = pregunta.Id }
    );

    await _context.SaveChangesAsync();

    // refrescar tabla
    var nueva = await _context.Preguntas
        .Include(p => p.Categoria)
        .FirstAsync(p => p.Id == pregunta.Id);

    Preguntas.Add(nueva);

    // limpiar formulario
    Enunciado = "";
    CategoriaSeleccionada = null;

    OpcionA = "";
    OpcionB = "";
    OpcionC = "";

    ACorrecta = false;
    BCorrecta = false;
    CCorrecta = false;
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