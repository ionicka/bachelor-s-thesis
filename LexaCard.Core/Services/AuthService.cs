using LexaCard.Core.DTOs;
using LexaCard.Core.Entities;
using LexaCard.Core.Interfaces;

namespace LexaCard.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUtilizatorRepository _repo;

    public AuthService(IUtilizatorRepository repo) => _repo = repo;

    public async Task<(bool Succes, UtilizatorDto? Utilizator, string? Eroare)>
        LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Parola))
            return (false, null, "Email si parola sunt obligatorii.");

        var u = await _repo.GetByEmailAsync(dto.Email.Trim().ToLower());
        if (u == null)
            return (false, null, "Email sau parola incorecta.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Parola, u.ParolaHash))
            return (false, null, "Email sau parola incorecta.");

        await _repo.ActualizeazaUltimaAutentificareAsync(u.Id);
        return (true, MapToDto(u), null);
    }

    public async Task<(bool Succes, string? Eroare)>
        InregistreazaAsync(InregistrareDto dto)
    {
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

        string hash = BCrypt.Net.BCrypt.HashPassword(dto.Parola, workFactor: 12);
        await _repo.CreeazaAsync(new Utilizator
        {
            NumeUtilizator = dto.NumeUtilizator.Trim(),
            Email          = dto.Email.Trim().ToLower(),
            ParolaHash     = hash
        });
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
        u.CarduriNoiPerZi      = Math.Clamp(carduriNoi, 1, 50);
        u.MaxCarduriPerSesiune = Math.Clamp(maxCarduri, 5, 100);
        await _repo.ActualizeazaAsync(u);
    }

    private static UtilizatorDto MapToDto(Utilizator u) =>
        new(u.Id, u.NumeUtilizator, u.Email,
            u.CarduriNoiPerZi, u.MaxCarduriPerSesiune);
}
