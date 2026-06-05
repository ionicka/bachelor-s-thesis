    using CommunityToolkit.Maui;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using FlashCards.Services.Http;
using FlashCards.ViewModels;
using FlashCards.Views;
using Microsoft.Extensions.Logging;

namespace FlashCards;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => { });

        // ── Adresa API ─────────────────────────────────────────────
        // Pentru test pe PC: localhost
        // Pentru Android pe acelasi WiFi: IP-ul PC-ului ex. http://192.168.1.100:5202/
        string apiUrl = "http://192.168.56.1/";

        // ── Servicii HTTP ─────────────────────────────────────────
        builder.Services.AddHttpClient<IAuthService, AuthServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));

        builder.Services.AddHttpClient<ICardService, CardServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));

        builder.Services.AddHttpClient<ISesiuneService, SesiuneServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));

        // ── Services MAUI ─────────────────────────────────────────
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISessionStateService, SessionStateService>();
        builder.Services.AddSingleton<IImageStorageService, ImageStorageService>();

        // ── ViewModels ────────────────────────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<InregistrareViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SesiuneConfigViewModel>();
        builder.Services.AddSingleton<FluxViewModel>();
        builder.Services.AddTransient<StatisticiViewModel>();
        builder.Services.AddTransient<SetariViewModel>();
        builder.Services.AddTransient<AdminPanelViewModel>();
        builder.Services.AddTransient<EditeazaCuvantViewModel>();

        // ── Pages ─────────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<InregistrarePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SesiuneConfigPage>();
        builder.Services.AddTransient<FluxPage>();
        builder.Services.AddTransient<StatisticiPage>();
        builder.Services.AddTransient<SetariPage>();
        builder.Services.AddTransient<FelicitariPage>();
        builder.Services.AddTransient<AdminPanelPage>();
        builder.Services.AddTransient<EditeazaCuvantPage>();

        builder.Services.AddMemoryCache();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}