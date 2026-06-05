using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlashCards.Core.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepo;
    private readonly IProgresRepository _progresRepo;
    private readonly ISesiuneRepository _sesiuneRepo;
    private readonly IRaspunsRepository _raspunsRepo;
    private readonly ISrsService _srs;
    private readonly ILogger<CardService> _logger;

    public CardService(
        ICardRepository cardRepo,
        IProgresRepository progresRepo,
        ISesiuneRepository sesiuneRepo,
        IRaspunsRepository raspunsRepo,
        ISrsService srs,
        ILogger<CardService> logger)
    {
        _cardRepo = cardRepo;
        _progresRepo = progresRepo;
        _sesiuneRepo = sesiuneRepo;
        _raspunsRepo = raspunsRepo;
        _srs = srs;
        _logger = logger;
    }

    public async Task<List<CardDto>> GetSesiuneAsync(
        int utilizatorId, int cuvinteNoi, int maxRevizuiri = 100)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var toateRevizuirile = await _cardRepo.GetRevizuiriAziAsync(utilizatorId);
        var revizuiri = toateRevizuirile.Take(maxRevizuiri).ToList();
        var noi = await _cardRepo.GetCuvinteNoiAsync(utilizatorId, cuvinteNoi);
        sw.Stop();

        _logger.LogInformation(
            "Sesiune incarcata pentru user {UserId}: {Revizuiri} revizuiri + {Noi} noi in {Ms}ms",
            utilizatorId, revizuiri.Count, noi.Count, sw.ElapsedMilliseconds);

        return revizuiri.Concat(noi).ToList();
    }

    public async Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId) =>
        await _cardRepo.GetToateCuvinteleAsync(utilizatorId);

    public async Task<RezultatRaspunsDto> ProceseazaRaspunsAsync(
        int utilizatorId, RaspunsCardDto dto)
    {
        var calitateFinal = dto.Calitate;

        if (dto.TipRaspuns == TipRaspuns.RemintireActiva &&
            !string.IsNullOrWhiteSpace(dto.TextTastat))
        {
            var card = await _cardRepo.GetCardAsync(utilizatorId, dto.CuvantId);
            if (card != null)
                calitateFinal = _srs.VerificaTextTastat(dto.TextTastat, card.Termen)
                    ? CalitatRaspuns.Tastat_Corect
                    : CalitatRaspuns.Nu_Stiu;
        }

        var progres = await _progresRepo.GetSauCreeazaAsync(utilizatorId, dto.CuvantId);
        var srs = _srs.CalculeazaUrmatoareaRevizuire(progres, calitateFinal);

        progres.NivelCunoastere = srs.NivelNou;
        progres.IntervalCurentZile = srs.IntervalNou;
        progres.DataUrmatoareiRevizuiri = srs.DataUrmatoareiRevizuiri;
        progres.DataUltimeiRevizuiri = DateTime.UtcNow;

        if (calitateFinal != CalitatRaspuns.Nu_Stiu)
            progres.NrRaspunsuriCorecte++;
        else
            progres.NrRaspunsuriGresite++;

        await _progresRepo.SalveazaAsync(progres);

        await _raspunsRepo.SalveazaAsync(new RaspunsDetaliat
        {
            ProgresCuvantId = progres.Id,
            SesiuneId = dto.SesiuneId,
            TipRaspuns = dto.TipRaspuns,
            Calitate = calitateFinal,
            EsteCorect = calitateFinal != CalitatRaspuns.Nu_Stiu,
            TimpRaspunsSec = dto.TimpRaspunsSec,
            TextTastat = dto.TextTastat,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogDebug("Raspuns procesat: user={UserId}, cuvant={CuvantId}, nivel {Vechi}->{Nou}",
            utilizatorId, dto.CuvantId, progres.NivelCunoastere, srs.NivelNou);

        return new RezultatRaspunsDto(
            srs.NivelNou, srs.IntervalNou, srs.DataUrmatoareiRevizuiri,
            srs.NivelNou >= 7, GeneraMesaj(srs.NivelNou, calitateFinal));
    }

    public async Task<StatisticiDto> GetStatisticiAsync(int utilizatorId)
    {
        var progrese = await _progresRepo.GetToateProgreselAsync(utilizatorId);
        var sesiuni = await _sesiuneRepo.GetIstoricAsync(utilizatorId, 30);

        int total = progrese.Count;
        int invatate = progrese.Count(p => p.NivelCunoastere >= 5);
        int consolidate = progrese.Count(p => p.NivelCunoastere >= 7);
        int deRevizuit = await _progresRepo.GetNrRevizuiriAziAsync(utilizatorId);
        int cuvinteNoi = await _progresRepo.GetNrCuvinteNoiAsync(utilizatorId);
        double rata = total == 0 ? 0 : progrese.Average(p => p.RataSucces);

        var zile = sesiuni
            .Where(s => s.DataSfarsitului.HasValue)
            .Select(s => DateOnly.FromDateTime(
                s.DataSesiunii.Kind == DateTimeKind.Utc
                    ? s.DataSesiunii.ToLocalTime()
                    : s.DataSesiunii))
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        int streak = CalculeazaStreak(zile);

        var zileCuStudiu = sesiuni
            .Where(s => s.DataSfarsitului.HasValue)
            .Select(s => DateOnly.FromDateTime(
                s.DataSesiunii.Kind == DateTimeKind.Utc
                    ? s.DataSesiunii.ToLocalTime()
                    : s.DataSesiunii))
            .Distinct()
            .ToList();

        var istoric = zileCuStudiu
            .Select(z => new StatisticiZilniceDto(z, 0, 0, 0))
            .OrderBy(s => s.Data).ToList();

        var aziLocal = DateOnly.FromDateTime(DateTime.Now);
        int sesiuniAzi = sesiuni
            .Count(s => s.DataSfarsitului.HasValue &&
                DateOnly.FromDateTime(
                    s.DataSesiunii.Kind == DateTimeKind.Utc
                        ? s.DataSesiunii.ToLocalTime()
                        : s.DataSesiunii) == aziLocal);

        return new StatisticiDto(
            total, invatate, consolidate,
            deRevizuit, cuvinteNoi,
            Math.Round(rata, 1), streak, sesiuniAzi, istoric);
    }

    private static int CalculeazaStreak(List<DateOnly> zile)
    {
        if (!zile.Any()) return 0;
        var azi = DateOnly.FromDateTime(DateTime.Now);
        var ziMaxima = zile.First();
        if (ziMaxima < azi.AddDays(-1)) return 0;

        int streak = 0;
        var asteptat = ziMaxima;
        foreach (var z in zile)
        {
            if (z == asteptat)
            {
                streak++;
                asteptat = asteptat.AddDays(-1);
            }
            else if (z < asteptat) break;
        }
        return streak;
    }

    private static string GeneraMesaj(byte nivel, CalitatRaspuns calitate) =>
        nivel >= 7 ? "Felicitari! Cuvant consolidat!" :
        calitate switch
        {
            CalitatRaspuns.Tastat_Corect => "Excelent! Ai tastat corect!",
            CalitatRaspuns.Stiu_Sigur => "Foarte bine! Continua asa!",
            CalitatRaspuns.Stiu_Ezitare => "Bine! Revii curand.",
            CalitatRaspuns.Nu_Stiu => "Nu-i bai! Revii in cateva zile.",
            _ => ""
        };

    public async Task<List<CardDto>> GetSesiuneFiltrataAsync(
    int utilizatorId, ConfigSesiuneDto config)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Prioritate maximă revizuiri, apoi noi (păstrăm logica existentă)
        var toateRevizuirile = await _cardRepo.GetRevizuiriAziFiltrateAsync(
            utilizatorId, config.Niveluri, config.Domenii);

        // Câte revizuiri încap, câte cuvinte noi mai trebuie
        var revizuiri = toateRevizuirile.Take(config.NrCarduri).ToList();
        int locuriRamase = config.NrCarduri - revizuiri.Count;

        List<CardDto> noi = new();
        if (locuriRamase > 0)
        {
            noi = await _cardRepo.GetCuvinteNoiFiltrateAsync(
                utilizatorId, locuriRamase, config.Niveluri, config.Domenii);
        }

        sw.Stop();
        _logger.LogInformation(
            "Sesiune filtrată user {UserId}: {Rev} revizuiri + {Noi} noi (filtre: {NrNiv} niv, {NrDom} dom, mod={Mod}) in {Ms}ms",
            utilizatorId, revizuiri.Count, noi.Count,
            config.Niveluri.Count, config.Domenii.Count, config.Mod, sw.ElapsedMilliseconds);

        return revizuiri.Concat(noi).ToList();
    }

    public async Task<DisponibilitateSesiuneDto> GetDisponibilitateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii)
    {
        return await _cardRepo.GetDisponibilitateAsync(utilizatorId, niveluri, domenii);
    }
    public async Task IgnoraCuvantAsync(int utilizatorId, int cuvantId)
    {
        await _progresRepo.IgnoraCuvantAsync(utilizatorId, cuvantId);
    }
}