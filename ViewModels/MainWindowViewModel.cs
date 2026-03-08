using Quiz.Features.Categorias;
using Quiz.Features.Juegos;
using Quiz.Features.Preguntas;
using Quiz.Features.Usuarios;

namespace Quiz.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public UsuarioViewModel   UsuarioVM   { get; }
    public PreguntaViewModel  PreguntaVM  { get; }
    public CategoriaViewModel CategoriaVM { get; }
    public JuegoViewModel     JuegoVM     { get; }

    public MainWindowViewModel() : this(App.CreateDbContext()) { }

    public MainWindowViewModel(AppDbContext context)
    {
        CategoriaVM = new CategoriaViewModel(context);
        UsuarioVM   = new UsuarioViewModel(context);
        PreguntaVM  = new PreguntaViewModel(context);
        JuegoVM     = new JuegoViewModel(context);
    }
}
