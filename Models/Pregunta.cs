using System;
using System.Collections.Generic;

namespace Quiz.Models;

public class Pregunta
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string TipoMedia { get; set; } = "texto";
    public string UrlMedia { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    
    public Categoria? Categoria { get; set; }  // Relación con Categoría
    public ICollection<Opciones> Opciones { get; set; } = new List<Opciones>();  // Relación con Opciones
}