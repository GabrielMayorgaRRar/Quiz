using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Quiz;

public class MayorQueDosConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
            return count > 2;

        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return 0;
    }
}