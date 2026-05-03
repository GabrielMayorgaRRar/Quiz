using CommunityToolkit.Mvvm.ComponentModel;
using Quiz.Features.Categorias;
using Quiz.Features.Home;
using Quiz.Features.Juegos;
using Quiz.Features.Preguntas;
using Quiz.Features.QuizSession;
using Quiz.Features.Usuarios;

namespace Quiz.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
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
    public JuegoViewModel JuegoVM { get; }
    public QuizSessionViewModel QuizSessionVM { get; }

    public CrearSalaViewModel CrearSalaVM { get; }

    // 🔥 FALTABA ESTO
    public UnirseViewModel UnirseVM { get; }

    public MainWindowViewModel() : this(App.CreateDbContext()) { }

    public MainWindowViewModel(AppDbContext context)
    {
        CategoriaVM = new CategoriaViewModel(context);
        UsuarioVM = new UsuarioViewModel(context);
        PreguntaVM = new PreguntaViewModel(context);
        JuegoVM = new JuegoViewModel(context);
        QuizSessionVM = new QuizSessionViewModel(context);

        HomeVM = new HomeViewModel(this);
        CrearSalaVM = new CrearSalaViewModel(this);

        // 🔥 YA FUNCIONA
        UnirseVM = new UnirseViewModel(this);

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
}