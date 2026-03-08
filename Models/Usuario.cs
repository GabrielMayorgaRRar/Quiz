using System;
namespace Quiz.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apodo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}
