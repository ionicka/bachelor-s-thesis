using Dapper;
using LexaCard.Core.DTOs;
using LexaCard.Core.Entities;
using LexaCard.Core.Enums;
using LexaCard.Core.Interfaces;
using LexaCard.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LexaCard.Data.Repositories;

// ═══════════════════════════════════════════════════════
// CardRepository — DAPPER pentru citiri performante
// ═══════════════════════════════════════════════════════

public class CardRepository : ICardRepository
{
    private readonly string _connStr;

    public CardRepository(string connectionString) => _connStr = connectionString;

    public async Task<List<CardDto>> GetCarduriPentruAziAsync(
        int utilizatorId, int max = 20)
    {
        await using var conn = new NpgsqlConnection(_connStr);

        // PostgreSQL suporta ORDER BY inainte de LIMIT in subquery
        string sql = @"
            SELECT * FROM (
                SELECT
                    c.""Id""               AS CuvantId,
                    c.""Termen"",
                    c.""Definitie"",
                    c.""ExempluPropozitie"",
                    c.""CaleImagine"",
                    c.""Pronuntie"",
                    c.""Nivel"",
                    p.""NivelCunoastere"",
                    p.""NrRaspunsuriCorecte"",
                    p.""NrRaspunsuriGresite"",
                    1 AS EsteDeRevizuit,
                    0 AS EsteNou
                FROM progres_cuvinte p
                INNER JOIN cuvinte c ON c.""Id"" = p.""CuvantId""
                WHERE p.""UtilizatorId"" = @UId
                  AND p.""DataUrmatoareiRevizuiri"" <= @Azi::date
                  AND p.""NivelCunoastere"" < 7
                ORDER BY p.""DataUrmatoareiRevizuiri"" ASC
                LIMIT @MaxR
            ) revizuire

            UNION ALL

            SELECT * FROM (
                SELECT
                    c.""Id""               AS CuvantId,
                    c.""Termen"",
                    c.""Definitie"",
                    c.""ExempluPropozitie"",
                    c.""CaleImagine"",
                    c.""Pronuntie"",
                    c.""Nivel"",
                    0  AS NivelCunoastere,
                    0  AS NrRaspunsuriCorecte,
                    0  AS NrRaspunsuriGresite,
                    0  AS EsteDeRevizuit,
                    1  AS EsteNou
                FROM cuvinte c
                WHERE c.""Id"" NOT IN (
                    SELECT ""CuvantId""
                    FROM progres_cuvinte
                    WHERE ""UtilizatorId"" = @UId)
                ORDER BY RANDOM()
                LIMIT @MaxN
            ) noi";

        int maxR = (int)(max * 0.75);
        int maxN = max - maxR;

        // Dapper nu suporta DateOnly direct — convertim la string
        string aziStr = DateOnly.FromDateTime(DateTime.UtcNow)
            .ToString("yyyy-MM-dd");

        var rows = await conn.QueryAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            Azi = aziStr,
            MaxR = maxR,
            MaxN = maxN
        });

        return rows.Select(MapToDto).ToList();
    }

    public async Task<CardDto?> GetCardAsync(int utilizatorId, int cuvantId)
    {
        await using var conn = new NpgsqlConnection(_connStr);

        string sql = @"
            SELECT
                c.""Id""               AS CuvantId,
                c.""Termen"",
                c.""Definitie"",
                c.""ExempluPropozitie"",
                c.""CaleImagine"",
                c.""Pronuntie"",
                c.""Nivel"",
                COALESCE(p.""NivelCunoastere"", 0)     AS NivelCunoastere,
                COALESCE(p.""NrRaspunsuriCorecte"", 0) AS NrRaspunsuriCorecte,
                COALESCE(p.""NrRaspunsuriGresite"", 0) AS NrRaspunsuriGresite,
                CASE WHEN p.""Id"" IS NULL THEN 1 ELSE 0 END AS EsteNou,
                CASE WHEN p.""DataUrmatoareiRevizuiri"" <= @Azi::date THEN 1 ELSE 0 END AS EsteDeRevizuit
            FROM cuvinte c
            LEFT JOIN progres_cuvinte p
                ON p.""CuvantId"" = c.""Id"" AND p.""UtilizatorId"" = @UId
            WHERE c.""Id"" = @CuvantId";

        var row = await conn.QueryFirstOrDefaultAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            CuvantId = cuvantId,
            Azi = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd")
        });

        return row == null ? null : MapToDto(row);
    }

    private static CardDto MapToDto(CardRaw r)
    {
        int total = r.NrRaspunsuriCorecte + r.NrRaspunsuriGresite;
        double rata = total == 0 ? 0
            : Math.Round((double)r.NrRaspunsuriCorecte / total * 100, 1);

        string blur = r.ExempluPropozitie
            .Replace("[TERMEN]", new string('_', r.Termen.Length));
        string rev = r.ExempluPropozitie.Replace("[TERMEN]", r.Termen);

        var tip = r.NrRaspunsuriCorecte < 3
            ? TipRaspuns.Recunoastere
            : TipRaspuns.RemintireActiva;

        return new CardDto(
            r.CuvantId, r.Termen, r.Definitie,
            blur, rev, r.CaleImagine, r.Pronuntie,
            (NivelCuvant)r.Nivel, (byte)r.NivelCunoastere,
            rata, r.EsteNou == 1, r.EsteDeRevizuit == 1, tip);
    }

    private class CardRaw
    {
        public int CuvantId { get; set; }
        public string Termen { get; set; } = "";
        public string Definitie { get; set; } = "";
        public string ExempluPropozitie { get; set; } = "";
        public string? CaleImagine { get; set; }
        public string? Pronuntie { get; set; }
        public int Nivel { get; set; }
        public int NivelCunoastere { get; set; }
        public int NrRaspunsuriCorecte { get; set; }
        public int NrRaspunsuriGresite { get; set; }
        public int EsteDeRevizuit { get; set; }
        public int EsteNou { get; set; }
    }
}

// ═══════════════════════════════════════════════════════
// ProgresRepository — EF CORE
// ═══════════════════════════════════════════════════════

public class ProgresRepository : IProgresRepository
{
    private readonly LexaDbContext _ctx;
    public ProgresRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<ProgresCuvant?> GetAsync(int utilizatorId, int cuvantId) =>
        await _ctx.ProgresCuvinte
            .FirstOrDefaultAsync(p =>
                p.UtilizatorId == utilizatorId &&
                p.CuvantId == cuvantId);

    public async Task<ProgresCuvant> GetSauCreeazaAsync(int utilizatorId, int cuvantId)
    {
        var p = await GetAsync(utilizatorId, cuvantId);
        if (p != null) return p;
        p = new ProgresCuvant { UtilizatorId = utilizatorId, CuvantId = cuvantId };
        _ctx.ProgresCuvinte.Add(p);
        await _ctx.SaveChangesAsync();
        return p;
    }

    public async Task SalveazaAsync(ProgresCuvant progres)
    {
        if (progres.Id == 0) _ctx.ProgresCuvinte.Add(progres);
        else _ctx.ProgresCuvinte.Update(progres);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<ProgresCuvant>> GetToateProgreselAsync(int utilizatorId) =>
        await _ctx.ProgresCuvinte
            .AsNoTracking()
            .Where(p => p.UtilizatorId == utilizatorId)
            .Include(p => p.Cuvant)
            .ToListAsync();
}

// ═══════════════════════════════════════════════════════
// UtilizatorRepository — EF CORE
// ═══════════════════════════════════════════════════════

public class UtilizatorRepository : IUtilizatorRepository
{
    private readonly LexaDbContext _ctx;
    public UtilizatorRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<Utilizator?> GetByEmailAsync(string email) =>
        await _ctx.Utilizatori
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task<Utilizator?> GetByIdAsync(int id) =>
        await _ctx.Utilizatori.FindAsync(id);

    public async Task<bool> ExistaEmailAsync(string email) =>
        await _ctx.Utilizatori.AnyAsync(u => u.Email == email.ToLower());

    public async Task<bool> ExistaNumeAsync(string nume) =>
        await _ctx.Utilizatori.AnyAsync(u => u.NumeUtilizator == nume);

    public async Task<Utilizator> CreeazaAsync(Utilizator u)
    {
        _ctx.Utilizatori.Add(u);
        await _ctx.SaveChangesAsync();
        return u;
    }

    public async Task ActualizeazaAsync(Utilizator u)
    {
        _ctx.Utilizatori.Update(u);
        await _ctx.SaveChangesAsync();
    }

    public async Task ActualizeazaUltimaAutentificareAsync(int id)
    {
        await _ctx.Utilizatori
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.UltimaAutentificare, DateTime.UtcNow));
    }
}

// ═══════════════════════════════════════════════════════
// SesiuneRepository — EF CORE
// ═══════════════════════════════════════════════════════

public class SesiuneRepository : ISesiuneRepository
{
    private readonly LexaDbContext _ctx;
    public SesiuneRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<SesiuneStudiu> IncepeAsync(int utilizatorId)
    {
        var s = new SesiuneStudiu
        {
            UtilizatorId = utilizatorId,
            DataSesiunii = DateTime.UtcNow
        };
        _ctx.SesiuniStudiu.Add(s);
        await _ctx.SaveChangesAsync();
        return s;
    }

    public async Task<SesiuneStudiu?> GetAsync(int id) =>
        await _ctx.SesiuniStudiu.FindAsync(id);

    public async Task ActualizeazaAsync(SesiuneStudiu s)
    {
        _ctx.SesiuniStudiu.Update(s);
        await _ctx.SaveChangesAsync();
    }

    public async Task InchideAsync(int id)
    {
        var s = await GetAsync(id);
        if (s == null) return;
        s.DataSfarsitului = DateTime.UtcNow;
        s.DurataSec = (int)(DateTime.UtcNow - s.DataSesiunii).TotalSeconds;
        await ActualizeazaAsync(s);
    }

    public async Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId, int zile = 7)
    {
        var de = DateTime.UtcNow.AddDays(-zile);
        return await _ctx.SesiuniStudiu
            .AsNoTracking()
            .Where(s => s.UtilizatorId == utilizatorId && s.DataSesiunii >= de)
            .OrderByDescending(s => s.DataSesiunii)
            .ToListAsync();
    }
}

// ═══════════════════════════════════════════════════════
// RaspunsRepository — EF CORE
// ═══════════════════════════════════════════════════════

public class RaspunsRepository : IRaspunsRepository
{
    private readonly LexaDbContext _ctx;
    public RaspunsRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task SalveazaAsync(RaspunsDetaliat r)
    {
        _ctx.RaspunsuriDetaliate.Add(r);
        await _ctx.SaveChangesAsync();
    }
}