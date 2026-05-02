using System;
using System.Collections.Generic;

namespace Quiz.Models;

public class Juego
{
    public int Id { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = "esperando";
    public int CategoriaId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    
    // Propiedades de navegación
    public Categoria? Categoria { get; set; }
    public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
}