using CommunityToolkit.Mvvm.ComponentModel;

namespace Quiz.Models;

public partial class Opciones : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string contenido = string.Empty;

    [ObservableProperty]
    private bool esCorrecta;

    public int PreguntaId { get; set; }

    public Pregunta? Pregunta { get; set; }
}