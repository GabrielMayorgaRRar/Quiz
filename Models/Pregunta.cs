namespace Quiz.Models;

public class Pregunta
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
