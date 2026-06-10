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

#if ANDROID
        string apiUrl = "http://192.168.2.102:5202/";
#else
string apiUrl = "http://localhost:5202/";
#endif
        // ── Servicii HTTP ─────────────────────────────────────────
        builder.Services.AddHttpClient<IAuthService, AuthServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));

        builder.Services.AddHttpClient<ICardService, CardServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));

        builder.Services.AddHttpClient<ISesiuneService, SesiuneServiceHttp>(client =>
            client.BaseAddress = new Uri(apiUrl));
        builder.Services.AddHttpClient<IAdminService, AdminServiceHttp>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        });

        // ── Services MAUI ─────────────────────────────────────────
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISessionStateService, SessionStateService>();
        builder.Services.AddSingleton<IImageStorageService, ImageStorageService>();

        // ── ViewModels ────────────────────────────────────────────
        // ── ViewModels ────────────────────────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<InregistrareViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<SesiuneConfigViewModel>();
        builder.Services.AddSingleton<FluxViewModel>();
        builder.Services.AddSingleton<StatisticiViewModel>();
        builder.Services.AddSingleton<SetariViewModel>();
        builder.Services.AddTransient<AdminPanelViewModel>();
        builder.Services.AddTransient<EditeazaCuvantViewModel>();
        // ── Pages ─────────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<InregistrarePage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SesiuneConfigPage>();
        builder.Services.AddSingleton<FluxPage>();
        builder.Services.AddSingleton<StatisticiPage>();
        builder.Services.AddSingleton<SetariPage>();
        builder.Services.AddTransient<FelicitariPage>();
        builder.Services.AddTransient<AdminPanelViewModel>();
        builder.Services.AddTransient<EditeazaCuvantPage>();
        builder.Services.AddSingleton<VocabularViewModel>();
        builder.Services.AddSingleton<VocabularPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}