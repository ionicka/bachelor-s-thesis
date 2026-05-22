using CommunityToolkit.Maui;
using FlashCards.Core.Interfaces;
using FlashCards.Core.Services;
using FlashCards.Data.Context;
using FlashCards.Data.Repositories;
using FlashCards.Services;
using FlashCards.ViewModels;
using FlashCards.Views;
using FlashCards.Core.Services;
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

        // ── BENCHMARK: Initializare baza de date ──────────────────
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LexaDbContext>();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
            System.Diagnostics.Debug.WriteLine("[BENCHMARK] Initializare FlashCards...");
            System.Diagnostics.Debug.WriteLine($"[BENCHMARK] Timestamp: {DateTime.Now:HH:mm:ss.fff}");
            System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");

            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            ctx.Database.EnsureCreated();
            sw1.Stop();
            System.Diagnostics.Debug.WriteLine($"[BENCHMARK] EnsureCreated (schema BD): {sw1.ElapsedMilliseconds} ms");

            if (!ctx.Cuvinte.Any())
            {
                var sw2 = System.Diagnostics.Stopwatch.StartNew();
                SeedData.Populeaza(ctx);
                sw2.Stop();
                System.Diagnostics.Debug.WriteLine($"[BENCHMARK] SeedData (50 cuvinte): {sw2.ElapsedMilliseconds} ms");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[BENCHMARK] SeedData: skipped (date existente)");
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");
            System.Diagnostics.Debug.WriteLine($"[BENCHMARK] Total initializare: {sw.ElapsedMilliseconds} ms");
            System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");            // Seed data — doar daca nu exista cuvinte
            if (!ctx.Cuvinte.Any())
            {
                SeedData.Populeaza(ctx);
            }
        }

        return app;
    }
}