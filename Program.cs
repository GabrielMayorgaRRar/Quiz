using Avalonia;
using System;
using Microsoft.Extensions.DependencyInjection;
using Quiz.ViewModels;
using Quiz.Services;

namespace Quiz;

sealed class Program
{
    public static IServiceProvider? Services { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        WireUpServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void WireUpServices(IServiceCollection services)
    {
        // Servicios del API — reemplazan la conexión directa a la BD
        services.AddSingleton<QuizApiService>();
        services.AddSingleton<QuizWebSocketService>();

        services.AddTransient<MainWindowViewModel>();
    }
}