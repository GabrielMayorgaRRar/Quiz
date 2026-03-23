namespace Quiz.Models;
using System.Collections.Generic;

public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public int CategoriaId { get; set; }


    public ICollection<Opciones>? Opciones { get; set; }
    public Categoria? Categoria { get; set; }
}