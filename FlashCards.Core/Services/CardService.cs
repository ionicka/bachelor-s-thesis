using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;

namespace FlashCards.Core.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepo;
    private readonly IProgresRepository _progresRepo;
    private readonly ISesiuneRepository _sesiuneRepo;
    private readonly IRaspunsRepository _raspunsRepo;
    private readonly ISrsService _srs;

    public CardService(
        ICardRepository cardRepo,
        IProgresRepository progresRepo,
        ISesiuneRepository sesiuneRepo,
        IRaspunsRepository raspunsRepo,
        ISrsService srs)
    {
        _cardRepo = cardRepo;
        _progresRepo = progresRepo;
        _sesiuneRepo = sesiuneRepo;
        _raspunsRepo = raspunsRepo;
        _srs = srs;
    }

    /// <summary>
    /// Construieste sesiunea zilnica:
    /// 1. Toate revizuirile programate pentru azi (prioritate maxima)
    /// 2. X cuvinte noi (configurate de utilizator)
    /// </summary>
    public async Task<List<CardDto>> GetSesiuneAsync(
        int utilizatorId, int cuvinteNoi, int maxRevizuiri = 100)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK] GetSesiuneAsync (utilizator {utilizatorId})");

        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var toateRevizuirile = await _cardRepo.GetRevizuiriAziAsync(utilizatorId);
        var revizuiri = toateRevizuirile.Take(maxRevizuiri).ToList();
        sw1.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   Revizuiri din BD (Dapper): {sw1.ElapsedMilliseconds} ms → {revizuiri.Count} carduri");

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var noi = await _cardRepo.GetCuvinteNoiAsync(utilizatorId, cuvinteNoi);
        sw2.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   Cuvinte noi din BD (Dapper): {sw2.ElapsedMilliseconds} ms → {noi.Count} carduri");

        sw.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   TOTAL initializare sesiune: {sw.ElapsedMilliseconds} ms ({revizuiri.Count + noi.Count} carduri total)");
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");

        return revizuiri.Concat(noi).ToList();
    }

    public async Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId) =>
        await _cardRepo.GetToateCuvinteleAsync(utilizatorId);

    public async Task<RezultatRaspunsDto> ProceseazaRaspunsAsync(
        int utilizatorId, RaspunsCardDto dto)
    {
        var calitateFinal = dto.Calitate;

        // Daca e modul tastare, verifica textul
        if (dto.TipRaspuns == TipRaspuns.RemintireActiva &&
            !string.IsNullOrWhiteSpace(dto.TextTastat))
        {
            var card = await _cardRepo.GetCardAsync(utilizatorId, dto.CuvantId);
            if (card != null)
                calitateFinal = _srs.VerificaTextTastat(dto.TextTastat, card.Termen)
                    ? CalitatRaspuns.Tastat_Corect
                    : CalitatRaspuns.Nu_Stiu;
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();

        var progres = await _progresRepo.GetSauCreeazaAsync(utilizatorId, dto.CuvantId);
        var srs = _srs.CalculeazaUrmatoareaRevizuire(progres, calitateFinal);

        // Actualizeaza progresul
        progres.NivelCunoastere = srs.NivelNou;
        progres.IntervalCurentZile = srs.IntervalNou;
        progres.DataUrmatoareiRevizuiri = srs.DataUrmatoareiRevizuiri;
        progres.DataUltimeiRevizuiri = DateTime.UtcNow;

        if (calitateFinal != CalitatRaspuns.Nu_Stiu)
            progres.NrRaspunsuriCorecte++;
        else
            progres.NrRaspunsuriGresite++;

        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        await _progresRepo.SalveazaAsync(progres);
        sw1.Stop();

        // Salveaza raspunsul detaliat
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
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
        sw2.Stop();
        sw.Stop();

        System.Diagnostics.Debug.WriteLine($"[BENCHMARK] ProceseazaRaspuns '{dto.TextTastat ?? dto.Calitate.ToString()}'" +
            $" → Nivel {srs.NivelNou} | Progres EF: {sw1.ElapsedMilliseconds}ms | Raspuns EF: {sw2.ElapsedMilliseconds}ms | Total: {sw.ElapsedMilliseconds}ms");

        return new RezultatRaspunsDto(
            srs.NivelNou,
            srs.IntervalNou,
            srs.DataUrmatoareiRevizuiri,
            srs.NivelNou >= 7,
            GeneraMesaj(srs.NivelNou, calitateFinal));
    }

    public async Task<StatisticiDto> GetStatisticiAsync(int utilizatorId)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK] GetStatisticiAsync (utilizator {utilizatorId})");

        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var progrese = await _progresRepo.GetToateProgreselAsync(utilizatorId);
        sw1.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   GetToateProgresele (EF Core): {sw1.ElapsedMilliseconds} ms → {progrese.Count} inregistrari");

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var sesiuni = await _sesiuneRepo.GetIstoricAsync(utilizatorId, 30);
        sw2.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   GetIstoric sesiuni (EF Core): {sw2.ElapsedMilliseconds} ms → {sesiuni.Count} sesiuni");

        int total = progrese.Count;
        int invatate = progrese.Count(p => p.NivelCunoastere >= 5);
        int consolidate = progrese.Count(p => p.NivelCunoastere >= 7);

        var sw3 = System.Diagnostics.Stopwatch.StartNew();
        int deRevizuit = await _progresRepo.GetNrRevizuiriAziAsync(utilizatorId);
        int cuvinteNoi = await _progresRepo.GetNrCuvinteNoiAsync(utilizatorId);
        sw3.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   Numar revizuiri+noi (EF Core): {sw3.ElapsedMilliseconds} ms");
        double rata = total == 0 ? 0 : progrese.Average(p => p.RataSucces);

        // Streak bazat DOAR pe sesiuni inchise (DataSfarsitului != null)
        // Nu folosim DataUltimeiRevizuiri pentru ca se seteaza si la prima vizualizare
        var zile = sesiuni
            .Where(s => s.DataSfarsitului.HasValue) // doar sesiuni finalizate
            .Select(s => DateOnly.FromDateTime(
                s.DataSesiunii.Kind == DateTimeKind.Utc
                    ? s.DataSesiunii.ToLocalTime()
                    : s.DataSesiunii))
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        int streak = CalculeazaStreak(zile);

        // Istoricul saptamanal - zile cu sesiuni finalizate
        var zileCuStudiu = sesiuni
            .Where(s => s.DataSfarsitului.HasValue)
            .Select(s => DateOnly.FromDateTime(
                s.DataSesiunii.Kind == DateTimeKind.Utc
                    ? s.DataSesiunii.ToLocalTime()
                    : s.DataSesiunii))
            .Distinct()
            .ToList();

        var zilePentruCalendar = zileCuStudiu;
        var istoric = zilePentruCalendar
            .Select(z => new StatisticiZilniceDto(z, 0, 0, 0))
            .OrderBy(s => s.Data).ToList();

        // Numara sesiunile finalizate azi
        var aziLocal = DateOnly.FromDateTime(DateTime.Now);
        int sesiuniAzi = sesiuni
            .Count(s => s.DataSfarsitului.HasValue &&
                DateOnly.FromDateTime(
                    s.DataSesiunii.Kind == DateTimeKind.Utc
                        ? s.DataSesiunii.ToLocalTime()
                        : s.DataSesiunii) == aziLocal);

        sw.Stop();
        System.Diagnostics.Debug.WriteLine($"[BENCHMARK]   TOTAL GetStatistici: {sw.ElapsedMilliseconds} ms");
        System.Diagnostics.Debug.WriteLine("───────────────────────────────────────");

        return new StatisticiDto(
            total, invatate, consolidate,
            deRevizuit, cuvinteNoi,
            Math.Round(rata, 1), streak, sesiuniAzi, istoric);
    }

    private static int CalculeazaStreak(List<DateOnly> zile)
    {
        if (!zile.Any()) return 0;

        // zile vine deja OrderByDescending
        var azi = DateOnly.FromDateTime(DateTime.Now);
        var ziMaxima = zile.First(); // cea mai recenta

        // Daca ultima activitate e mai veche de ieri, streak = 0
        if (ziMaxima < azi.AddDays(-1)) return 0;

        int streak = 0;
        var asteptat = ziMaxima; // incepem de la cea mai recenta

        foreach (var z in zile)
        {
            if (z == asteptat)
            {
                streak++;
                asteptat = asteptat.AddDays(-1);
            }
            else if (z < asteptat)
            {
                break; // gap in zile, streak s-a terminat
            }
            // z > asteptat inseamna duplicat, ignoram
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
}