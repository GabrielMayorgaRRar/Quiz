using System;
namespace Quiz.Models;

public class Partida
{
    public int Id { get; set; }
    public int JuegoId { get; set; }
    public int UsuarioId { get; set; }
    public int Puntos { get; set; }
    public DateTime Fecha { get; set; }
}