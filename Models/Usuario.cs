using System;
using System.Collections.Generic;

namespace Quiz.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;   // Nombre real
    public string Apodo { get; set; } = string.Empty;    // Nickname para la partida
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    
    // Relación con las partidas jugadas
    public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
}