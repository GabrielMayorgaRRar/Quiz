using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;

namespace Quiz.Features.Preguntas;

// Enumeración para tipos de respuesta
public enum TipoRespuesta
{
    Texto,
    Imagen,
    Audio
}

// Modelo de opción de respuesta para la UI
public partial class OpcionRespuestaUI : ObservableObject
{
    [ObservableProperty]
    private string _contenido = string.Empty;

    [ObservableProperty]
    private bool _esCorrecta;

    [ObservableProperty]
    private TipoRespuesta _tipoRespuesta = TipoRespuesta.Texto;

    public string TipoWatermark => TipoRespuesta switch
    {
        TipoRespuesta.Texto => "Texto de la respuesta...",
        TipoRespuesta.Imagen => "Ruta de la imagen (ej: /Assets/imagen.jpg)",
        TipoRespuesta.Audio => "Ruta del audio (ej: /Assets/sonido.mp3)",
        _ => "Contenido de la respuesta..."
    };
}

// ViewModel principal
public partial class PreguntaViewModel : ObservableObject
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private string _enunciado = string.Empty;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private string _mensajeOpciones = string.Empty;

    [ObservableProperty]
    private string _mensajeExito = string.Empty;

    [ObservableProperty]
    private string _mensajeLimiteOpciones = string.Empty;

    [ObservableProperty]
    private string _mensajeMinOpciones = string.Empty;

    [ObservableProperty]
    private string _mensajeEliminar = string.Empty;

    [ObservableProperty]
    private string _mensajeEliminarError = string.Empty;

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private ObservableCollection<OpcionRespuestaUI> _opciones = new();

    // Usar directamente Quiz.Models.Opciones
    [ObservableProperty]
    private ObservableCollection<Opciones> _opcionesTemp = new();

    [ObservableProperty]
    private ObservableCollection<Pregunta> _preguntas = new();

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = new();

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
    private async Task GuardarPreguntaAsync()
    {
        MensajeExito = "";

        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            MensajeError = "Debe escribir una pregunta.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        if (CategoriaSeleccionada == null)
        {
            MensajeError = "Debe seleccionar una categoría.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        // validar duplicado
        bool existe = await _context.Preguntas.AnyAsync(p =>
            p.Enunciado.ToLower().Trim() == Enunciado.ToLower().Trim()
            && p.CategoriaId == CategoriaSeleccionada.Id);

        if (existe)
        {
            MensajeError = "Esta pregunta ya existe en esa categoría.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        if (OpcionesTemp.Count < 2)
        {
            MensajeMinOpciones = "Debe haber al menos 2 opciones.";
            _ = LimpiarMensajeAsync(() => MensajeMinOpciones = "");
            return;
        }

        if (OpcionesTemp.Any(o => string.IsNullOrWhiteSpace(o.Contenido)))
        {
            MensajeOpciones = "Todas las opciones deben tener contenido.";
            _ = LimpiarMensajeAsync(() => MensajeOpciones = "");
            return;
        }

        var repetidas = OpcionesTemp
            .GroupBy(o => o.Contenido.Trim().ToLower())
            .Any(g => g.Count() > 1);

        if (repetidas)
        {
            MensajeOpciones = "No puede haber opciones de respuesta iguales.";
            _ = LimpiarMensajeAsync(() => MensajeOpciones = "");
            return;
        }

        int correctas = OpcionesTemp.Count(o => o.EsCorrecta);

        if (correctas != 1)
        {
            MensajeOpciones = "Debe marcar una opción correcta.";
            _ = LimpiarMensajeAsync(() => MensajeOpciones = "");
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
        _ = LimpiarMensajeAsync(() => MensajeExito = "");

        // limpiar formulario
        Enunciado = "";
        CategoriaSeleccionada = null;

        // limpiar lista de opciones
        OpcionesTemp.Clear();

        // volver a dejar 2 opciones iniciales
        OpcionesTemp.Add(new Opciones());
        OpcionesTemp.Add(new Opciones());
    }

    [RelayCommand]
    private async Task EliminarPreguntaAsync()
    {
        MensajeEliminar = "";
        MensajeEliminarError = "";

        if (PreguntaSeleccionada is null)
        {
            MensajeEliminarError = "No hay ninguna pregunta seleccionada.";
            _ = LimpiarMensajeAsync(() => MensajeEliminarError = "");
            return;
        }

        _context.Preguntas.Remove(PreguntaSeleccionada);
        await _context.SaveChangesAsync();

        Preguntas.Remove(PreguntaSeleccionada);
        PreguntaSeleccionada = null;

        MensajeEliminar = "Pregunta eliminada.";
        _ = LimpiarMensajeAsync(() => MensajeEliminar = "");
    }

    [RelayCommand]
    private void AgregarOpcion()
    {
        if (OpcionesTemp.Count >= 4)
        {
            MensajeLimiteOpciones = "Ya no puedes agregar más de 4 opciones.";
            _ = LimpiarMensajeAsync(() => MensajeLimiteOpciones = "");
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
            _ = LimpiarMensajeAsync(() => MensajeMinOpciones = "");
            return;
        }

        MensajeMinOpciones = "";
        OpcionesTemp.Remove(opcion);
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        Enunciado = "";
        CategoriaSeleccionada = null;
        OpcionesTemp.Clear();
        OpcionesTemp.Add(new Opciones());
        OpcionesTemp.Add(new Opciones());
        MensajeError = "";
        MensajeOpciones = "";
    }

    private async Task LimpiarMensajeAsync(Action limpiarAccion)
    {
        await Task.Delay(1000);
        limpiarAccion();
    }

    [RelayCommand]
    private async Task SeleccionarArchivoAsync(OpcionRespuestaUI opcion)
    {
        // TODO: Implementar diálogo para seleccionar archivo
        await Task.Delay(100);
    }
}