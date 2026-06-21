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


        // ── Servicii HTTP ──
        builder.Services.AddHttpClient<IAuthService, AuthServiceHttp>(client =>
            client.BaseAddress = new Uri(ApiConfig.ApiBaseUrl));

        builder.Services.AddHttpClient<ICardService, CardServiceHttp>(client =>
            client.BaseAddress = new Uri(ApiConfig.ApiBaseUrl));

        builder.Services.AddHttpClient<ISesiuneService, SesiuneServiceHttp>(client =>
            client.BaseAddress = new Uri(ApiConfig.ApiBaseUrl));

        builder.Services.AddHttpClient<IAdminService, AdminServiceHttp>(client =>
            client.BaseAddress = new Uri(ApiConfig.ApiBaseUrl));

        // â”€â”€ Services MAUI â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISessionStateService, SessionStateService>();
        builder.Services.AddSingleton<IImageStorageService, ImageStorageService>();

        // â”€â”€ ViewModels â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        // â”€â”€ ViewModels â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<InregistrareViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<SesiuneConfigViewModel>();
        builder.Services.AddSingleton<FluxViewModel>();
        builder.Services.AddSingleton<StatisticiViewModel>();
        builder.Services.AddSingleton<SetariViewModel>();
        builder.Services.AddTransient<AdminPanelViewModel>();
        builder.Services.AddTransient<EditeazaCuvantViewModel>();
        // â”€â”€ Pages â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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