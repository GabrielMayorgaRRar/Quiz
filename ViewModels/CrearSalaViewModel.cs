using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Quiz.Services;
using System.Linq;

namespace Quiz.ViewModels;

public partial class CrearSalaViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    public CrearSalaViewModel(MainWindowViewModel main)
    {
        _main = main;
        _ = CargarCategoriasAsync();
    }

    private async Task CargarCategoriasAsync()
    {
        var cats = await _main.ApiService.GetCategoriesAsync();
        Categorias.Clear();
        foreach (var c in cats) Categorias.Add(c);
    }

    [ObservableProperty]
    private string nombreSala = "";

    [ObservableProperty]
    private string nombre = "";

    [ObservableProperty]
    private string apodo = "";

    [ObservableProperty]
    private CategoryData? categoriaSeleccionada;

    [ObservableProperty]
    private string codigoSala = "";

    [ObservableProperty]
    private string textoCopiar = "COPIAR";

    public ObservableCollection<CategoryData> Categorias { get; } = new();

    public ObservableCollection<AvatarItem> Avatares { get; } = new()
    {
        new AvatarItem("https://drive.google.com/uc?export=view&id=1ycLLwdHblj_0d2AwxJb4x5zRdLGZ9S28"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1IACUfAUF33xmeo5KRvdONSIsEt7yeDRj"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1S9M9ymGL5gbsJUn1Y74L0WXm0zi0AoMr"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1obVlKml9-eRpXDOIu-bioqiNRTuTSl0x"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1gNQ15eDDu933yT7D9j5WD6bM8pw5LM2w"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1rCDWAGB6pb8HKcZjPF0r7CzHEE3tCbT7"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1XSwV66l1n-h6qC-RczE_CexjuchOvS43"),
        new AvatarItem("https://drive.google.com/uc?export=view&id=1wj6zxCWCdMU1BJoPAyxxOCTbvOGzT5mz")
    };

    [ObservableProperty]
    private AvatarItem? avatarSeleccionado;

    public bool PuedeCrear =>
        !string.IsNullOrWhiteSpace(NombreSala) &&
        !string.IsNullOrWhiteSpace(Nombre) &&
        !string.IsNullOrWhiteSpace(Apodo) &&
        CategoriaSeleccionada != null &&
        AvatarSeleccionado != null;

    partial void OnNombreSalaChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnNombreChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnApodoChanged(string value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnCategoriaSeleccionadaChanged(CategoryData? value) => OnPropertyChanged(nameof(PuedeCrear));
    partial void OnAvatarSeleccionadoChanged(AvatarItem? value) => OnPropertyChanged(nameof(PuedeCrear));



    [RelayCommand]
    private async Task CopiarCodigo()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow?.Clipboard is { } clipboard)
        {
            await clipboard.SetTextAsync(CodigoSala);

            TextoCopiar = "COPIADO ✔";
            await Task.Delay(1500);
            TextoCopiar = "COPIAR";
        }
    }

    [RelayCommand]
    private async Task CrearSala()
    {
        if (CategoriaSeleccionada == null) return;
        
        var request = new CreateRoomRequest
        {
            Name = NombreSala,
            Nickname = Nombre,
            AvatarUrl = AvatarSeleccionado?.Url ?? "",
            CategoryId = CategoriaSeleccionada.Id
        };
        
        var res = await _main.ApiService.CreateRoomAsync(request);
        if (res != null && res.Game != null)
        {
            CodigoSala = res.Game.Key;
            Console.WriteLine($"Sala creada: {CodigoSala}");
            _main.IrASala(CodigoSala, res.Game.Id, Nombre, true);
        }
    }
    [RelayCommand]
    private void Volver()
    {
        _main.IrAHome();
    }
}