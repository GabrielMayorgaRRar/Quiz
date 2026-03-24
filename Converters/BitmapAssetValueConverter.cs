using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

namespace Quiz.Converters;

public class BitmapAssetValueConverter : IValueConverter
{
    public static readonly BitmapAssetValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string rawUri && !string.IsNullOrWhiteSpace(rawUri))
        {
            try
            {
                if (rawUri.StartsWith("avares://"))
                {
                    return new Bitmap(AssetLoader.Open(new Uri(rawUri)));
                }

                string absolutePath = rawUri;
                if (!Path.IsPathRooted(absolutePath))
                {
                    absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rawUri.TrimStart('/'));
                }

                if (File.Exists(absolutePath))
                {
                    return new Bitmap(absolutePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando imagen: {ex.Message}");
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
