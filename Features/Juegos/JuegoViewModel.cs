using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.Features.Juegos;

public partial class JuegoViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    // ========== PROPIEDADES PARA CRUD DE SALAS ==========
    [ObservableProperty]
    private ObservableCollection<Juego> _juegos = [];

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = [];

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _clave = string.Empty;

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    [ObservableProperty]
    private Juego? _juegoSeleccionado;

    // ========== PROPIEDADES PARA EL JUEGO ACTIVO ==========
    [ObservableProperty]
    private Juego? _juegoActual;                    // Sala en la que estás jugando

    [ObservableProperty]
    private Partida? _miParticipacion;              // Tu participación en la partida

    [ObservableProperty]
    private List<Pregunta> _preguntas = new();       // Lista de preguntas del juego

    [ObservableProperty]
    private int _preguntaActualIndex = 0;            // Índice de la pregunta actual

    [ObservableProperty]
    private string _preguntaTexto = string.Empty;    // Texto de la pregunta actual

    [ObservableProperty]
    private List<Opciones> _opciones = new();          // Opciones barajadas

    [ObservableProperty]
    private bool _enJuego = false;                   // Si el juego está activo

    [ObservableProperty]
    private string _codigoSala = string.Empty;       // Código para unirse a sala

    [ObservableProperty]
    private string _nickname = string.Empty;         // Nickname del jugador

    [ObservableProperty]
    private string _mensaje = string.Empty;          // Mensajes de feedback

    public JuegoViewModel(AppDbContext context)
    {
        _context = context;
        GenerarNuevaClave();
        _ = CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        var juegos = await _context.Juegos.ToListAsync();
        Juegos = new ObservableCollection<Juego>(juegos);

        var categorias = await _context.Categorias.ToListAsync();
        Categorias = new ObservableCollection<Categoria>(categorias);
    }

    private void GenerarNuevaClave()
    {
        // Format: yyMMdd-HHmmss (e.g. 260308-153045)
        Clave = DateTime.Now.ToString("yyMMdd-HHmmss");
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Clave) || CategoriaSeleccionada is null) return;
        
        var juego = new Juego 
        { 
            Nombre = Nombre, 
            Clave = Clave, 
            CategoriaId = CategoriaSeleccionada.Id,
            Estado = "esperando",
            FechaCreacion = DateTime.Now
        };
        
        _context.Juegos.Add(juego);
        await _context.SaveChangesAsync();
        Juegos.Add(juego);
        
        // Reset form for next entry
        Nombre = string.Empty;
        GenerarNuevaClave();
        CategoriaSeleccionada = null;
        
        Mensaje = $" Sala creada! Código: {juego.Clave}";
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (JuegoSeleccionado is null) return;
        _context.Juegos.Remove(JuegoSeleccionado);
        await _context.SaveChangesAsync();
        Juegos.Remove(JuegoSeleccionado);
        Mensaje = " Sala eliminada";
    }

    [RelayCommand]
    private void RefrescarClave()
    {
        GenerarNuevaClave();
    }

    [RelayCommand]
    private async Task UnirseASala()
    {
        if (string.IsNullOrWhiteSpace(CodigoSala))
        {
            Mensaje = "Ingresa un código de sala";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Nickname))
        {
            Mensaje = "Ingresa tu nickname";
            return;
        }
        
        // Buscar la sala por código
        var juego = await _context.Juegos
            .FirstOrDefaultAsync(j => j.Clave == CodigoSala.ToUpper());
        
        if (juego == null)
        {
            Mensaje = "Sala no encontrada";
            return;
        }
        
        if (juego.Estado != "esperando")
        {
            Mensaje = "La partida ya comenzó o terminó";
            return;
        }
        
        // Crear un nuevo usuario
        var usuario = new Usuario
        {
            Apodo = Nickname,
            Nombre = Nickname,
            FechaRegistro = DateTime.Now
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        
        // Crear la participación (Partida)
        var partida = new Partida
        {
            JuegoId = juego.Id,
            UsuarioId = usuario.Id,
            Puntos = 0,
            Activo = true,
            Completada = false,
            FechaInicio = DateTime.Now
        };
        _context.Partidas.Add(partida);
        await _context.SaveChangesAsync();
        
        // Actualizar propiedades
        JuegoActual = juego;
        MiParticipacion = partida;
        
        Mensaje = $"Te uniste a {juego.Nombre} como {Nickname}!";
    }

    [RelayCommand]
    private async Task IniciarJuego()
    {
        if (JuegoActual == null)
        {
            Mensaje = "No hay sala seleccionada. Únete primero.";
            return;
        }
        
        if (MiParticipacion == null)
        {
            Mensaje = "Debes unirte a la sala primero";
            return;
        }
        
        // Cambiar estado de la sala
        JuegoActual.Estado = "activo";
        await _context.SaveChangesAsync();
        
        // Cargar preguntas de la categoría
        Preguntas = await _context.Preguntas
            .Where(p => p.CategoriaId == JuegoActual.CategoriaId)
            .Include(p => p.Opciones)
            .ToListAsync();
        
        if (Preguntas.Count == 0)
        {
            Mensaje = "No hay preguntas disponibles para esta categoría";
            return;
        }
        
        // Iniciar el juego
        EnJuego = true;
        PreguntaActualIndex = 0;
        CargarPregunta(0);
        Mensaje = " ¡Partida iniciada! Buena suerte!";
    }

    private void CargarPregunta(int index)
    {
        if (index >= Preguntas.Count)
        {
            _ = FinalizarJuego();
            return;
        }
        
        var pregunta = Preguntas[index];
        PreguntaTexto = pregunta.Texto;
        
        // Barajar opciones (orden aleatorio)
        Opciones = pregunta.Opciones
            .OrderBy(x => Guid.NewGuid())
            .ToList();
    }

    [RelayCommand]
    private async Task Responder(Opciones opcionSeleccionada)
    {
        if (MiParticipacion == null || JuegoActual == null)
        {
            Mensaje = " Error: No hay partida activa";
            return;
        }
        
        if (PreguntaActualIndex >= Preguntas.Count)
        {
            await FinalizarJuego();
            return;
        }
        
        var preguntaActual = Preguntas[PreguntaActualIndex];
        var esCorrecta = opcionSeleccionada.EsCorrecta;
        var puntos = esCorrecta ? 10 : 0;
        
        // Registrar el detalle de la respuesta
        var detalle = new DetallePartida
        {
            PartidaId = MiParticipacion.Id,
            PreguntaId = preguntaActual.Id,
            OpcionId = opcionSeleccionada.Id,
            EsCorrecta = esCorrecta,
            PuntosObtenidos = puntos,
            FechaRespuesta = DateTime.Now
        };
        
        _context.DetallesPartida.Add(detalle);
        
        // Actualizar puntaje total del jugador
        MiParticipacion.Puntos += puntos;
        
        await _context.SaveChangesAsync();
        
        // Mostrar feedback
        if (esCorrecta)
        {
            Mensaje = " ¡Correcto! +10 puntos";
        }
        else
        {
            var opcionCorrecta = preguntaActual.Opciones.FirstOrDefault(o => o.EsCorrecta);
            Mensaje = $" Incorrecto. La respuesta correcta era: {opcionCorrecta?.Contenido}";
        }
        
        // Avanzar a la siguiente pregunta
        PreguntaActualIndex++;
        
        if (PreguntaActualIndex < Preguntas.Count)
        {
            CargarPregunta(PreguntaActualIndex);
        }
        else
        {
            await FinalizarJuego();
        }
    }

    private async Task FinalizarJuego()
{
    if (MiParticipacion != null)
    {
        MiParticipacion.Completada = true;
        MiParticipacion.FechaFin = DateTime.Now;
        await _context.SaveChangesAsync();
    }
    
    EnJuego = false;
    
    // Validar que JuegoActual no sea null
    if (JuegoActual == null)
    {
        Mensaje = "Error: Sala no encontrada";
        return;
    }
    
    // Obtener ranking con Include completo
    var partidas = await _context.Partidas
        .Where(p => p.JuegoId == JuegoActual.Id)
        .Include(p => p.Usuario)  // ← Esto incluye los datos del usuario
        .ToListAsync();
    
    var ranking = partidas
        .OrderByDescending(p => p.Puntos)
        .Select(p => new { 
            Nickname = p.Usuario?.Apodo ?? "Anónimo",  // ← Usuario podría ser null
            Puntos = p.Puntos 
        })
        .ToList();
    
    var rankingTexto = string.Join("\n", ranking.Select((r, i) => 
        $"{i + 1}. {r.Nickname} - {r.Puntos} puntos"));
    
    var tuPuntaje = MiParticipacion?.Puntos ?? 0;
    var tuPosicion = ranking.FindIndex(r => r.Puntos == tuPuntaje) + 1;
    
    Mensaje = $" ¡JUEGO TERMINADO! \n\n" +
              $"Tu puntaje: {tuPuntaje} puntos\n" +
              $"Tu posición: #{tuPosicion} de {ranking.Count}\n\n" +
              $"📊 RANKING FINAL:\n{rankingTexto}";
    
    // Cambiar estado de la sala
    JuegoActual.Estado = "terminado";
    await _context.SaveChangesAsync();
}
}