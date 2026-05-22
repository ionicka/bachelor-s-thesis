using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Interfaces;


namespace FlashCards.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUtilizatorRepository _repo;

    public AuthService(IUtilizatorRepository repo) => _repo = repo;

    public async Task<(bool Succes, UtilizatorDto? Utilizator, string? Eroare)>
        LoginAsync(LoginDto dto)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK] LoginAsync ({dto.Email})");

        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Parola))
            return (false, null, "Email si parola sunt obligatorii.");

        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var u = await _repo.GetByEmailAsync(dto.Email.Trim().ToLower());
        sw1.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   GetByEmail (EF Core): {sw1.ElapsedMilliseconds} ms");

        if (u == null)
            return (false, null, "Email sau parola incorecta.");

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        bool parolaCorecta = BCrypt.Net.BCrypt.Verify(dto.Parola, u.ParolaHash);
        sw2.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   BCrypt.Verify (hash parola): {sw2.ElapsedMilliseconds} ms");

        if (!parolaCorecta)
            return (false, null, "Email sau parola incorecta.");

        await _repo.ActualizeazaUltimaAutentificareAsync(u.Id);

        sw.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   TOTAL Login: {sw.ElapsedMilliseconds} ms → succes");
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");

        return (true, MapToDto(u), null);
    }

    public async Task<(bool Succes, string? Eroare)>
        InregistreazaAsync(InregistrareDto dto)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK] InregistreazaAsync ({dto.Email})");

        if (string.IsNullOrWhiteSpace(dto.NumeUtilizator))
            return (false, "Numele de utilizator este obligatoriu.");
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
            return (false, "Email invalid.");
        if (string.IsNullOrWhiteSpace(dto.Parola) || dto.Parola.Length < 6)
            return (false, "Parola trebuie sa aiba minim 6 caractere.");
        if (dto.Parola != dto.ConfirmaParola)
            return (false, "Parolele nu coincid.");
        if (await _repo.ExistaEmailAsync(dto.Email.Trim().ToLower()))
            return (false, "Exista deja un cont cu acest email.");
        if (await _repo.ExistaNumeAsync(dto.NumeUtilizator.Trim()))
            return (false, "Numele de utilizator este deja folosit.");

        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        string hash = BCrypt.Net.BCrypt.HashPassword(dto.Parola, workFactor: 12);
        sw1.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   BCrypt.HashPassword (workFactor=12): {sw1.ElapsedMilliseconds} ms");

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        await _repo.CreeazaAsync(new Utilizator
        {
            NumeUtilizator = dto.NumeUtilizator.Trim(),
            Email = dto.Email.Trim().ToLower(),
            ParolaHash = hash
        });
        sw2.Stop();

        sw.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   INSERT utilizator (EF Core): {sw2.ElapsedMilliseconds} ms");
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   TOTAL Inregistrare: {sw.ElapsedMilliseconds} ms → succes");
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");

        return (true, null);
    }

    public async Task<UtilizatorDto?> GetUtilizatorCurentAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        return u == null ? null : MapToDto(u);
    }

    public async Task ActualizeazaSetariAsync(int id, int carduriNoi, int maxCarduri)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return;
        u.CarduriNoiPerZi = Math.Clamp(carduriNoi, 1, 50);
        u.MaxCarduriPerSesiune = Math.Clamp(maxCarduri, 5, 100);
        await _repo.ActualizeazaAsync(u);
    }

    private static UtilizatorDto MapToDto(Utilizator u) =>
        new(u.Id, u.NumeUtilizator, u.Email,
            u.CarduriNoiPerZi, u.MaxCarduriPerSesiune);
}