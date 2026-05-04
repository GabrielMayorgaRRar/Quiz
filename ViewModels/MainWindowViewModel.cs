using CommunityToolkit.Mvvm.ComponentModel;
using Quiz.Features.Categorias;
using Quiz.Features.Home;
using Quiz.Features.Juegos;
using Quiz.Features.Preguntas;
using Quiz.Features.QuizSession;
using Quiz.Features.Usuarios;
using Quiz.Services;

namespace Quiz.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public QuizApiService ApiService { get; } = new();

    [ObservableProperty]
    private object? currentView;

    [ObservableProperty]
    private int _selectedTab = 0;

    partial void OnSelectedTabChanged(int value)
    {
        if (value == 2)
        {
            PreguntaVM.RecargarDatos();
        }
    }

    public HomeViewModel HomeVM { get; }
    public UsuarioViewModel UsuarioVM { get; }
    public PreguntaViewModel PreguntaVM { get; }
    public CategoriaViewModel CategoriaVM { get; }
    public QuizSessionViewModel QuizSessionVM { get; }

    public CrearSalaViewModel CrearSalaVM { get; }

    public UnirseViewModel UnirseVM { get; }
    public SalaViewModel SalaVM { get; }


    public MainWindowViewModel() : this(App.CreateDbContext()) { }

    public MainWindowViewModel(AppDbContext context)
    {
        CategoriaVM = new CategoriaViewModel(context);
        UsuarioVM = new UsuarioViewModel(context);
        PreguntaVM = new PreguntaViewModel(context);


        QuizSessionVM = new QuizSessionViewModel(context);

        HomeVM = new HomeViewModel(this);
        CrearSalaVM = new CrearSalaViewModel(this);

        UnirseVM = new UnirseViewModel(this);
        SalaVM = new SalaViewModel(this);

        CurrentView = HomeVM;
    }

    public void IrACrearSala()
    {
        CurrentView = CrearSalaVM;
    }

    public void IrAHome()
    {
        CurrentView = HomeVM;
    }

    public void IrAUnirse()
    {
        CurrentView = UnirseVM;
    }

    public void IrASala(string codigo, int gameId, string jugador, bool owner, int departureId, System.Collections.Generic.List<DepartureData>? players = null)
    {
        SalaVM.Inicializar(codigo, gameId, jugador, owner, departureId, players);
        CurrentView = SalaVM;
    }

    public void IrAJuego(int gameId, int departureId)
    {
        CurrentView = new JuegoViewModel(this, gameId, departureId);
    }
}