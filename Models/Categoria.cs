using System;
using System.Collections.Generic;

namespace Quiz.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string IconoUrl { get; set; } = string.Empty;
    
    // Propiedades de navegación
    public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
    public ICollection<Juego> Juegos { get; set; } = new List<Juego>();
}