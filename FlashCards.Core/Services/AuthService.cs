using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlashCards.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUtilizatorRepository _repo;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUtilizatorRepository repo, ILogger<AuthService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<(bool Succes, UtilizatorDto? Utilizator, string? Eroare)>
        LoginAsync(LoginDto dto)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Login attempt: {Email}", dto.Email);

        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Parola))
            return (false, null, "Email si parola sunt obligatorii.");

        var u = await _repo.GetByEmailAsync(dto.Email.Trim().ToLower());
        if (u == null)
        {
            _logger.LogWarning("Login failed (user not found): {Email}", dto.Email);
            return (false, null, "Email sau parola incorecta.");
        }

        bool parolaCorecta = BCrypt.Net.BCrypt.Verify(dto.Parola, u.ParolaHash);
        if (!parolaCorecta)
        {
            _logger.LogWarning("Login failed (wrong password): {Email}", dto.Email);
            return (false, null, "Email sau parola incorecta.");
        }

        await _repo.ActualizeazaUltimaAutentificareAsync(u.Id);

        sw.Stop();
        _logger.LogInformation("Login success: {Email} in {Ms}ms", dto.Email, sw.ElapsedMilliseconds);

        return (true, MapToDto(u), null);
    }

    public async Task<(bool Succes, string? Eroare)>
        InregistreazaAsync(InregistrareDto dto)
    {
        _logger.LogInformation("Registration attempt: {Email}", dto.Email);

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

        string hash = BCrypt.Net.BCrypt.HashPassword(dto.Parola, workFactor: 10);
        await _repo.CreeazaAsync(new Utilizator
        {
            NumeUtilizator = dto.NumeUtilizator.Trim(),
            Email = dto.Email.Trim().ToLower(),
            ParolaHash = hash
        });

        _logger.LogInformation("Registration success: {Email}", dto.Email);
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
        u.CarduriNoiPerZi, u.MaxCarduriPerSesiune, u.Rol);
}