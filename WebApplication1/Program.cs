using FlashCards.Core.Interfaces;
using FlashCards.Core.Services;
using FlashCards.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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

builder.WebHost.UseUrls("http://0.0.0.0:5202");

var app = builder.Build();

// ─── Imagini statice ───
var folderImagini = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "User Name", "com.companyname.FlashCards", "Data", "imagini_cuvinte");

if (!Directory.Exists(folderImagini))
    Directory.CreateDirectory(folderImagini);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(folderImagini),
    RequestPath = "/imagini"
});

app.MapControllers();
app.Run();