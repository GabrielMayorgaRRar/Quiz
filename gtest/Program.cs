using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var input = "https://drive.google.com/uc?export=view\\u0026id=1fW26h4WM2ykKp2KG0kpvCym-TcvwP5r0";
        var url = input.Trim().Trim('"', '\'').Replace("\\u0026", "&");
        var match = Regex.Match(url, @"(?:/d/|id=)([a-zA-Z0-9_-]{10,})", RegexOptions.IgnoreCase);
        var directUrl = match.Success ? $"https://drive.google.com/uc?export=download&id={match.Groups[1].Value}" : input;
        
        Console.WriteLine($"Direct URL: {directUrl}");
        
        using var client = new HttpClient();
        try {
            var response = await client.GetAsync(directUrl);
            Console.WriteLine($"Status: {response.StatusCode}");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Console.WriteLine($"Downloaded {bytes.Length} bytes.");
            if (bytes.Length > 0)
            {
                Console.WriteLine($"First bytes: {bytes[0]:X2} {bytes[1]:X2}");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
