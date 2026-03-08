namespace Quiz.Models;

public class DetallePartida
{
    public int Id { get; set; }
    public int PartidaId { get; set;}
    public int PreguntaId { get; set;}
    public int OpcionElegida { get; set;}
    public bool EsCorrecta { get; set;}
}