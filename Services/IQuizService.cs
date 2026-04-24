using System.Collections.Generic;
using System.Threading.Tasks;
namespace Quiz.Models;
public interface IQuizService
{
    Task<List<Pregunta>> GetPreguntasPorCategoria(string categoria);
}