using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Enums;

namespace FlashCards.Core.Interfaces;

public interface ICardRepository
{
    Task<List<CardDto>> GetRevizuiriAziAsync(int utilizatorId);
    Task<List<CardDto>> GetCuvinteNoiAsync(int utilizatorId, int max);
    Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId);
    Task<CardDto?> GetCardAsync(int utilizatorId, int cuvantId);

    // ─── NOI ───
    Task<List<CardDto>> GetRevizuiriAziFiltrateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii);

    Task<List<CardDto>> GetCuvinteNoiFiltrateAsync(
        int utilizatorId,
        int max,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii);

    Task<DisponibilitateSesiuneDto> GetDisponibilitateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii);
}

public interface IProgresRepository
{
    Task<ProgresCuvant?> GetAsync(int utilizatorId, int cuvantId);
    Task<ProgresCuvant> GetSauCreeazaAsync(int utilizatorId, int cuvantId);
    Task SalveazaAsync(ProgresCuvant progres);
    Task<List<ProgresCuvant>> GetToateProgreselAsync(int utilizatorId);
    Task<int> GetNrRevizuiriAziAsync(int utilizatorId);
    Task<int> GetNrCuvinteNoiAsync(int utilizatorId);
}

public interface IUtilizatorRepository
{
    Task<Utilizator?> GetByEmailAsync(string email);
    Task<Utilizator?> GetByIdAsync(int id);
    Task<bool> ExistaEmailAsync(string email);
    Task<bool> ExistaNumeAsync(string nume);
    Task<Utilizator> CreeazaAsync(Utilizator utilizator);
    Task ActualizeazaAsync(Utilizator utilizator);
    Task ActualizeazaUltimaAutentificareAsync(int utilizatorId);
}

public interface ISesiuneRepository
{
    Task<SesiuneStudiu> IncepeAsync(int utilizatorId);
    Task<SesiuneStudiu?> GetAsync(int sesiuneId);
    Task ActualizeazaAsync(SesiuneStudiu sesiune);
    Task InchideAsync(int sesiuneId);
    Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId, int ultimeleZile = 7);
}

public interface IRaspunsRepository
{
    Task SalveazaAsync(RaspunsDetaliat raspuns);
}

public interface ISrsService
{
    RezultatSrs CalculeazaUrmatoareaRevizuire(
        ProgresCuvant progres, CalitatRaspuns calitate);
    TipRaspuns DeterminaTipRaspuns(ProgresCuvant progres);
    bool VerificaTextTastat(string textTastat, string terminCorect);
}

public interface ICardService
{
    Task<List<CardDto>> GetSesiuneAsync(
        int utilizatorId, int cuvinteNoi, int maxRevizuiri = 100);

    Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId);

    Task<RezultatRaspunsDto> ProceseazaRaspunsAsync(
        int utilizatorId, RaspunsCardDto raspuns);

    Task<StatisticiDto> GetStatisticiAsync(int utilizatorId);

    // ─── NOI ───
    Task<List<CardDto>> GetSesiuneFiltrataAsync(
        int utilizatorId, ConfigSesiuneDto config);

    Task<DisponibilitateSesiuneDto> GetDisponibilitateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii);
}

public interface IAuthService
{
    Task<(bool Succes, UtilizatorDto? Utilizator, string? Eroare)> LoginAsync(LoginDto dto);
    Task<(bool Succes, string? Eroare)> InregistreazaAsync(InregistrareDto dto);
    Task<UtilizatorDto?> GetUtilizatorCurentAsync(int utilizatorId);
    Task ActualizeazaSetariAsync(int utilizatorId, int carduriNoi, int maxCarduri);
}

public interface ISesiuneService
{
    Task<int> IncepeSesiuneAsync(int utilizatorId);
    Task InchideSesiuneAsync(int sesiuneId, int nrVazute, int nrCorect, int nrGresit);
    Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId);
}

public interface IAdminRepository
{
    // Listare cu filtrare
    Task<List<CuvantListaDto>> GetCuvinteAsync(FiltruCuvinteDto? filtru = null);

    // Pentru formularul de edit — încarcă tot ce trebuie pentru pre-populare
    Task<CuvantEditDto?> GetCuvantPentruEditAsync(int id);

    // CRUD
    Task<int> CreeazaCuvantAsync(CuvantEditDto dto);
    Task ActualizeazaCuvantAsync(CuvantEditDto dto);
    Task StergeCuvantAsync(int id);

    // Verificare unicitate (la add și edit)
    Task<bool> ExistaTermenAsync(string termen, int? exceptId = null);

    // Numărători pentru dashboard admin (opțional, util)
    Task<int> GetNrTotalCuvinteAsync();
    Task<Dictionary<DomeniuCuvant, int>> GetNrPeDomeniiAsync();
}

public interface IAdminService
{
    // Listare
    Task<List<CuvantListaDto>> GetCuvinteAsync(FiltruCuvinteDto? filtru = null);

    // Pentru pagina de edit
    Task<CuvantEditDto?> GetCuvantPentruEditAsync(int id);

    // Operații cu validare
    Task<RezultatOperatieDto> CreeazaCuvantAsync(CuvantEditDto dto);
    Task<RezultatOperatieDto> ActualizeazaCuvantAsync(CuvantEditDto dto);
    Task<RezultatOperatieDto> StergeCuvantAsync(int id);

    // Statistici
    Task<int> GetNrTotalCuvinteAsync();
    Task<Dictionary<DomeniuCuvant, int>> GetNrPeDomeniiAsync();
}