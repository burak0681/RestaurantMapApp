using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System.Text;

namespace RestaurantMapApp;

public static class MauiProgram
{
    public static IServiceProvider? Services { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        // UTF-8 encoding desteği
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        // Dependency Injection
        builder.Services.AddSingleton<Services.IRestaurantService, Services.RestaurantService>();

        var app = builder.Build();
        Services = app.Services;
        return app;
    }
}