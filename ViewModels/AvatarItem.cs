using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Quiz.ViewModels;

public partial class AvatarItem : ObservableObject
{
    public string Url { get; }

    [ObservableProperty]
    private Bitmap? _imagen;

    public AvatarItem(string url)
    {
        Url = url;
        _ = CargarImagenAsync();
    }

    private async Task CargarImagenAsync()
    {
        try
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(Url);
            using var stream = new System.IO.MemoryStream(bytes);
            Imagen = new Bitmap(stream);
        }
        catch { }
    }
}
