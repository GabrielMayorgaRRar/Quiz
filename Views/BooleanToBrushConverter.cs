using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Quiz.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool correcto && correcto)
            return new SolidColorBrush(Color.Parse("#4CAF50")); // verde

        return new SolidColorBrush(Color.Parse("#F44336")); // rojo
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}