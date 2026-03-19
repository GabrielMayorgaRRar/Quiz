using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.Features.Categorias;

public partial class CategoriaViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = [];

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private Categoria? _categoriaSeleccionada;

    public CategoriaViewModel(AppDbContext context)
    {
        _context = context;
    }

    private async Task CargarCategoriasAsync()
    {
        var lista = await _context.Categorias.ToListAsync();
        Categorias = new ObservableCollection<Categoria>(lista);
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre)) return;
        var categoria = new Categoria { Nombre = Nombre };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        Categorias.Add(categoria);
        Nombre = string.Empty;
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (CategoriaSeleccionada is null) return;
        _context.Categorias.Remove(CategoriaSeleccionada);
        await _context.SaveChangesAsync();
        Categorias.Remove(CategoriaSeleccionada);
    }
}
