using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Quiz.ViewModels;
using Quiz.Models;

namespace Quiz.Features.Home;

public partial class HomeViewModel : ViewModelBase
{
    public Action<string>? OnStartQuizWithCategory;

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = [];

    public HomeViewModel()
    {
        _ = CargarCategoriasAsync();
    }

    public async Task CargarCategoriasAsync()
    {
        try
        {
            using var context = App.CreateDbContext();
            var dbCategorias = await context.Categorias.ToListAsync();
            System.Console.WriteLine($"[HOME] Cargadas {dbCategorias.Count} categorias desde la BD.");
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Categorias.Clear();
                foreach (var c in dbCategorias)
                {
                    Categorias.Add(c);
                }
                
                // Si no hay categorías, agregar una de prueba para validar la UI
                if (Categorias.Count == 0)
                {
                    Categorias.Add(new Categoria { Nombre = "Prueba Vacía", Descripcion = "No hay info en BD" });
                }
            });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"[HOME DB ERROR]: {ex.Message}");
        }
    }

    [RelayCommand]
    private void IniciarPartida()
    {
        OnStartQuizWithCategory?.Invoke(string.Empty);
    }

    [RelayCommand]
    private void IniciarPartidaConCategoria(string categoria)
    {
        OnStartQuizWithCategory?.Invoke(categoria);
    }
}
