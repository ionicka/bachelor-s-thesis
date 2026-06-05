using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using System.Net.Http.Json;

namespace FlashCards.Services.Http;

public class CardServiceHttp : ICardService
{
    private readonly HttpClient _http;

    public CardServiceHttp(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CardDto>> GetSesiuneAsync(
        int utilizatorId, int cuvinteNoi, int maxRevizuiri = 100)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<CardDto>>(
                $"api/cards/sesiune/{utilizatorId}?cuvinteNoi={cuvinteNoi}&maxRevizuiri={maxRevizuiri}")
                ?? new();
        }
        catch { return new(); }
    }

    public async Task<RezultatRaspunsDto> ProceseazaRaspunsAsync(
        int utilizatorId, RaspunsCardDto dto)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/cards/raspuns/{utilizatorId}", dto);
        return await response.Content.ReadFromJsonAsync<RezultatRaspunsDto>()
            ?? throw new Exception("Raspuns invalid de la server.");
    }

    public async Task<StatisticiDto> GetStatisticiAsync(int utilizatorId)
    {
        return await _http.GetFromJsonAsync<StatisticiDto>(
            $"api/cards/statistici/{utilizatorId}")
            ?? throw new Exception("Statistici invalide.");
    }

    public async Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId)
    {
        return await _http.GetFromJsonAsync<List<CardDto>>(
            $"api/cards/toate/{utilizatorId}") ?? new();
    }

    public async Task<List<CardDto>> GetSesiuneFiltrataAsync(
        int utilizatorId, ConfigSesiuneDto config)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/cards/sesiune-filtrata/{utilizatorId}", config);
        return await response.Content.ReadFromJsonAsync<List<CardDto>>() ?? new();
    }

    public async Task<DisponibilitateSesiuneDto> GetDisponibilitateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii)
    {
        var niv = string.Join("&", niveluri.Select(n => $"niveluri={(int)n}"));
        var dom = string.Join("&", domenii.Select(d => $"domenii={(int)d}"));
        var query = string.Join("&", new[] { niv, dom }.Where(s => s.Length > 0));
        var url = $"api/cards/disponibilitate/{utilizatorId}" +
                  (query.Length > 0 ? $"?{query}" : "");

        return await _http.GetFromJsonAsync<DisponibilitateSesiuneDto>(url)
            ?? new DisponibilitateSesiuneDto(0, 0, 0);
    }
}