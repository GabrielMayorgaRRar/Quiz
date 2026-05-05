using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Quiz.Views;

public partial class JuegoView : UserControl
{
    public JuegoView()
    {
        InitializeComponent();
    }

    private void OnInnerButtonClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
    }
}