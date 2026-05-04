using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Quiz.Converters;

public class IndexVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && parameter is string indexStr && int.TryParse(indexStr, out int index))
        {
            return count > index;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}