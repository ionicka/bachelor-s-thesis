using LexaCard.Core.DTOs;
using LexaCard.Core.Entities;
using LexaCard.Core.Enums;
using LexaCard.Core.Interfaces;

namespace LexaCard.Core.Services;

public class CardService : ICardService
{
    private readonly ICardRepository    _cardRepo;
    private readonly IProgresRepository _progresRepo;
    private readonly ISesiuneRepository _sesiuneRepo;
    private readonly IRaspunsRepository _raspunsRepo;
    private readonly ISrsService        _srs;

    public CardService(
        ICardRepository    cardRepo,
        IProgresRepository progresRepo,
        ISesiuneRepository sesiuneRepo,
        IRaspunsRepository raspunsRepo,
        ISrsService        srs)
    {
        _cardRepo    = cardRepo;
        _progresRepo = progresRepo;
        _sesiuneRepo = sesiuneRepo;
        _raspunsRepo = raspunsRepo;
        _srs         = srs;
    }

    public async Task<List<CardDto>> GetFluxAsync(int utilizatorId) =>
        await _cardRepo.GetCarduriPentruAziAsync(utilizatorId, 20);

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

        var progres = await _progresRepo.GetSauCreeazaAsync(
            utilizatorId, dto.CuvantId);
        var srs = _srs.CalculeazaUrmatoareaRevizuire(progres, calitateFinal);

        progres.NivelCunoastere         = srs.NivelNou;
        progres.IntervalCurentZile      = srs.IntervalNou;
        progres.DataUrmatoareiRevizuiri = srs.DataUrmatoareiRevizuiri;
        progres.DataUltimeiRevizuiri    = DateTime.UtcNow;

        if (calitateFinal != CalitatRaspuns.Nu_Stiu)
            progres.NrRaspunsuriCorecte++;
        else
            progres.NrRaspunsuriGresite++;

        await _progresRepo.SalveazaAsync(progres);

        await _raspunsRepo.SalveazaAsync(new RaspunsDetaliat
        {
            ProgresCuvantId = progres.Id,
            SesiuneId       = dto.SesiuneId,
            TipRaspuns      = dto.TipRaspuns,
            Calitate        = calitateFinal,
            EsteCorect      = calitateFinal != CalitatRaspuns.Nu_Stiu,
            TimpRaspunsSec  = dto.TimpRaspunsSec,
            TextTastat      = dto.TextTastat,
            Timestamp       = DateTime.UtcNow
        });

        return new RezultatRaspunsDto(
            srs.NivelNou, srs.IntervalNou,
            srs.DataUrmatoareiRevizuiri,
            srs.NivelNou >= 7,
            GeneraMesaj(srs.NivelNou, calitateFinal));
    }

    public async Task<StatisticiDto> GetStatisticiAsync(int utilizatorId)
    {
        var progrese = await _progresRepo.GetToateProgreselAsync(utilizatorId);
        var sesiuni  = await _sesiuneRepo.GetIstoricAsync(utilizatorId, 7);

        int total       = progrese.Count;
        int invatate    = progrese.Count(p => p.NivelCunoastere >= 5);
        int consolidate = progrese.Count(p => p.NivelCunoastere >= 7);
        int deRevizuit  = progrese.Count(p => p.EsteDeRevizuitAzi);
        double rata     = total == 0 ? 0 : progrese.Average(p => p.RataSucces);

        var zile   = sesiuni.Select(s => DateOnly.FromDateTime(s.DataSesiunii))
                            .Distinct().OrderByDescending(d => d).ToList();
        int streak = CalculeazaStreak(zile);

        var istoric = sesiuni
            .GroupBy(s => DateOnly.FromDateTime(s.DataSesiunii))
            .Select(g => new StatisticiZilniceDto(
                g.Key,
                g.Sum(s => s.NrCarduriVazute),
                g.Sum(s => s.NrCorect),
                g.Sum(s => s.NrCorect + s.NrGresit) == 0 ? 0
                    : Math.Round((double)g.Sum(s => s.NrCorect) /
                                 g.Sum(s => s.NrCorect + s.NrGresit) * 100, 1)))
            .OrderBy(s => s.Data).ToList();

        return new StatisticiDto(total, invatate, consolidate,
            deRevizuit, Math.Round(rata, 1), streak, istoric);
    }

    private static int CalculeazaStreak(List<DateOnly> zile)
    {
        if (!zile.Any()) return 0;
        var azi = DateOnly.FromDateTime(DateTime.UtcNow);
        if (zile.First() < azi.AddDays(-1)) return 0;
        int streak = 0;
        var zi = azi;
        foreach (var z in zile)
        {
            if (z == zi || z == zi.AddDays(-1)) { streak++; zi = z.AddDays(-1); }
            else break;
        }
        return streak;
    }

    private static string GeneraMesaj(byte nivel, CalitatRaspuns calitate) =>
        nivel >= 7 ? "Felicitari! Cuvant consolidat!" :
        calitate switch
        {
            CalitatRaspuns.Tastat_Corect => "Excelent! Ai tastat corect!",
            CalitatRaspuns.Stiu_Sigur    => "Foarte bine! Continua asa!",
            CalitatRaspuns.Stiu_Ezitare  => "Bine! Revii curand.",
            CalitatRaspuns.Nu_Stiu       => "Nu-i bai! Revii in cateva zile.",
            _ => ""
        };
}
