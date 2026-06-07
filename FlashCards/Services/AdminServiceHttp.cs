using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using System.Net.Http.Json;

namespace FlashCards.Services.Http;

public class AdminServiceHttp : IAdminService
{
    private readonly HttpClient _http;
    public AdminServiceHttp(HttpClient http) => _http = http;

    public async Task<List<CuvantListaDto>> GetCuvinteAsync(FiltruCuvinteDto? filtru = null)
    {
        if (filtru == null)
            return await _http.GetFromJsonAsync<List<CuvantListaDto>>("api/admin/cuvinte") ?? new();
        var response = await _http.PostAsJsonAsync("api/admin/cuvinte/filtru", filtru);
        return await response.Content.ReadFromJsonAsync<List<CuvantListaDto>>() ?? new();
    }

    public async Task<CuvantEditDto?> GetCuvantPentruEditAsync(int id) =>
        await _http.GetFromJsonAsync<CuvantEditDto>($"api/admin/cuvinte/{id}");

    public async Task<RezultatOperatieDto> CreeazaCuvantAsync(CuvantEditDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/admin/cuvinte", dto);
        return await response.Content.ReadFromJsonAsync<RezultatOperatieDto>()
            ?? new RezultatOperatieDto(false, "Eroare", null);
    }

    public async Task<RezultatOperatieDto> ActualizeazaCuvantAsync(CuvantEditDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/admin/cuvinte/{dto.Id}", dto);
        return await response.Content.ReadFromJsonAsync<RezultatOperatieDto>()
            ?? new RezultatOperatieDto(false, "Eroare", null);
    }

    public async Task<RezultatOperatieDto> StergeCuvantAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/admin/cuvinte/{id}");
        return await response.Content.ReadFromJsonAsync<RezultatOperatieDto>()
            ?? new RezultatOperatieDto(false, "Eroare", null);
    }

    public async Task<int> GetNrTotalCuvinteAsync() =>
        await _http.GetFromJsonAsync<int>("api/admin/cuvinte/total");

    public async Task<Dictionary<DomeniuCuvant, int>> GetNrPeDomeniiAsync() =>
        await _http.GetFromJsonAsync<Dictionary<DomeniuCuvant, int>>("api/admin/cuvinte/domenii") ?? new();
}