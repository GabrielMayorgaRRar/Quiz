using CommunityToolkit.Mvvm.ComponentModel;
using Quiz.Features.Home;
using Quiz.Services;
using Quiz.ViewModels;

namespace Quiz.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public QuizApiService ApiService { get; } = new();
    public QuizWebSocketService WsService { get; } = new();

    [ObservableProperty]
    private object? currentView;

    public HomeViewModel HomeVM { get; }
    public CrearSalaViewModel CrearSalaVM { get; }
    public UnirseViewModel UnirseVM { get; }
    public SalaViewModel SalaVM { get; }

    public MainWindowViewModel()
    {
        HomeVM = new HomeViewModel(this);
        CrearSalaVM = new CrearSalaViewModel(this);
        UnirseVM = new UnirseViewModel(this);
        SalaVM = new SalaViewModel(this);

        CurrentView = HomeVM;
    }

    public void IrACrearSala() => CurrentView = CrearSalaVM;
    public void IrAHome() => CurrentView = HomeVM;
    public void IrAUnirse() => CurrentView = UnirseVM;

    public void IrASala(string codigo, int gameId, string jugador, bool owner, int departureId,
        System.Collections.Generic.List<DepartureData>? players = null, int categoryId = 0)
    {
        SalaVM.Inicializar(codigo, gameId, jugador, owner, departureId, players, categoryId);
        CurrentView = SalaVM;
    }

    public void IrAJuego(int gameId, int departureId, int categoryId = 0)
    {
        CurrentView = new JuegoViewModel(this, gameId, departureId, categoryId);
    }
}