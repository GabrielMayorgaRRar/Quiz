namespace Quiz.Models;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
}
