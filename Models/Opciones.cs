using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Models;

public enum TipoRespuesta
{
    Texto,
    Imagen,
    Audio
}

public partial class Opciones : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string contenido = string.Empty;

    [ObservableProperty]
    private bool esCorrecta;

    [ObservableProperty]
    private TipoRespuesta _tipoRespuesta = TipoRespuesta.Texto;

    public int PreguntaId { get; set; }

    public Pregunta? Pregunta { get; set; }

    [NotMapped]
    [ObservableProperty]
    private Avalonia.Media.Imaging.Bitmap? _imagenCargada;
}