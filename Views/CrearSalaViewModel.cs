using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

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
    private void CopiarCodigo()
    {
        // Opcional: implementar clipboard real después
        Console.WriteLine($"Código copiado: {CodigoSala}");
    }

    [RelayCommand]
    private void CrearSala()
    {
        Console.WriteLine("Sala creada");
    }
}