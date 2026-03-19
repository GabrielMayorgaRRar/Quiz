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
    private int _selectedTab = 0;

    public HomeViewModel      HomeVM      { get; }
    public UsuarioViewModel   UsuarioVM   { get; }
    public PreguntaViewModel  PreguntaVM  { get; }
    public CategoriaViewModel CategoriaVM { get; }
    public JuegoViewModel     JuegoVM     { get; }
    public QuizSessionViewModel QuizSessionVM { get; }

    public MainWindowViewModel() : this(App.CreateDbContext()) { }

    public MainWindowViewModel(AppDbContext context)
    {
        CategoriaVM = new CategoriaViewModel(context);
        UsuarioVM   = new UsuarioViewModel(context);
        PreguntaVM  = new PreguntaViewModel(context);
        JuegoVM     = new JuegoViewModel(context);
        
        HomeVM = new HomeViewModel();
        QuizSessionVM = new QuizSessionViewModel();

        // Wiring navigation
        // Assuming Tabs: 0=Inicio, 1=Juegos, 2=Preguntas, 3=Categorias, 4=Usuarios, 5=Partida Activa
        HomeVM.OnStartQuizRequested = () => SelectedTab = 5;
        QuizSessionVM.OnQuizFinished = () => SelectedTab = 0;
    }
}
