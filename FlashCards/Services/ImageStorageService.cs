using Microsoft.Extensions.Logging;

namespace FlashCards.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly ILogger<ImageStorageService> _logger;
    private readonly string _folderImagini;

    // Extensii acceptate
    private static readonly string[] ExtensiiPermise =
        { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    public ImageStorageService(ILogger<ImageStorageService> logger)
    {
        _logger = logger;
        _folderImagini = Path.Combine(
            FileSystem.Current.AppDataDirectory,
            "imagini_cuvinte");

        // Asigură că folderul există
        if (!Directory.Exists(_folderImagini))
        {
            Directory.CreateDirectory(_folderImagini);
            _logger.LogInformation("Folder imagini creat: {Path}", _folderImagini);
        }
    }

    public async Task<string?> AlegeSiSalveazaAsync()
    {
        try
        {
            var optiuni = new PickOptions
            {
                PickerTitle = "Alege o imagine",
                FileTypes = FilePickerFileType.Images
            };

            var rezultat = await FilePicker.Default.PickAsync(optiuni);
            if (rezultat == null) return null;  // user a anulat

            // Validare extensie
            string extensie = Path.GetExtension(rezultat.FileName).ToLowerInvariant();
            if (!ExtensiiPermise.Contains(extensie))
            {
                _logger.LogWarning("Extensie respinsă: {Ext}", extensie);
                await Application.Current!.MainPage!.DisplayAlert(
                    "Format nesuportat",
                    $"Doar imagini: {string.Join(", ", ExtensiiPermise)}",
                    "OK");
                return null;
            }

            // Generează nume unic — evităm coliziuni dacă admin alege 2 imagini cu același nume
            string numeUnic = $"{Guid.NewGuid():N}{extensie}";
            string caleDestinatie = Path.Combine(_folderImagini, numeUnic);

            // Copiază fișierul
            using (var streamSursa = await rezultat.OpenReadAsync())
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
            _logger.LogError(ex, "Eroare la alegere/salvare imagine");
            await Application.Current!.MainPage!.DisplayAlert(
                "Eroare",
                "Nu s-a putut salva imaginea. Reîncearcă.",
                "OK");
            return null;
        }
    }

    public string GetCaleAbsoluta(string numeFisier)
    {
        if (string.IsNullOrWhiteSpace(numeFisier)) return string.Empty;
        return Path.Combine(_folderImagini, numeFisier);
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