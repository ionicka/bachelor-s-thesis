namespace FlashCards.Services;

public interface IImageStorageService
{
    /// <summary>
    /// Deschide FilePicker, lasă userul să aleagă o imagine,
    /// o copiază local în folderul aplicației și returnează numele fișierului
    /// (relativ - de salvat în BD).
    /// Returnează null dacă userul anulează.
    /// </summary>
    Task<string?> AlegeSiSalveazaAsync();
    Task<List<string>> AlegeSiSalveazaMultipleAsync(int maxImagini = 2);

    /// <summary>
    /// Calea absolută a unei imagini salvate (pentru Image.Source în XAML).
    /// </summary>
    string GetCaleAbsoluta(string numeFisier);

    /// <summary>
    /// Șterge o imagine din storage local.
    /// Nu aruncă excepție dacă nu există.
    /// </summary>
    void Sterge(string numeFisier);

    /// <summary>
    /// Verifică dacă fișierul există în storage.
    /// </summary>
    bool Exista(string numeFisier);

}
