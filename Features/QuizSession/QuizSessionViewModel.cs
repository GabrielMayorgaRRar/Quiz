using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quiz.ViewModels;

namespace Quiz.Features.QuizSession;

public enum TipoPregunta
{
    Texto,
    Imagen,
    Audio
}

public class MockPregunta
{
    public string Enunciado { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public ObservableCollection<MockOpcion> Opciones { get; set; } = [];
}

public class MockOpcion
{
    public string Contenido { get; set; } = string.Empty;
    public bool EsCorrecta { get; set; }
}

public partial class QuizSessionViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MockPregunta> _preguntas = [];

    [ObservableProperty]
    private MockPregunta _preguntaActual = null!;

    [ObservableProperty]
    private int _indiceActual = 0;

    [ObservableProperty]
    private string _progresoTexto = string.Empty;

    [ObservableProperty]
    private bool _esModoTexto;

    [ObservableProperty]
    private bool _esModoImagen;

    [ObservableProperty]
    private bool _esModoAudio;

    [ObservableProperty]
    private bool _juegoTerminado;

    [ObservableProperty]
    private MockOpcion? _opcionSeleccionada;

    [ObservableProperty]
    private bool _mostrarFeedback;

    [ObservableProperty]
    private bool _esCorrecta;

    [ObservableProperty]
    private string _mensajeFeedback = string.Empty;

    [ObservableProperty]
    private string _respuestaCorrectaTexto = string.Empty;

    // NUEVA PROPIEDAD: Categoría seleccionada desde el menú
    [ObservableProperty]
    private string? _categoriaSeleccionada;

    public Action? OnQuizFinished;

    public QuizSessionViewModel()
    {
        // No cargar nada aquí, se carga cuando se establece la categoría
    }

    // Cuando cambia la categoría, cargamos las preguntas
    partial void OnCategoriaSeleccionadaChanged(string? value)
    {
        CargarPreguntasPorCategoria(value);
        IndiceActual = 0;
        ActualizarEstadoFase();
    }

    private void CargarPreguntasPorCategoria(string? categoria)
    {
        // ========== CIENCIA (12 preguntas) ==========
        var ciencia = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿Qué elemento químico tiene el símbolo 'Au'?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Plata", EsCorrecta = false },
                    new() { Contenido = "Oro", EsCorrecta = true },
                    new() { Contenido = "Cobre", EsCorrecta = false },
                    new() { Contenido = "Aluminio", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el planeta más grande del sistema solar?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Marte", EsCorrecta = false },
                    new() { Contenido = "Júpiter", EsCorrecta = true },
                    new() { Contenido = "Saturno", EsCorrecta = false },
                    new() { Contenido = "Neptuno", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el animal terrestre más rápido?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "León", EsCorrecta = false },
                    new() { Contenido = "Guepardo", EsCorrecta = true },
                    new() { Contenido = "Antílope", EsCorrecta = false },
                    new() { Contenido = "Caballo", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el océano más grande del mundo?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Atlántico", EsCorrecta = false },
                    new() { Contenido = "Índico", EsCorrecta = false },
                    new() { Contenido = "Pacífico", EsCorrecta = true },
                    new() { Contenido = "Ártico", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el hueso más largo del cuerpo humano?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Fémur", EsCorrecta = true },
                    new() { Contenido = "Tibia", EsCorrecta = false },
                    new() { Contenido = "Húmero", EsCorrecta = false },
                    new() { Contenido = "Radio", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué gas absorben las plantas para la fotosíntesis?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Oxígeno", EsCorrecta = false },
                    new() { Contenido = "Nitrógeno", EsCorrecta = false },
                    new() { Contenido = "Dióxido de carbono", EsCorrecta = true },
                    new() { Contenido = "Hidrógeno", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuántos corazones tiene un pulpo?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1", EsCorrecta = false },
                    new() { Contenido = "2", EsCorrecta = false },
                    new() { Contenido = "3", EsCorrecta = true },
                    new() { Contenido = "4", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el metal más ligero?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Aluminio", EsCorrecta = false },
                    new() { Contenido = "Titanio", EsCorrecta = false },
                    new() { Contenido = "Litio", EsCorrecta = true },
                    new() { Contenido = "Magnesio", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿A qué temperatura hierve el agua a nivel del mar?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "90°C", EsCorrecta = false },
                    new() { Contenido = "100°C", EsCorrecta = true },
                    new() { Contenido = "110°C", EsCorrecta = false },
                    new() { Contenido = "120°C", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué planeta es conocido como el 'planeta rojo'?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Venus", EsCorrecta = false },
                    new() { Contenido = "Marte", EsCorrecta = true },
                    new() { Contenido = "Júpiter", EsCorrecta = false },
                    new() { Contenido = "Saturno", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es la velocidad de la luz aproximadamente?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "300,000 km/s", EsCorrecta = true },
                    new() { Contenido = "150,000 km/s", EsCorrecta = false },
                    new() { Contenido = "1,000,000 km/s", EsCorrecta = false },
                    new() { Contenido = "500,000 km/s", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué célula no tiene núcleo?",
                Categoria = "Ciencia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Neurona", EsCorrecta = false },
                    new() { Contenido = "Glóbulo rojo", EsCorrecta = true },
                    new() { Contenido = "Glóbulo blanco", EsCorrecta = false },
                    new() { Contenido = "Célula muscular", EsCorrecta = false }
                ]
            }
        };

        // ========== HISTORIA (12 preguntas) ==========
        var historia = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿En qué año llegó Cristóbal Colón a América?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1492", EsCorrecta = true },
                    new() { Contenido = "1510", EsCorrecta = false },
                    new() { Contenido = "1485", EsCorrecta = false },
                    new() { Contenido = "1503", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién fue el primer presidente de México?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Benito Juárez", EsCorrecta = false },
                    new() { Contenido = "Porfirio Díaz", EsCorrecta = false },
                    new() { Contenido = "Guadalupe Victoria", EsCorrecta = true },
                    new() { Contenido = "Miguel Hidalgo", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué año comenzó la Segunda Guerra Mundial?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1914", EsCorrecta = false },
                    new() { Contenido = "1939", EsCorrecta = true },
                    new() { Contenido = "1941", EsCorrecta = false },
                    new() { Contenido = "1945", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién pintó la Mona Lisa?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Miguel Ángel", EsCorrecta = false },
                    new() { Contenido = "Leonardo da Vinci", EsCorrecta = true },
                    new() { Contenido = "Rafael", EsCorrecta = false },
                    new() { Contenido = "Donatello", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué imperio construyó Machu Picchu?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Azteca", EsCorrecta = false },
                    new() { Contenido = "Maya", EsCorrecta = false },
                    new() { Contenido = "Inca", EsCorrecta = true },
                    new() { Contenido = "Olmeca", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién fue el primer hombre en la luna?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Buzz Aldrin", EsCorrecta = false },
                    new() { Contenido = "Neil Armstrong", EsCorrecta = true },
                    new() { Contenido = "Yuri Gagarin", EsCorrecta = false },
                    new() { Contenido = "Michael Collins", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué año cayó el Muro de Berlín?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1985", EsCorrecta = false },
                    new() { Contenido = "1989", EsCorrecta = true },
                    new() { Contenido = "1991", EsCorrecta = false },
                    new() { Contenido = "1993", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién fue el líder de la Revolución Mexicana?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Emiliano Zapata", EsCorrecta = false },
                    new() { Contenido = "Pancho Villa", EsCorrecta = false },
                    new() { Contenido = "Francisco I. Madero", EsCorrecta = true },
                    new() { Contenido = "Venustiano Carranza", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué civilización construyó las pirámides de Egipto?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Romana", EsCorrecta = false },
                    new() { Contenido = "Griega", EsCorrecta = false },
                    new() { Contenido = "Egipcia", EsCorrecta = true },
                    new() { Contenido = "Persa", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué año se firmó la Declaración de Independencia de EE.UU.?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1776", EsCorrecta = true },
                    new() { Contenido = "1789", EsCorrecta = false },
                    new() { Contenido = "1810", EsCorrecta = false },
                    new() { Contenido = "1821", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién fue el primer emperador romano?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Julio César", EsCorrecta = false },
                    new() { Contenido = "Augusto", EsCorrecta = true },
                    new() { Contenido = "Nerón", EsCorrecta = false },
                    new() { Contenido = "Trajano", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué país construyó el Canal de Panamá?",
                Categoria = "Historia",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Francia", EsCorrecta = false },
                    new() { Contenido = "España", EsCorrecta = false },
                    new() { Contenido = "Estados Unidos", EsCorrecta = true },
                    new() { Contenido = "Inglaterra", EsCorrecta = false }
                ]
            }
        };

        // ========== MÚSICA (12 preguntas - todas con audio) ==========
        var musica = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "Identifica la obra maestra clásica que está sonando...",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Sinfonía No. 5 - Beethoven", EsCorrecta = true },
                    new() { Contenido = "Las Cuatro Estaciones - Vivaldi", EsCorrecta = false },
                    new() { Contenido = "Claro de Luna - Debussy", EsCorrecta = false },
                    new() { Contenido = "El Lago de los Cisnes - Tchaikovsky", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué banda inglesa lanzó el álbum 'Abbey Road'?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Queen", EsCorrecta = false },
                    new() { Contenido = "The Rolling Stones", EsCorrecta = false },
                    new() { Contenido = "The Beatles", EsCorrecta = true },
                    new() { Contenido = "Pink Floyd", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién es conocida como la 'Reina del Pop'?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Beyoncé", EsCorrecta = false },
                    new() { Contenido = "Madonna", EsCorrecta = true },
                    new() { Contenido = "Lady Gaga", EsCorrecta = false },
                    new() { Contenido = "Britney Spears", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué género musical se originó en Jamaica?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Salsa", EsCorrecta = false },
                    new() { Contenido = "Reggae", EsCorrecta = true },
                    new() { Contenido = "Merengue", EsCorrecta = false },
                    new() { Contenido = "Bachata", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué instrumento toca Mozart en esta pieza?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Violín", EsCorrecta = false },
                    new() { Contenido = "Piano", EsCorrecta = true },
                    new() { Contenido = "Flauta", EsCorrecta = false },
                    new() { Contenido = "Guitarra", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué canción de rock suena?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Bohemian Rhapsody", EsCorrecta = true },
                    new() { Contenido = "Stairway to Heaven", EsCorrecta = false },
                    new() { Contenido = "Hotel California", EsCorrecta = false },
                    new() { Contenido = "Imagine", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué ritmo latino es este?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Cumbia", EsCorrecta = true },
                    new() { Contenido = "Salsa", EsCorrecta = false },
                    new() { Contenido = "Merengue", EsCorrecta = false },
                    new() { Contenido = "Bachata", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué banda toca esta canción?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Coldplay", EsCorrecta = false },
                    new() { Contenido = "U2", EsCorrecta = true },
                    new() { Contenido = "Radiohead", EsCorrecta = false },
                    new() { Contenido = "Muse", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué instrumento de viento suena?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Saxofón", EsCorrecta = true },
                    new() { Contenido = "Trompeta", EsCorrecta = false },
                    new() { Contenido = "Clarinete", EsCorrecta = false },
                    new() { Contenido = "Flauta", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué ópera de Mozart es esta?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "La flauta mágica", EsCorrecta = true },
                    new() { Contenido = "Las bodas de Fígaro", EsCorrecta = false },
                    new() { Contenido = "Don Giovanni", EsCorrecta = false },
                    new() { Contenido = "Cosi fan tutte", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué género de música electrónica es?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "House", EsCorrecta = true },
                    new() { Contenido = "Techno", EsCorrecta = false },
                    new() { Contenido = "Trance", EsCorrecta = false },
                    new() { Contenido = "Dubstep", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué banda de los 80 suena?",
                Categoria = "Música",
                Tipo = TipoPregunta.Audio,
                Opciones = [
                    new() { Contenido = "Queen", EsCorrecta = false },
                    new() { Contenido = "The Police", EsCorrecta = true },
                    new() { Contenido = "The Cure", EsCorrecta = false },
                    new() { Contenido = "Duran Duran", EsCorrecta = false }
                ]
            }
        };

        // ========== DEPORTES (12 preguntas) ==========
        var deportes = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿Qué país ganó el Mundial de Fútbol de 2022?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Brasil", EsCorrecta = false },
                    new() { Contenido = "Francia", EsCorrecta = false },
                    new() { Contenido = "Argentina", EsCorrecta = true },
                    new() { Contenido = "Croacia", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué deporte se utiliza un 'tapete'?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Tenis", EsCorrecta = false },
                    new() { Contenido = "Lucha libre", EsCorrecta = true },
                    new() { Contenido = "Natación", EsCorrecta = false },
                    new() { Contenido = "Gimnasia", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuántos jugadores tiene un equipo de básquetbol en la cancha?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "5", EsCorrecta = true },
                    new() { Contenido = "6", EsCorrecta = false },
                    new() { Contenido = "7", EsCorrecta = false },
                    new() { Contenido = "4", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién tiene más títulos de Fórmula 1?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Ayrton Senna", EsCorrecta = false },
                    new() { Contenido = "Michael Schumacher", EsCorrecta = false },
                    new() { Contenido = "Lewis Hamilton", EsCorrecta = true },
                    new() { Contenido = "Max Verstappen", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el deporte más practicado del mundo?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Fútbol", EsCorrecta = true },
                    new() { Contenido = "Baloncesto", EsCorrecta = false },
                    new() { Contenido = "Tenis", EsCorrecta = false },
                    new() { Contenido = "Cricket", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cada cuántos años se celebran los Juegos Olímpicos?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "2 años", EsCorrecta = false },
                    new() { Contenido = "4 años", EsCorrecta = true },
                    new() { Contenido = "5 años", EsCorrecta = false },
                    new() { Contenido = "6 años", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué país ha ganado más mundiales de fútbol?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Argentina", EsCorrecta = false },
                    new() { Contenido = "Alemania", EsCorrecta = false },
                    new() { Contenido = "Brasil", EsCorrecta = true },
                    new() { Contenido = "Italia", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué tenista tiene más Grand Slams en individuales?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Roger Federer", EsCorrecta = false },
                    new() { Contenido = "Rafael Nadal", EsCorrecta = false },
                    new() { Contenido = "Novak Djokovic", EsCorrecta = true },
                    new() { Contenido = "Pete Sampras", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué año se celebró el primer Mundial de Fútbol?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1920", EsCorrecta = false },
                    new() { Contenido = "1930", EsCorrecta = true },
                    new() { Contenido = "1940", EsCorrecta = false },
                    new() { Contenido = "1950", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué país ganó el primer Mundial de Fútbol?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Brasil", EsCorrecta = false },
                    new() { Contenido = "Argentina", EsCorrecta = false },
                    new() { Contenido = "Uruguay", EsCorrecta = true },
                    new() { Contenido = "Italia", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuántos puntos vale un touchdown en fútbol americano?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "3", EsCorrecta = false },
                    new() { Contenido = "6", EsCorrecta = true },
                    new() { Contenido = "7", EsCorrecta = false },
                    new() { Contenido = "8", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué selección de fútbol tiene el apodo 'La Verdeamarela'?",
                Categoria = "Deportes",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Argentina", EsCorrecta = false },
                    new() { Contenido = "Brasil", EsCorrecta = true },
                    new() { Contenido = "México", EsCorrecta = false },
                    new() { Contenido = "Colombia", EsCorrecta = false }
                ]
            }
        };

        // ========== TECNOLOGÍA (12 preguntas) ==========
        var tecnologia = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿Qué significa 'CPU' en informática?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Central Processing Unit", EsCorrecta = true },
                    new() { Contenido = "Computer Personal Unit", EsCorrecta = false },
                    new() { Contenido = "Central Program Utility", EsCorrecta = false },
                    new() { Contenido = "Control Processing Unit", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué año se fundó Microsoft?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "1975", EsCorrecta = true },
                    new() { Contenido = "1980", EsCorrecta = false },
                    new() { Contenido = "1969", EsCorrecta = false },
                    new() { Contenido = "1985", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué lenguaje de programación usa .NET?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Python", EsCorrecta = false },
                    new() { Contenido = "Java", EsCorrecta = false },
                    new() { Contenido = "C#", EsCorrecta = true },
                    new() { Contenido = "JavaScript", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué empresa creó el iPhone?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Samsung", EsCorrecta = false },
                    new() { Contenido = "Apple", EsCorrecta = true },
                    new() { Contenido = "Google", EsCorrecta = false },
                    new() { Contenido = "Microsoft", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué es la 'nube' en informática?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "El clima", EsCorrecta = false },
                    new() { Contenido = "Servidores remotos", EsCorrecta = true },
                    new() { Contenido = "Un tipo de virus", EsCorrecta = false },
                    new() { Contenido = "Un navegador", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué significa 'RAM'?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Read Access Memory", EsCorrecta = false },
                    new() { Contenido = "Random Access Memory", EsCorrecta = true },
                    new() { Contenido = "Rapid Access Memory", EsCorrecta = false },
                    new() { Contenido = "Run Access Memory", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el sistema operativo más usado en PC?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "macOS", EsCorrecta = false },
                    new() { Contenido = "Linux", EsCorrecta = false },
                    new() { Contenido = "Windows", EsCorrecta = true },
                    new() { Contenido = "Chrome OS", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué significa 'HTTP'?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "HyperText Transfer Protocol", EsCorrecta = true },
                    new() { Contenido = "High Tech Transfer Protocol", EsCorrecta = false },
                    new() { Contenido = "Hyper Transfer Text Protocol", EsCorrecta = false },
                    new() { Contenido = "Home Tool Transfer Protocol", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién fundó Tesla Motors?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Elon Musk", EsCorrecta = false },
                    new() { Contenido = "Martin Eberhard", EsCorrecta = true },
                    new() { Contenido = "Nikola Tesla", EsCorrecta = false },
                    new() { Contenido = "Jeff Bezos", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué es un firewall?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Un virus", EsCorrecta = false },
                    new() { Contenido = "Un muro contra incendios", EsCorrecta = false },
                    new() { Contenido = "Un sistema de seguridad", EsCorrecta = true },
                    new() { Contenido = "Un navegador", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué significa 'AI'?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Artificial Intelligence", EsCorrecta = true },
                    new() { Contenido = "Automated Interface", EsCorrecta = false },
                    new() { Contenido = "Advanced Integration", EsCorrecta = false },
                    new() { Contenido = "Active Input", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué año se lanzó el primer iPhone?",
                Categoria = "Tecnología",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "2005", EsCorrecta = false },
                    new() { Contenido = "2007", EsCorrecta = true },
                    new() { Contenido = "2008", EsCorrecta = false },
                    new() { Contenido = "2010", EsCorrecta = false }
                ]
            }
        };

        // ========== GEOGRAFÍA (12 preguntas) ==========
        var geografia = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿Cuál es el río más largo del mundo?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Amazonas", EsCorrecta = true },
                    new() { Contenido = "Nilo", EsCorrecta = false },
                    new() { Contenido = "Misisipi", EsCorrecta = false },
                    new() { Contenido = "Yangtsé", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el país más grande del mundo?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Canadá", EsCorrecta = false },
                    new() { Contenido = "China", EsCorrecta = false },
                    new() { Contenido = "Rusia", EsCorrecta = true },
                    new() { Contenido = "Estados Unidos", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es la capital de Francia?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Londres", EsCorrecta = false },
                    new() { Contenido = "Berlín", EsCorrecta = false },
                    new() { Contenido = "París", EsCorrecta = true },
                    new() { Contenido = "Madrid", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué continente está Egipto?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Asia", EsCorrecta = false },
                    new() { Contenido = "Europa", EsCorrecta = false },
                    new() { Contenido = "África", EsCorrecta = true },
                    new() { Contenido = "Oceanía", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es la montaña más alta del mundo?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Monte Everest", EsCorrecta = true },
                    new() { Contenido = "K2", EsCorrecta = false },
                    new() { Contenido = "Makalu", EsCorrecta = false },
                    new() { Contenido = "Lhotse", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el océano más pequeño?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Pacífico", EsCorrecta = false },
                    new() { Contenido = "Atlántico", EsCorrecta = false },
                    new() { Contenido = "Índico", EsCorrecta = false },
                    new() { Contenido = "Ártico", EsCorrecta = true }
                ]
            },
            new() {
                Enunciado = "¿Cuál es la capital de Japón?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Seúl", EsCorrecta = false },
                    new() { Contenido = "Pekín", EsCorrecta = false },
                    new() { Contenido = "Tokio", EsCorrecta = true },
                    new() { Contenido = "Bangkok", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué país tiene forma de bota?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "España", EsCorrecta = false },
                    new() { Contenido = "Grecia", EsCorrecta = false },
                    new() { Contenido = "Italia", EsCorrecta = true },
                    new() { Contenido = "Portugal", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el desierto más grande del mundo?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Sahara", EsCorrecta = false },
                    new() { Contenido = "Antártida", EsCorrecta = true },
                    new() { Contenido = "Gobi", EsCorrecta = false },
                    new() { Contenido = "Kalahari", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es la capital de Australia?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Sídney", EsCorrecta = false },
                    new() { Contenido = "Melbourne", EsCorrecta = false },
                    new() { Contenido = "Canberra", EsCorrecta = true },
                    new() { Contenido = "Brisbane", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué río atraviesa la ciudad de París?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Rin", EsCorrecta = false },
                    new() { Contenido = "Danubio", EsCorrecta = false },
                    new() { Contenido = "Sena", EsCorrecta = true },
                    new() { Contenido = "Loira", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Cuál es el país más pequeño del mundo?",
                Categoria = "Geografía",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Mónaco", EsCorrecta = false },
                    new() { Contenido = "San Marino", EsCorrecta = false },
                    new() { Contenido = "Vaticano", EsCorrecta = true },
                    new() { Contenido = "Liechtenstein", EsCorrecta = false }
                ]
            }
        };

        // ========== ARTE (12 preguntas) ==========
        var arte = new ObservableCollection<MockPregunta>
        {
            new() {
                Enunciado = "¿Quién pintó 'La noche estrellada'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Pablo Picasso", EsCorrecta = false },
                    new() { Contenido = "Vincent van Gogh", EsCorrecta = true },
                    new() { Contenido = "Claude Monet", EsCorrecta = false },
                    new() { Contenido = "Salvador Dalí", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué museo se encuentra en París?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "El Prado", EsCorrecta = false },
                    new() { Contenido = "Louvre", EsCorrecta = true },
                    new() { Contenido = "British Museum", EsCorrecta = false },
                    new() { Contenido = "Metropolitan", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién pintó el techo de la Capilla Sixtina?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Rafael", EsCorrecta = false },
                    new() { Contenido = "Donatello", EsCorrecta = false },
                    new() { Contenido = "Miguel Ángel", EsCorrecta = true },
                    new() { Contenido = "Leonardo da Vinci", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué movimiento artístico representa Salvador Dalí?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Impresionismo", EsCorrecta = false },
                    new() { Contenido = "Cubismo", EsCorrecta = false },
                    new() { Contenido = "Surrealismo", EsCorrecta = true },
                    new() { Contenido = "Expresionismo", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién es el autor de 'El Guernica'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Francisco de Goya", EsCorrecta = false },
                    new() { Contenido = "Diego Velázquez", EsCorrecta = false },
                    new() { Contenido = "Pablo Picasso", EsCorrecta = true },
                    new() { Contenido = "Joan Miró", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué artista es conocido por sus 'puntos'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Andy Warhol", EsCorrecta = false },
                    new() { Contenido = "Roy Lichtenstein", EsCorrecta = false },
                    new() { Contenido = "Georges Seurat", EsCorrecta = true },
                    new() { Contenido = "Henri Matisse", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién pintó 'Las Meninas'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Francisco de Goya", EsCorrecta = false },
                    new() { Contenido = "Diego Velázquez", EsCorrecta = true },
                    new() { Contenido = "El Greco", EsCorrecta = false },
                    new() { Contenido = "José de Ribera", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué artista es famoso por sus 'relojes blandos'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Salvador Dalí", EsCorrecta = true },
                    new() { Contenido = "René Magritte", EsCorrecta = false },
                    new() { Contenido = "Max Ernst", EsCorrecta = false },
                    new() { Contenido = "Joan Miró", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿En qué ciudad se encuentra el Museo del Prado?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Barcelona", EsCorrecta = false },
                    new() { Contenido = "Madrid", EsCorrecta = true },
                    new() { Contenido = "Sevilla", EsCorrecta = false },
                    new() { Contenido = "Valencia", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién pintó 'El grito'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Edvard Munch", EsCorrecta = true },
                    new() { Contenido = "Gustav Klimt", EsCorrecta = false },
                    new() { Contenido = "Egon Schiele", EsCorrecta = false },
                    new() { Contenido = "Paul Cézanne", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Qué movimiento artístico representa Claude Monet?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Impresionismo", EsCorrecta = true },
                    new() { Contenido = "Cubismo", EsCorrecta = false },
                    new() { Contenido = "Expresionismo", EsCorrecta = false },
                    new() { Contenido = "Barroco", EsCorrecta = false }
                ]
            },
            new() {
                Enunciado = "¿Quién pintó 'La persistencia de la memoria'?",
                Categoria = "Arte",
                Tipo = TipoPregunta.Texto,
                Opciones = [
                    new() { Contenido = "Pablo Picasso", EsCorrecta = false },
                    new() { Contenido = "Salvador Dalí", EsCorrecta = true },
                    new() { Contenido = "Joan Miró", EsCorrecta = false },
                    new() { Contenido = "René Magritte", EsCorrecta = false }
                ]
            }
        };

        // Seleccionar las preguntas según la categoría
        switch (categoria)
        {
            case "Ciencia":
                Preguntas = ciencia;
                break;
            case "Historia":
                Preguntas = historia;
                break;
            case "Musica":
                Preguntas = musica;
                break;
            case "Deportes":
                Preguntas = deportes;
                break;
            case "Tecnologia":
                Preguntas = tecnologia;
                break;
            case "Geografia":
                Preguntas = geografia;
                break;
            case "Arte":
                Preguntas = arte;
                break;
            default:
                // Si no hay categoría o es "Todas", cargar todas las preguntas
                var todas = new ObservableCollection<MockPregunta>();
                foreach (var p in ciencia) todas.Add(p);
                foreach (var p in historia) todas.Add(p);
                foreach (var p in musica) todas.Add(p);
                foreach (var p in deportes) todas.Add(p);
                foreach (var p in tecnologia) todas.Add(p);
                foreach (var p in geografia) todas.Add(p);
                foreach (var p in arte) todas.Add(p);
                Preguntas = todas;
                break;
        }
    }

    private void ActualizarEstadoFase()
    {
        if (Preguntas.Count == 0) return;
        
        PreguntaActual = Preguntas[IndiceActual];
        ProgresoTexto = $"{IndiceActual + 1} / {Preguntas.Count}";
        
        EsModoTexto = PreguntaActual.Tipo == TipoPregunta.Texto;
        EsModoImagen = PreguntaActual.Tipo == TipoPregunta.Imagen;
        EsModoAudio = PreguntaActual.Tipo == TipoPregunta.Audio;
        JuegoTerminado = false;
        MostrarFeedback = false;
    }

    [RelayCommand]
    private void SeleccionarOpcion(MockOpcion opcion)
    {
        OpcionSeleccionada = opcion;
        
        // Verificar si la respuesta es correcta
        EsCorrecta = opcion.EsCorrecta;
        
        if (opcion.EsCorrecta)
        {
            MensajeFeedback = "¡Correcto!";
        }
        else
        {
            // Buscar cuál era la respuesta correcta
            var respuestaCorrecta = PreguntaActual.Opciones.FirstOrDefault(o => o.EsCorrecta);
            RespuestaCorrectaTexto = respuestaCorrecta?.Contenido ?? "No especificada";
            MensajeFeedback = $"Incorrecto. La respuesta correcta era: {RespuestaCorrectaTexto}";
        }
        
        MostrarFeedback = true;
        
        // Esperar 2 segundos para mostrar el feedback antes de avanzar
        Task.Delay(2000).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                MostrarFeedback = false;
                
                // Advance to next question
                if (IndiceActual < Preguntas.Count - 1)
                {
                    IndiceActual++;
                    ActualizarEstadoFase();
                }
                else
                {
                    // Quiz finished
                    JuegoTerminado = true;
                    EsModoTexto = false;
                    EsModoImagen = false;
                    EsModoAudio = false;
                    ProgresoTexto = "COMPLETADO";
                }
            });
        });
    }

    [RelayCommand]
    private void ReiniciarPartida()
    {
        IndiceActual = 0;
        ActualizarEstadoFase();
    }

    [RelayCommand]
    private void FinalizarPartida()
    {
        OnQuizFinished?.Invoke();
    }
}
