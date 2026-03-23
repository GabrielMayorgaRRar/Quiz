using System;
using System.IO;

namespace Quiz;

public static class EnvLoader
{
    public static void Load()
    {
        var currentDir = Directory.GetCurrentDirectory();
        string? dotenv = null;

        while (currentDir != null)
        {
            var potentialDotenv = Path.Combine(currentDir, ".env");
            if (File.Exists(potentialDotenv))
            {
                dotenv = potentialDotenv;
                break;
            }
            
            var parent = Directory.GetParent(currentDir);
            if (parent == null) break;
            currentDir = parent.FullName;
        }

        if (dotenv == null)
        {
            currentDir = AppDomain.CurrentDomain.BaseDirectory;
            while (currentDir != null)
            {
                var potentialDotenv = Path.Combine(currentDir, ".env");
                if (File.Exists(potentialDotenv))
                {
                    dotenv = potentialDotenv;
                    break;
                }
                var parent = Directory.GetParent(currentDir);
                if (parent == null) break;
                currentDir = parent.FullName;
            }
        }

        if (dotenv == null)
            return;

        foreach (var line in File.ReadAllLines(dotenv))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }
    }
}
