using Microsoft.Extensions.Logging;

namespace FlashCards.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly ILogger<ImageStorageService> _logger;
    private readonly string _folderImagini;

    // Extensii acceptate
    private static readonly string[] ExtensiiPermise =
        { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

    public ImageStorageService(ILogger<ImageStorageService> logger)
    {
        _logger = logger;
        _folderImagini = Path.Combine(
            FileSystem.Current.AppDataDirectory,
            "imagini_cuvinte");

        if (!Directory.Exists(_folderImagini))
        {
            Directory.CreateDirectory(_folderImagini);
            _logger.LogInformation("Folder imagini creat: {Path}", _folderImagini);
        }
     
    }

    // ─── Construire filtru fișiere reutilizabil ───
    private static FilePickerFileType GetTipuriImagini() =>
        new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI,   new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" } },
            { DevicePlatform.Android, new[] { "image/*" } },
            { DevicePlatform.iOS,     new[] { "public.image" } },
            { DevicePlatform.macOS,   new[] { "public.image" } }
        });

    // ─── O singură imagine (rămâne neschimbată funcțional) ───
    public async Task<string?> AlegeSiSalveazaAsync()
    {
        try
        {
            var optiuni = new PickOptions
            {
                PickerTitle = "Alege o imagine",
                FileTypes = GetTipuriImagini()
            };

            var rezultat = await FilePicker.Default.PickAsync(optiuni);
            if (rezultat == null) return null;

            return await SalveazaFisierAsync(rezultat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la alegere/salvare imagine");
            await Application.Current!.MainPage!.DisplayAlert(
                "Eroare", "Nu s-a putut salva imaginea. Reîncearcă.", "OK");
            return null;
        }
    }

    // ─── Multiple imagini ───
    public async Task<List<string>> AlegeSiSalveazaMultipleAsync(int maxImagini = 2)
    {
        var rezultate = new List<string>();
        try
        {
            var optiuni = new PickOptions
            {
                PickerTitle = $"Alege până la {maxImagini} imagini",
                FileTypes = GetTipuriImagini()
            };

            var fisiere = await FilePicker.Default.PickMultipleAsync(optiuni);
            if (fisiere == null) return rezultate;

            foreach (var fisier in fisiere.Take(maxImagini))
            {
                string? cale = await SalveazaFisierAsync(fisier);
                if (!string.IsNullOrEmpty(cale))
                    rezultate.Add(cale);
            }

            if (rezultate.Count == 0 && fisiere.Any())
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Atenție",
                    "Niciuna dintre imagini nu a putut fi salvată.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la alegere multiple imagini");
            await Application.Current!.MainPage!.DisplayAlert(
                "Eroare", "Nu s-au putut salva imaginile.", "OK");
        }
        return rezultate;
    }

    // ─── Logica unificată de validare + salvare un fișier ───
    private async Task<string?> SalveazaFisierAsync(FileResult fisier)
    {
        try
        {
            string extensie = Path.GetExtension(fisier.FileName).ToLowerInvariant();
            if (!ExtensiiPermise.Contains(extensie))
            {
                _logger.LogWarning("Extensie respinsă: {Ext} pentru {Nume}",
                    extensie, fisier.FileName);
                return null;
            }

            string numeUnic = $"{Guid.NewGuid():N}{extensie}";
            string caleDestinatie = Path.Combine(_folderImagini, numeUnic);

            using (var streamSursa = await fisier.OpenReadAsync())
            using (var streamDestinatie = File.Create(caleDestinatie))
            {
                await streamSursa.CopyToAsync(streamDestinatie);
            }

            _logger.LogInformation("Imagine salvată: {Nume} ({Size} bytes)",
                numeUnic, new FileInfo(caleDestinatie).Length);

            return numeUnic;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la salvarea fișierului {Nume}", fisier.FileName);
            return null;
        }
    }

    public string GetCaleAbsoluta(string numeFisier)
    {
        if (string.IsNullOrWhiteSpace(numeFisier)) return string.Empty;

        // Verifică dacă există local
        var caleLocala = Path.Combine(_folderImagini, numeFisier);
        if (File.Exists(caleLocala))
            return caleLocala;

        // Fallback — din Resources
#if ANDROID
        return $"http://10.0.2.2:5202/imagini/{numeFisier}";
#else
    return Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Resources", "ImagesCuvinte", numeFisier);
#endif
    }

    public void Sterge(string numeFisier)
    {
        if (string.IsNullOrWhiteSpace(numeFisier)) return;
        try
        {
            var cale = Path.Combine(_folderImagini, numeFisier);
            if (File.Exists(cale))
            {
                File.Delete(cale);
                _logger.LogInformation("Imagine ștearsă: {Nume}", numeFisier);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nu s-a putut șterge imaginea {Nume}", numeFisier);
        }
    }

    public bool Exista(string numeFisier)
    {
        if (string.IsNullOrWhiteSpace(numeFisier)) return false;
        return File.Exists(Path.Combine(_folderImagini, numeFisier));
    }
}