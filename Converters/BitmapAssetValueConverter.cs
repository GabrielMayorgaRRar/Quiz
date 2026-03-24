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
                rawUri = rawUri.Trim();

                if (rawUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    rawUri = rawUri.Substring(7);
                    if (rawUri.StartsWith("/") && System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        rawUri = rawUri.TrimStart('/');
                    }
                }

                if (rawUri.StartsWith("avares://"))
                {
                    return new Bitmap(AssetLoader.Open(new Uri(rawUri)));
                }

                string cleanUri = rawUri.TrimStart('/', '\\').Replace('\\', '/');
                
                if (cleanUri.StartsWith("Assets/"))
                {
                    try 
                    {
                        var uri = new Uri($"avares://Quiz/{cleanUri}");
                        if (AssetLoader.Exists(uri))
                        {
                            return new Bitmap(AssetLoader.Open(uri));
                        }
                    }
                    catch { }
                }

                string absolutePath = rawUri;
                if (!Path.IsPathRooted(absolutePath))
                {
                    absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanUri);
                }

                if (File.Exists(absolutePath))
                {
                    return new Bitmap(absolutePath);
                }
                else
                {
                    File.AppendAllText("/tmp/quiz_converter.log", $"[{DateTime.Now}] File not found: {absolutePath}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("/tmp/quiz_converter.log", $"[{DateTime.Now}] Exception loading {rawUri}: {ex.Message}\n");
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
