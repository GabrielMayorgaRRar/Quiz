using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.IO;

namespace Quiz.Converters;

public class FileNameWithoutExtensionConverter : IValueConverter
{
    public static readonly FileNameWithoutExtensionConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            try 
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch 
            {
                return path;
            }
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
