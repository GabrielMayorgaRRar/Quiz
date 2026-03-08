using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace Quiz.Converters;

public class BitmapAssetValueConverter : IValueConverter
{
    public static readonly BitmapAssetValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string rawUri && Uri.TryCreate(rawUri, UriKind.Absolute, out var uri))
        {
            try
            {
                return new Bitmap(AssetLoader.Open(uri));
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
