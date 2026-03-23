using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    private string _mensajeEliminar = "";

    [ObservableProperty]
    private string _mensajeEliminarError = "";

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private ObservableCollection<OpcionRespuesta> _opciones = [];

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
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeError = "";
        });
            return;
        }

        if (CategoriaSeleccionada == null)
        {
            MensajeError = "Debe seleccionar una categoría.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeError = "";
        });
            return;
        }

        // validar duplicado
        bool existe = await _context.Preguntas.AnyAsync(p =>
            p.Enunciado.ToLower().Trim() == Enunciado.ToLower().Trim()
            && p.CategoriaId == CategoriaSeleccionada.Id);

        if (existe)
        {
            MensajeError = "Esta pregunta ya existe en esa categoría.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeError = "";
        });
            return;
        }

        if (OpcionesTemp.Count < 2)
            return;

        if (OpcionesTemp.Any(o => string.IsNullOrWhiteSpace(o.Contenido)))
        {
            MensajeOpciones = "Todas las opciones deben tener contenido.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeOpciones = "";
        });
            return;
        }

        var repetidas = OpcionesTemp
        .GroupBy(o => o.Contenido.Trim().ToLower())
        .Any(g => g.Count() > 1);

        if (repetidas)
        {
            MensajeOpciones = "No puede haber opciones de respuesta iguales.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeOpciones = "";
        });
            return;
        }

        int correctas = OpcionesTemp.Count(o => o.EsCorrecta);

        if (correctas != 1)
        {
            MensajeOpciones = "Debe marcar una opción correcta.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeOpciones = "";
        });
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
        _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeExito = "";
        });

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
        MensajeEliminar = "";
        MensajeEliminarError = "";

        if (PreguntaSeleccionada is null)
        {
            MensajeEliminarError = "No hay ninguna pregunta seleccionada.";

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                MensajeEliminarError = "";
            });

            return;
        }

        _context.Preguntas.Remove(PreguntaSeleccionada);
        await _context.SaveChangesAsync();

        Preguntas.Remove(PreguntaSeleccionada);

        MensajeEliminar = "Pregunta eliminada.";

        _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeEliminar = "";
        });
    }

    [RelayCommand]
    private void AgregarOpcion()
    {
        if (OpcionesTemp.Count >= 4)
        {
            MensajeLimiteOpciones = "Ya no puedes agregar más de 4 opciones.";
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeLimiteOpciones = "";
        });
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
            _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            MensajeMinOpciones = "";
        });
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