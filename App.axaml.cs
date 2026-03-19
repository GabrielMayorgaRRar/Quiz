using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Quiz.ViewModels;
using Quiz.Views;

namespace Quiz;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Crea y configura el AppDbContext leyendo variables de entorno.
    /// </summary>
    public static AppDbContext CreateDbContext()
    {
        var host   = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port   = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var name   = Environment.GetEnvironmentVariable("DB_NAME") ?? "FLDSMDFR";
        var user   = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var pass   = Environment.GetEnvironmentVariable("DB_PASS") ?? "fghj";

        var connStr = $"Host={host};Port={port};Database={name};Username={user};Password={pass}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr)
            .Options;
        return new AppDbContext(options);
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}
