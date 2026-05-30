using CommunityToolkit.Maui;
using FlashCards.Core.Interfaces;
using FlashCards.Core.Services;
using FlashCards.Data.Repositories;
using FlashCards.Services;
using FlashCards.ViewModels;
using FlashCards.Views;
using Microsoft.EntityFrameworkCore;
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
            .ConfigureFonts(fonts => { /* ... */ });

        // ── Config + Connection String ──────────────────────────
        var config = ConfigLoader.Incarca();  // sincron, nu mai GetResult()
        string connStr = config.Database.ToConnectionString();

        builder.Services.AddSingleton(config);

        // Factory: fiecare repo primește contextul lui propriu, nu se intersectează
        builder.Services.AddDbContextFactory<LexaDbContext>(opt =>
            opt.UseNpgsql(connStr));

        builder.Services.AddTransient<ICardRepository>(_ =>
            new CardRepository(connStr));
        builder.Services.AddTransient<IProgresRepository, ProgresRepository>();
        builder.Services.AddTransient<IUtilizatorRepository, UtilizatorRepository>();
        builder.Services.AddTransient<ISesiuneRepository, SesiuneRepository>();
        builder.Services.AddTransient<IRaspunsRepository, RaspunsRepository>();
        builder.Services.AddTransient<IAdminRepository, AdminRepository>();

        // ── Services Core — Transient ─────────────────────────────
        builder.Services.AddTransient<ISrsService, SrsService>();
        builder.Services.AddTransient<ICardService, CardService>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<ISesiuneService, SesiuneService>();
        builder.Services.AddTransient<IAdminService, AdminService>();

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

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LexaDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            ctx.Database.EnsureCreated();


            //admins job
          /*  if (!ctx.Cuvinte.Any())
            {
                SeedData.Populeaza(ctx);
                logger.LogInformation("SeedData populat cu cuvinte initiale");
            }*/

            sw.Stop();
            logger.LogInformation("Initializare BD completa in {Ms}ms", sw.ElapsedMilliseconds);
        }

        return app;
    }
}