using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Quiz.ViewModels;

public partial class CrearSalaViewModel : ObservableObject
{
    [ObservableProperty]
    private string nombreSala = "";

    [ObservableProperty]
    private string nombre = "";

    [ObservableProperty]
    private string apodo = "";

    [ObservableProperty]
    private string? categoriaSeleccionada;

    [ObservableProperty]
    private string codigoSala = "";

    public ObservableCollection<string> Categorias { get; } = new()
    {
        "Historia",
        "Ciencia",
        "Deportes",
        "Tecnología"
    };

    public bool PuedeCrear =>
        !string.IsNullOrWhiteSpace(NombreSala) &&
        !string.IsNullOrWhiteSpace(Nombre) &&
        !string.IsNullOrWhiteSpace(Apodo) &&
        CategoriaSeleccionada != null;

    public CrearSalaViewModel()
    {
        GenerarCodigo();
    }

    partial void OnNombreSalaChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnNombreChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnApodoChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnCategoriaSeleccionadaChanged(string? value) => OnPropertyChanged(nameof(PuedeCrear));

    private void GenerarCodigo()
    {
        var random = new Random();
        CodigoSala = random.Next(100000, 999999).ToString();
    }

    [RelayCommand]
    private async Task CopiarCodigo()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow?.Clipboard != null)
        {
            await desktop.MainWindow.Clipboard.SetTextAsync(CodigoSala);
        }
    }

    [RelayCommand]
    private void CrearSala()
    {
        Console.WriteLine($"Sala creada: {NombreSala} - Código: {CodigoSala}");
    }
}