namespace Quiz.Models;

public class Opciones
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public int PreguntaId { get; set; }
    public bool EsCorrecta { get; set; }
}