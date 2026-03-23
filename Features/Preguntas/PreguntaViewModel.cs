using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;

namespace Quiz.Features.Preguntas;

// Enum para tipos de respuesta
public enum TipoRespuesta
{
    Texto,
    Imagen,
    Audio
}

// Modelo de opción de respuesta
public partial class OpcionRespuesta : ObservableObject
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

public partial class PreguntaViewModel : ObservableObject
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private string _enunciado = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = [];

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private ObservableCollection<OpcionRespuesta> _opciones = [];

    public PreguntaViewModel(AppDbContext context)
    {
        _context = context;
        _ = CargarCategoriasAsync();
        
        // Inicializar con 4 opciones vacías
        for (int i = 0; i < 4; i++)
        {
            Opciones.Add(new OpcionRespuesta());
        }
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _context.Categorias
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        Categorias = new ObservableCollection<Categoria>(categorias);
    }

    [RelayCommand]
    private void AgregarOpcion()
    {
        Opciones.Add(new OpcionRespuesta());
    }

    [RelayCommand]
    private async Task SeleccionarArchivoAsync(OpcionRespuesta opcion)
    {
        // TODO: Implementar diálogo para seleccionar archivo
        // Esto dependerá de la implementación de Avalonia
        // Por ahora es un placeholder, crei que es mejor
        await Task.Delay(100);
        
        // Ejemplo de cómo podría ser:
        // var dialog = new OpenFileDialog();
        // dialog.Filters.Add(new FileDialogFilter { Name = "Imágenes", Extensions = { "jpg", "png", "gif" } });
        // dialog.Filters.Add(new FileDialogFilter { Name = "Audios", Extensions = { "mp3", "wav", "ogg" } });
        // 
        // var result = await dialog.ShowAsync();
        // if (result != null && result.Length > 0)
        // {
        //     opcion.Contenido = result[0];
        // }
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            // TODO: Mostrar mensaje de error - La pregunta es obligatoria
            return;
        }
        
        if (CategoriaSeleccionada is null)
        {
            // TODO: Mostrar mensaje de error - Debe seleccionar una categoría
            return;
        }
        
        // Validar que todas las opciones tengan contenido
        foreach (var opcion in Opciones)
        {
            if (string.IsNullOrWhiteSpace(opcion.Contenido))
            {
                // TODO: Mostrar mensaje de error - Todas las opciones deben tener contenido
                return;
            }
        }
        
        // Validar que haya al menos una opción correcta
        if (!Opciones.Any(o => o.EsCorrecta))
        {
            // TODO: Mostrar mensaje de error - Debe haber al menos una respuesta correcta
            return;
        }
        
        // Validar que no haya más de una opción correcta (opcional)
        var correctasCount = Opciones.Count(o => o.EsCorrecta);
        if (correctasCount > 1)
        {
            // TODO: Mostrar mensaje de error - Solo puede haber una respuesta correcta
            return;
        }

        try
        {
            // Crear la pregunta
            var pregunta = new Pregunta
            {
                Enunciado = Enunciado,
                CategoriaId = CategoriaSeleccionada.Id
            };
            
            _context.Preguntas.Add(pregunta);
            await _context.SaveChangesAsync();

            // Guardar las opciones
            foreach (var opcion in Opciones)
            {
                var respuesta = new Respuesta
                {
                    Contenido = opcion.Contenido,
                    EsCorrecta = opcion.EsCorrecta,
                    TipoRespuesta = opcion.TipoRespuesta.ToString(),
                    PreguntaId = pregunta.Id
                };
                
                _context.Respuestas.Add(respuesta);
            }
            
            await _context.SaveChangesAsync();

            // Limpiar formulario después de guardar exitosamente
            Enunciado = string.Empty;
            CategoriaSeleccionada = null;
            Opciones.Clear();
            
            // Reinicializar con 4 opciones vacías
            for (int i = 0; i < 4; i++)
            {
                Opciones.Add(new OpcionRespuesta());
            }
            
            // TODO: Mostrar mensaje de éxito - "Pregunta guardada correctamente"
        }
        catch (Exception ex)
        {
            // TODO: Mostrar mensaje de error
            System.Diagnostics.Debug.WriteLine($"Error al guardar pregunta: {ex.Message}");
        }
    }
    
    // Método para limpiar el formulario
    [RelayCommand]
    private void LimpiarFormulario()
    {
        Enunciado = string.Empty;
        CategoriaSeleccionada = null;
        Opciones.Clear();
        
        for (int i = 0; i < 4; i++)
        {
            Opciones.Add(new OpcionRespuesta());
        }
    }
}
