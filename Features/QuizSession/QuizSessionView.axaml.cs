using Avalonia.Controls;
using Quiz.Models;

namespace Quiz.Features.QuizSession;

public partial class QuizSessionView : UserControl
{
    public QuizSessionView()
    {
        InitializeComponent();
    }

    private void OnOptionTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is Control control && control.Tag is Opciones opcion)
        {
            if (DataContext is QuizSessionViewModel vm)
            {
                if (vm.SeleccionarOpcionCommand.CanExecute(opcion))
                {
                    vm.SeleccionarOpcionCommand.Execute(opcion);
                }
            }
        }
    }
}
