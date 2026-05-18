using CommunityToolkit.Maui;
using LexaCard.Core.Interfaces;
using LexaCard.Core.Services;
using LexaCard.Data.Context;
using LexaCard.Data.Repositories;
using LexaCard.Services;
using LexaCard.ViewModels;
using LexaCard.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexaCard;

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

        builder.Services.AddDbContext<LexaDbContext>(opt =>
            opt.UseNpgsql(connStr));

        // ── Repositories ──────────────────────────────────────────
        builder.Services.AddScoped<ICardRepository>(_ =>
            new CardRepository(connStr));
        builder.Services.AddScoped<IProgresRepository, ProgresRepository>();
        builder.Services.AddScoped<IUtilizatorRepository, UtilizatorRepository>();
        builder.Services.AddScoped<ISesiuneRepository, SesiuneRepository>();
        builder.Services.AddScoped<IRaspunsRepository, RaspunsRepository>();

        // ── Services Core ─────────────────────────────────────────
        builder.Services.AddScoped<ISrsService, SrsService>();
        builder.Services.AddScoped<ICardService, CardService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
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

        // Creeaza baza de date si aplica seed data
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LexaDbContext>();
            ctx.Database.EnsureCreated();
        }

        return app;
    }
}