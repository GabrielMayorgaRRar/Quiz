using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quiz.Models;
using System.Text.Json;
using System.Net.Http;
namespace Quiz.Services;
public class QuizServiceAPI : IQuizService
{
    private readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("https://tu-api.com/")
    };

    public async Task<List<Pregunta>> GetPreguntasPorCategoria(string categoria)
    {
        var response = await _http.GetStringAsync($"preguntas?categoria={categoria}");
        return JsonSerializer.Deserialize<List<Pregunta>>(response);
    }
}