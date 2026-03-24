using Avalonia.Controls;
using Avalonia.Input;

namespace Quiz.Features.Preguntas;

public partial class PreguntaView : UserControl
{
    public PreguntaView()
    {
        InitializeComponent();
    }

    private void Opcion_KeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is TextBox tb)
        {
            var pos = tb.CaretIndex;

            tb.Text = tb.Text?.ToLower();

            tb.CaretIndex = pos;
        }
    }

    private async void SeleccionarArchivo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Quiz.Models.Opciones opcion)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
                {
                    Title = "Seleccionar Archivo Multimedia",
                    AllowMultiple = false
                });

                if (files.Count >= 1)
                {
                    var file = files[0];
                    if (file.Path.LocalPath is string localPath && !string.IsNullOrEmpty(localPath))
                    {
                        opcion.Contenido = localPath;
                    }
                }
            }
        }
    }
}