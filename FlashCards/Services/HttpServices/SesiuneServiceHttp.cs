using FlashCards.Core.Entities;
using FlashCards.Core.Interfaces;
using System.Net.Http.Json;

namespace FlashCards.Services.Http;

public class SesiuneServiceHttp : ISesiuneService
{
    private readonly HttpClient _http;

    public SesiuneServiceHttp(HttpClient http)
    {
        _http = http;
    }

    public async Task<int> IncepeSesiuneAsync(int utilizatorId)
    {
        var response = await _http.PostAsync(
            $"api/sesiune/incepe/{utilizatorId}", null);
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task InchideSesiuneAsync(
        int sesiuneId, int nrVazute, int nrCorect, int nrGresit)
    {
        await _http.PutAsync(
            $"api/sesiune/inchide/{sesiuneId}?nrVazute={nrVazute}&nrCorect={nrCorect}&nrGresit={nrGresit}",
            null);
    }

    public async Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId)
    {
        return await _http.GetFromJsonAsync<List<SesiuneStudiu>>(
            $"api/sesiune/istoric/{utilizatorId}") ?? new();
    }
}