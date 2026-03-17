using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Quiz.Converters;

public class CorrectTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool esCorrecta)
        {
            if (esCorrecta)
                return new SolidColorBrush(Color.Parse("#2E7D32")); // Verde oscuro
            else
                return new SolidColorBrush(Color.Parse("#C62828")); // Rojo oscuro
        }
        return new SolidColorBrush(Color.Parse("#000000")); // Negro por defecto
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
