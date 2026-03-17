using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Quiz.Converters;

public class CorrectBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool esCorrecta)
        {
            if (esCorrecta)
                return new SolidColorBrush(Color.Parse("#81C784")); // Verde claro
            else
                return new SolidColorBrush(Color.Parse("#E57373")); // Rojo claro
        }
        return new SolidColorBrush(Color.Parse("#FFFFFF")); // Blanco por defecto
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
