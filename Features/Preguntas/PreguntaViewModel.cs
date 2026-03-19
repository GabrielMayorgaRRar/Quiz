using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.ViewModels;
using System.Linq;

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
    private string _mensajeError = "";

    [ObservableProperty]
    private string _mensajeOpciones = "";

    [ObservableProperty]
    private string _mensajeExito = "";

    [ObservableProperty]
    private string _mensajeLimiteOpciones = "";

    [ObservableProperty]
    private string _mensajeMinOpciones = "";

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private Pregunta? _preguntaSeleccionada;

    public PreguntaViewModel(AppDbContext context)
    {
        _context = context;

        // inicializar opciones visibles
        OpcionesTemp.Add(new Opciones());
        OpcionesTemp.Add(new Opciones());

        _ = CargarDatosAsync();
    }

    [ObservableProperty]
    private ObservableCollection<Opciones> _opcionesTemp = new();

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
    private void MarcarCorrecta(Opciones opcionSeleccionada)
    {
        foreach (var op in OpcionesTemp)
            op.EsCorrecta = false;

        opcionSeleccionada.EsCorrecta = true;
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        MensajeExito = "";
        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            MensajeError = "Debe escribir una pregunta.";
            return;
        }

        if (CategoriaSeleccionada == null)
        {
            MensajeError = "Debe seleccionar una categoría.";
            return;
        }

        // validar duplicado
        bool existe = await _context.Preguntas.AnyAsync(p =>
            p.Enunciado.ToLower().Trim() == Enunciado.ToLower().Trim()
            && p.CategoriaId == CategoriaSeleccionada.Id);

        if (existe)
        {
            MensajeError = "Esta pregunta ya existe en esa categoría.";
            return;
        }

        if (OpcionesTemp.Count < 2)
            return;

        if (OpcionesTemp.Any(o => string.IsNullOrWhiteSpace(o.Contenido)))
        {
            MensajeOpciones = "Todas las opciones deben tener contenido.";
            return;
        }

        int correctas = OpcionesTemp.Count(o => o.EsCorrecta);

        if (correctas != 1)
        {
            MensajeOpciones = "Debe marcar una opción correcta.";
            return;
        }

        MensajeOpciones = "";
        MensajeError = "";

        // crear pregunta
        var pregunta = new Pregunta
        {
            Enunciado = Enunciado.Trim(),
            CategoriaId = CategoriaSeleccionada.Id
        };

        _context.Preguntas.Add(pregunta);
        await _context.SaveChangesAsync();

        // guardar opciones
        foreach (var op in OpcionesTemp)
        {
            op.PreguntaId = pregunta.Id;
        }

        _context.Opciones.AddRange(OpcionesTemp);

        await _context.SaveChangesAsync();

        // refrescar tabla
        var nueva = await _context.Preguntas
            .Include(p => p.Categoria)
            .FirstAsync(p => p.Id == pregunta.Id);

        Preguntas.Add(nueva);
        MensajeExito = "Pregunta guardada correctamente.";

        // limpiar formulario
        Enunciado = "";
        CategoriaSeleccionada = null;

        // limpiar lista de opciones
        OpcionesTemp.Clear();

        // volver a dejar 3 opciones iniciales
        OpcionesTemp.Add(new Opciones());
        OpcionesTemp.Add(new Opciones());
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (PreguntaSeleccionada is null) return;
        _context.Preguntas.Remove(PreguntaSeleccionada);
        await _context.SaveChangesAsync();
        Preguntas.Remove(PreguntaSeleccionada);
    }

    [RelayCommand]
    private void AgregarOpcion()
    {
        if (OpcionesTemp.Count >= 4)
        {
            MensajeLimiteOpciones = "Ya no puedes agregar más de 4 opciones.";
            return;
        }

        MensajeLimiteOpciones = "";
        OpcionesTemp.Add(new Opciones());
    }

    [RelayCommand]
    private void EliminarOpcion(Opciones opcion)
    {
        if (OpcionesTemp.Count <= 2)
        {
            MensajeMinOpciones = "Debe haber al menos 2 opciones.";
            return;
        }

        MensajeMinOpciones = "";
        OpcionesTemp.Remove(opcion);
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