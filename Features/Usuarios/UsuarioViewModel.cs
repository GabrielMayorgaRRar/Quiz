using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.Features.Usuarios;

public partial class UsuarioViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Usuario> _usuarios = [];

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _apodo = string.Empty;    [ObservableProperty]
    private Usuario? _usuarioSeleccionado;

    public UsuarioViewModel(AppDbContext context)
    {
        _context = context;
    }

    private async Task CargarUsuariosAsync()
    {
        var lista = await _context.Usuarios.ToListAsync();
        Usuarios = new ObservableCollection<Usuario>(lista);
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre)) return;
        var usuario = new Usuario { Nombre = Nombre, Apodo = Apodo };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        Usuarios.Add(usuario);
        Nombre = string.Empty;
        Apodo = string.Empty;
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (UsuarioSeleccionado is null) return;
        _context.Usuarios.Remove(UsuarioSeleccionado);
        await _context.SaveChangesAsync();
        Usuarios.Remove(UsuarioSeleccionado);
    }
}
