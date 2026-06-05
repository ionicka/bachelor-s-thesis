using FlashCards.Core.Interfaces;
using FlashCards.Core.Services;
using FlashCards.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Conexiune BD ───
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Lipseste connectionString!");

// ─── EF Core ───
builder.Services.AddDbContextFactory<FlashCardDbContext>(opt =>
    opt.UseNpgsql(connStr));

// ─── Repositories ───
builder.Services.AddScoped<ICardRepository>(_ => new CardRepository(connStr));
builder.Services.AddScoped<IProgresRepository, ProgresRepository>();
builder.Services.AddScoped<ISesiuneRepository, SesiuneRepository>();
builder.Services.AddScoped<IRaspunsRepository, RaspunsRepository>();
builder.Services.AddScoped<IUtilizatorRepository, UtilizatorRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// ─── Services ───
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ISrsService, SrsService>();
builder.Services.AddScoped<ISesiuneService, SesiuneService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Urls.Add("http://192.168.56.1");
app.Run();