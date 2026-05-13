using LexaCard.Core.DTOs;
using LexaCard.Core.Entities;
using LexaCard.Core.Enums;

namespace LexaCard.Core.Interfaces;

public interface ICardRepository
{
    Task<List<CardDto>> GetCarduriPentruAziAsync(int utilizatorId, int max = 20);
    Task<CardDto?> GetCardAsync(int utilizatorId, int cuvantId);
}

public interface IProgresRepository
{
    Task<ProgresCuvant?> GetAsync(int utilizatorId, int cuvantId);
    Task<ProgresCuvant> GetSauCreeazaAsync(int utilizatorId, int cuvantId);
    Task SalveazaAsync(ProgresCuvant progres);
    Task<List<ProgresCuvant>> GetToateProgreselAsync(int utilizatorId);
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
    Task<List<CardDto>> GetFluxAsync(int utilizatorId);
    Task<RezultatRaspunsDto> ProceseazaRaspunsAsync(
        int utilizatorId, RaspunsCardDto raspuns);
    Task<StatisticiDto> GetStatisticiAsync(int utilizatorId);
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
    Task<int> IncepeSesuineAsync(int utilizatorId);
    Task InchideSesiuneAsync(int sesiuneId, int nrVazute, int nrCorect, int nrGresit);
    Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId);
}
