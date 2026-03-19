using System;
using System.Collections.ObjectModel;
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

    public JuegoViewModel(AppDbContext context)
    {
        _context = context;
        GenerarNuevaClave();
        // Fire & Forget initialization
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
            nombre = Nombre, 
            Clave = Clave, 
            CategoriaId = CategoriaSeleccionada.Id,
            FechaCreacion = DateTime.Now
        };
        
        _context.Juegos.Add(juego);
        await _context.SaveChangesAsync();
        Juegos.Add(juego);
        
        // Reset form for next entry
        Nombre = string.Empty;
        GenerarNuevaClave();
        CategoriaSeleccionada = null;
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (JuegoSeleccionado is null) return;
        _context.Juegos.Remove(JuegoSeleccionado);
        await _context.SaveChangesAsync();
        Juegos.Remove(JuegoSeleccionado);
    }

    [RelayCommand]
    private void RefrescarClave()
    {
        GenerarNuevaClave();
    }
}
