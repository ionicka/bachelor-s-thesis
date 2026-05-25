using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlashCards.Core.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _repo;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IAdminRepository repo, ILogger<AdminService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<CuvantListaDto>> GetCuvinteAsync(FiltruCuvinteDto? filtru = null) =>
        await _repo.GetCuvinteAsync(filtru);

    public async Task<CuvantEditDto?> GetCuvantPentruEditAsync(int id) =>
        await _repo.GetCuvantPentruEditAsync(id);

    public async Task<int> GetNrTotalCuvinteAsync() =>
        await _repo.GetNrTotalCuvinteAsync();

    public async Task<Dictionary<DomeniuCuvant, int>> GetNrPeDomeniiAsync() =>
        await _repo.GetNrPeDomeniiAsync();

    public async Task<RezultatOperatieDto> CreeazaCuvantAsync(CuvantEditDto dto)
    {
        // Validare
        var validare = ValideazaDto(dto);
        if (!validare.Succes) return validare;

        // Verifică unicitate termen
        if (await _repo.ExistaTermenAsync(dto.Termen))
            return new RezultatOperatieDto(false,
                $"Cuvântul '{dto.Termen.Trim()}' există deja în baza de date.", null);

        try
        {
            int idNou = await _repo.CreeazaCuvantAsync(dto);
            _logger.LogInformation("Cuvant creat: '{Termen}' (Id={Id})", dto.Termen, idNou);
            return new RezultatOperatieDto(true, "Cuvânt adăugat cu succes!", idNou);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la creare cuvant '{Termen}'", dto.Termen);
            return new RezultatOperatieDto(false,
                "Eroare la salvare. Verifică datele și reîncearcă.", null);
        }
    }

    public async Task<RezultatOperatieDto> ActualizeazaCuvantAsync(CuvantEditDto dto)
    {
        if (!dto.Id.HasValue)
            return new RezultatOperatieDto(false, "Id lipsă — nu se poate actualiza.", null);

        var validare = ValideazaDto(dto);
        if (!validare.Succes) return validare;

        // Verifică unicitate termen — DAR exclude cuvântul curent
        if (await _repo.ExistaTermenAsync(dto.Termen, exceptId: dto.Id))
            return new RezultatOperatieDto(false,
                $"Există deja alt cuvânt cu termenul '{dto.Termen.Trim()}'.", null);

        try
        {
            await _repo.ActualizeazaCuvantAsync(dto);
            _logger.LogInformation("Cuvant actualizat: Id={Id}, Termen='{Termen}'", dto.Id, dto.Termen);
            return new RezultatOperatieDto(true, "Modificări salvate cu succes!", dto.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la actualizare cuvant Id={Id}", dto.Id);
            return new RezultatOperatieDto(false,
                "Eroare la salvare. Verifică datele și reîncearcă.", null);
        }
    }

    public async Task<RezultatOperatieDto> StergeCuvantAsync(int id)
    {
        try
        {
            await _repo.StergeCuvantAsync(id);
            _logger.LogInformation("Cuvant sters: Id={Id}", id);
            return new RezultatOperatieDto(true, "Cuvânt șters.", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la stergere cuvant Id={Id}", id);
            return new RezultatOperatieDto(false,
                "Eroare la ștergere.", null);
        }
    }

    // ─── Validare centralizată ─────────────────────────────────
    private static RezultatOperatieDto ValideazaDto(CuvantEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Termen))
            return new RezultatOperatieDto(false, "Termenul este obligatoriu.", null);

        if (dto.Termen.Length > 200)
            return new RezultatOperatieDto(false, "Termenul depășește 200 caractere.", null);

        if (string.IsNullOrWhiteSpace(dto.Definitie))
            return new RezultatOperatieDto(false, "Definiția (engleză) este obligatorie.", null);

        // Filtrăm exemplele goale și le numărăm pe cele valide
        var exempleValide = dto.Exemple
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();

        if (exempleValide.Count < 1)
            return new RezultatOperatieDto(false,
                "Trebuie cel puțin un exemplu de propoziție.", null);

        if (exempleValide.Count > 5)
            return new RezultatOperatieDto(false,
                "Maxim 5 exemple permise.", null);

        // Fiecare exemplu trebuie să conțină [TERMEN]
        for (int i = 0; i < exempleValide.Count; i++)
        {
            if (!exempleValide[i].Contains("[TERMEN]"))
                return new RezultatOperatieDto(false,
                    $"Exemplul {i + 1} trebuie să conțină marcajul [TERMEN].", null);
        }

        if (dto.Imagini.Count > 2)
            return new RezultatOperatieDto(false,
                "Maxim 2 imagini permise per cuvânt.", null);

        return new RezultatOperatieDto(true, null, null);
    }
}