using FlashCards.Core.DTOs;
using FlashCards.Core.Interfaces;
using System.Net.Http.Json;

namespace FlashCards.Services.Http;

public class AuthServiceHttp : IAuthService
{
    private readonly HttpClient _http;

    public AuthServiceHttp(HttpClient http)
    {
        _http = http;
    }

    public async Task<(bool Succes, UtilizatorDto? Utilizator, string? Eroare)> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorDto>();
                return (false, null, err?.Eroare ?? "Eroare necunoscuta.");
            }
            var utilizator = await response.Content.ReadFromJsonAsync<UtilizatorDto>();
            return (true, utilizator, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Eroare conexiune: {ex.Message}");
        }
    }

    public async Task<(bool Succes, string? Eroare)> InregistreazaAsync(InregistrareDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/inregistrare", dto);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorDto>();
                return (false, err?.Eroare ?? "Eroare necunoscuta.");
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Eroare conexiune: {ex.Message}");
        }
    }

    public async Task<UtilizatorDto?> GetUtilizatorCurentAsync(int utilizatorId)
    {
        try
        {
            return await _http.GetFromJsonAsync<UtilizatorDto>($"api/auth/{utilizatorId}");
        }
        catch { return null; }
    }

    public async Task ActualizeazaSetariAsync(int utilizatorId, int carduriNoi, int maxCarduri)
    {
        try
        {
            await _http.PutAsync(
                $"api/auth/setari/{utilizatorId}?carduriNoi={carduriNoi}&maxCarduri={maxCarduri}",
                null);
        }
        catch { }
    }
}

// Helper pentru erori
public class ErrorDto
{
    public string? Eroare { get; set; }
}