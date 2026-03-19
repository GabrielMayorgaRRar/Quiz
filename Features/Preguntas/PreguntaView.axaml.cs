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
}