using CommunityToolkit.Maui;
using FlashCards.Core.Interfaces;
using FlashCards.Core.Services;
using FlashCards.Data.Context;
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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── PostgreSQL ────────────────────────────────────────────
        string connStr = LexaDbContext.ConnectionString;

        // DbContextFactory permite crearea de contexte independente
        // rezolva NpgsqlOperationInProgressException
        builder.Services.AddDbContextFactory<LexaDbContext>(opt =>
            opt.UseNpgsql(connStr));

        // Pastram si AddDbContext pentru compatibilitate
        builder.Services.AddDbContext<LexaDbContext>(opt =>
            opt.UseNpgsql(connStr));

        // ── Repositories — Transient pentru a evita conflicte de conexiune ──
        builder.Services.AddTransient<ICardRepository>(_ =>
            new CardRepository(connStr));
        builder.Services.AddTransient<IProgresRepository, ProgresRepository>();
        builder.Services.AddTransient<IUtilizatorRepository, UtilizatorRepository>();
        builder.Services.AddTransient<ISesiuneRepository, SesiuneRepository>();
        builder.Services.AddTransient<IRaspunsRepository, RaspunsRepository>();

        // ── Services Core — Transient ─────────────────────────────
        builder.Services.AddTransient<ISrsService, SrsService>();
        builder.Services.AddTransient<ICardService, CardService>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<ISesiuneService, SesiuneService>();

        // ── Services MAUI ─────────────────────────────────────────
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISessionStateService, SessionStateService>();

        // ── ViewModels ────────────────────────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<InregistrareViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SesiuneConfigViewModel>();
        builder.Services.AddSingleton<FluxViewModel>();
        builder.Services.AddTransient<StatisticiViewModel>();
        builder.Services.AddTransient<SetariViewModel>();

        // ── Pages ─────────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<InregistrarePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SesiuneConfigPage>();
        builder.Services.AddTransient<FluxPage>();
        builder.Services.AddTransient<StatisticiPage>();
        builder.Services.AddTransient<SetariPage>();
        builder.Services.AddTransient<FelicitariPage>();

        builder.Services.AddMemoryCache();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Creeaza schema si populeaza cu date la prima rulare
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LexaDbContext>();

            // Sterge si recreaza baza de date (doar in development)
            // ctx.Database.EnsureDeleted();

            // Creeaza tabelele dupa schema din entitati
            ctx.Database.EnsureCreated();

            // Seed data — doar daca nu exista cuvinte
            if (!ctx.Cuvinte.Any())
            {
                SeedData.Populeaza(ctx);
            }
        }

        return app;
    }
}