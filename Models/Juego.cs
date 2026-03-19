using System;
namespace Quiz.Models;

public class Juego
{
    public int Id { get; set; }
    public string Clave { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string nombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
