using System;

namespace Quiz.Models;

public class DetallePartida
{
    public int Id { get; set; }
    public int PartidaId { get; set; }
    public int PreguntaId { get; set; }
    public int OpcionId { get; set; }
    public bool EsCorrecta { get; set; }
    public int PuntosObtenidos { get; set; }
    public DateTime FechaRespuesta { get; set; } = DateTime.Now;
    
    // Propiedades de navegación
    public Partida? Partida { get; set; }
    public Pregunta? Pregunta { get; set; }
    public Opciones? Opcion { get; set; }
}