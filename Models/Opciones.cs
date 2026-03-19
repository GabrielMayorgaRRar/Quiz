namespace Quiz.Models;

public class Opciones
{
    public int Id { get; set; }

    public string Contenido { get; set; } = string.Empty;

    public bool EsCorrecta { get; set; }

    public int PreguntaId { get; set; }

    // Es una relacion para navegacion
    public Pregunta? Pregunta { get; set; }
}