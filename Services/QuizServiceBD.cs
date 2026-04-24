using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
namespace Quiz.Services;
public class QuizServiceBD : IQuizService
{
    private readonly AppDbContext _context;

    public QuizServiceBD(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pregunta>> GetPreguntasPorCategoria(string categoria)
    {
        IQueryable<Pregunta> query = _context.Preguntas
            .Include(p => p.Categoria)
            .Include(p => p.Opciones);

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(p => p.Categoria != null && p.Categoria.Nombre == categoria);
        }

        return await query.ToListAsync();
    }
}