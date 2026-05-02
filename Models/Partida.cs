using System;
using System.Collections.Generic;

namespace Quiz.Models;

public class Partida
{
    public int Id { get; set; }
    public int JuegoId { get; set; }
    public int UsuarioId { get; set; }
    public int Puntos { get; set; } = 0;
    public bool Activo { get; set; } = true;
    public bool Completada { get; set; } = false;
    public DateTime FechaInicio { get; set; } = DateTime.Now;
    public DateTime? FechaFin { get; set; }
    
    // Propiedades de navegación
    public Juego? Juego { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<DetallePartida> Detalles { get; set; } = new List<DetallePartida>();
}