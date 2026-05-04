using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quiz.Services;
using System.Threading.Tasks;

namespace Quiz.ViewModels;

public partial class UnirseViewModel : ObservableObject
{
    private readonly MainWindowViewModel _main;

    public UnirseViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    [ObservableProperty]
    private string nombre = "";

    [ObservableProperty]
    private string apodo = "";

    [ObservableProperty]
    private string codigoSala = "";

    public System.Collections.ObjectModel.ObservableCollection<AvatarItem> Avatares { get; } = new()
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

    public bool PuedeUnirse =>
        !string.IsNullOrWhiteSpace(Nombre) &&
        !string.IsNullOrWhiteSpace(Apodo) &&
        !string.IsNullOrWhiteSpace(CodigoSala) &&
        AvatarSeleccionado != null;

    partial void OnNombreChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));
    partial void OnApodoChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));
    partial void OnCodigoSalaChanged(string value) => OnPropertyChanged(nameof(PuedeUnirse));
    partial void OnAvatarSeleccionadoChanged(AvatarItem? value) => OnPropertyChanged(nameof(PuedeUnirse));

    [RelayCommand]
    private async Task Unirse()
    {
        Console.WriteLine($"Intentando unirse a sala: {CodigoSala}");

        var request = new JoinRoomRequest
        {
            Key = CodigoSala,
            Nickname = Nombre,
            AvatarUrl = AvatarSeleccionado?.Url ?? ""
        };

        var res = await _main.ApiService.JoinRoomAsync(request);
        if (res != null && res.Game != null)
        {
            _main.IrASala(CodigoSala, res.Game.Id, Nombre, false, res.Departure?.Id ?? 0, res.Players);
        }
        else
        {
            Console.WriteLine("Error al unirse a la sala");
        }
    }

    [RelayCommand]
    private void Volver()
    {
        _main.IrAHome();
    }
}