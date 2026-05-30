using Dapper;
using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using FlashCards.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlashCards.Data.Repositories;

// ===============================================================
// CardRepository — DAPPER pentru citiri performante
// ===============================================================

public class CardRepository : ICardRepository
{
    private readonly string _connStr;
    public CardRepository(string connectionString) => _connStr = connectionString;

    /// <summary>
    /// Revizuiri programate pentru azi - toate, fara limita
    /// </summary>
    public async Task<List<CardDto>> GetRevizuiriAziAsync(int utilizatorId)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        string sql = @"
            SELECT
                c.""Id""               AS CuvantId,
                c.""Termen"",
                c.""Definitie"",
                    c.""DefinitieRo"",
                c.""ExemplePropozitii"",
                c.""CaleImagini"",
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
            ORDER BY p.""NivelCunoastere"" ASC, p.""DataUrmatoareiRevizuiri"" ASC";

        var rows = await conn.QueryAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            Azi = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd")
        });
        return rows.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Cuvinte noi - nevazute niciodata de utilizator, limitate la max
    /// </summary>
    public async Task<List<CardDto>> GetCuvinteNoiAsync(int utilizatorId, int max)
    {
        if (max <= 0) return new List<CardDto>();

        await using var conn = new NpgsqlConnection(_connStr);
        string sql = @"
            SELECT
                c.""Id""               AS CuvantId,
                c.""Termen"",
                c.""Definitie"",
                    c.""DefinitieRo"",
                c.""ExemplePropozitii"",
                c.""CaleImagini"",
                c.""Pronuntie"",
                c.""Nivel"",
                0 AS NivelCunoastere,
                0 AS NrRaspunsuriCorecte,
                0 AS NrRaspunsuriGresite,
                0 AS EsteDeRevizuit,
                1 AS EsteNou
            FROM cuvinte c
            WHERE c.""Id"" NOT IN (
                SELECT ""CuvantId"" FROM progres_cuvinte
                WHERE ""UtilizatorId"" = @UId)
            ORDER BY c.""Nivel"" ASC, RANDOM()
            LIMIT @Max";

        var rows = await conn.QueryAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            Max = max
        });
        return rows.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Toate cuvintele - pentru practica libera
    /// </summary>
    public async Task<List<CardDto>> GetToateCuvinteleAsync(int utilizatorId)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        string sql = @"
            SELECT
                c.""Id""               AS CuvantId,
                c.""Termen"",
                c.""Definitie"",
                    c.""DefinitieRo"",
                c.""ExemplePropozitii"",
                c.""CaleImagini"",
                c.""Pronuntie"",
                c.""Nivel"",
                COALESCE(p.""NivelCunoastere"", 0)     AS NivelCunoastere,
                COALESCE(p.""NrRaspunsuriCorecte"", 0) AS NrRaspunsuriCorecte,
                COALESCE(p.""NrRaspunsuriGresite"", 0) AS NrRaspunsuriGresite,
                0 AS EsteDeRevizuit,
                CASE WHEN p.""Id"" IS NULL THEN 1 ELSE 0 END AS EsteNou
            FROM cuvinte c
            LEFT JOIN progres_cuvinte p
                ON p.""CuvantId"" = c.""Id"" AND p.""UtilizatorId"" = @UId
            ORDER BY RANDOM()";

        var rows = await conn.QueryAsync<CardRaw>(sql, new { UId = utilizatorId });
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
                    c.""DefinitieRo"",
                c.""ExemplePropozitii"",
                c.""CaleImagini"",
                c.""Pronuntie"",
                c.""Nivel"",
                COALESCE(p.""NivelCunoastere"", 0)     AS NivelCunoastere,
                COALESCE(p.""NrRaspunsuriCorecte"", 0) AS NrRaspunsuriCorecte,
                COALESCE(p.""NrRaspunsuriGresite"", 0) AS NrRaspunsuriGresite,
                CASE WHEN p.""Id"" IS NULL THEN 1 ELSE 0 END AS EsteNou,
                CASE WHEN p.""DataUrmatoareiRevizuiri"" <= @Azi::date
                     THEN 1 ELSE 0 END AS EsteDeRevizuit
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

        // Exemple — split dupa |
        var exemple = r.ExemplePropozitii
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        // Primul exemplu pentru blur/revelat
        string primulExemplu = exemple.FirstOrDefault() ?? r.Termen;
        string blur = primulExemplu.Replace("[TERMEN]", new string('_', r.Termen.Length));
        string rev = primulExemplu.Replace("[TERMEN]", r.Termen);
        string casute = string.Concat(r.Termen.Select(c => c == ' ' ? "  " : "□"));

        // Imagini — split dupa |
        var imagini = (r.CaleImagini ?? "")
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        string? primaImagine = imagini.FirstOrDefault();

        var tip = r.NivelCunoastere >= 3
            ? TipRaspuns.RemintireActiva
            : TipRaspuns.Recunoastere;

        return new CardDto(
            r.CuvantId, r.Termen,
            r.Definitie, r.DefinitieRo,
            blur, rev, casute,
            exemple, imagini, primaImagine, r.Pronuntie,
            (NivelCuvant)r.Nivel, (byte)r.NivelCunoastere,
            rata, r.EsteNou == 1, r.EsteDeRevizuit == 1, tip);
    }

    private class CardRaw
    {
        public int CuvantId { get; set; }
        public string Termen { get; set; } = "";
        public string Definitie { get; set; } = "";
        public string? DefinitieRo { get; set; }
        public string ExemplePropozitii { get; set; } = "";
        public string? CaleImagini { get; set; }
        public string? Pronuntie { get; set; }
        public int Nivel { get; set; }
        public int NivelCunoastere { get; set; }
        public int NrRaspunsuriCorecte { get; set; }
        public int NrRaspunsuriGresite { get; set; }
        public int EsteDeRevizuit { get; set; }
        public int EsteNou { get; set; }
    }
    public async Task<List<CardDto>> GetRevizuiriAziFiltrateAsync(
    int utilizatorId,
    List<NivelCuvant> niveluri,
    List<DomeniuCuvant> domenii)
    {
        await using var conn = new NpgsqlConnection(_connStr);

        // Construim WHERE dinamic pe baza filtrelor
        var clauze = new List<string>
    {
        "p.\"UtilizatorId\" = @UId",
        "p.\"DataUrmatoareiRevizuiri\" <= @Azi::date",
        "p.\"NivelCunoastere\" < 7"
    };

        if (niveluri != null && niveluri.Count > 0)
            clauze.Add("c.\"Nivel\" = ANY(@Niveluri)");
        if (domenii != null && domenii.Count > 0)
            clauze.Add("c.\"Domeniu\" = ANY(@Domenii)");

        string sql = $@"
        SELECT
            c.""Id""               AS CuvantId,
            c.""Termen"",
            c.""Definitie"",
            c.""DefinitieRo"",
            c.""ExemplePropozitii"",
            c.""CaleImagini"",
            c.""Pronuntie"",
            c.""Nivel"",
            p.""NivelCunoastere"",
            p.""NrRaspunsuriCorecte"",
            p.""NrRaspunsuriGresite"",
            1 AS EsteDeRevizuit,
            0 AS EsteNou
        FROM progres_cuvinte p
        INNER JOIN cuvinte c ON c.""Id"" = p.""CuvantId""
        WHERE {string.Join(" AND ", clauze)}
        ORDER BY p.""NivelCunoastere"" ASC, p.""DataUrmatoareiRevizuiri"" ASC";

        var rows = await conn.QueryAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            Azi = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
            Niveluri = niveluri?.Select(n => (int)n).ToArray() ?? Array.Empty<int>(),
            Domenii = domenii?.Select(d => (int)d).ToArray() ?? Array.Empty<int>()
        });
        return rows.Select(MapToDto).ToList();
    }

    public async Task<List<CardDto>> GetCuvinteNoiFiltrateAsync(
        int utilizatorId,
        int max,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii)
    {
        if (max <= 0) return new List<CardDto>();

        await using var conn = new NpgsqlConnection(_connStr);

        var clauze = new List<string>
    {
        @"c.""Id"" NOT IN (SELECT ""CuvantId"" FROM progres_cuvinte WHERE ""UtilizatorId"" = @UId)"
    };

        if (niveluri != null && niveluri.Count > 0)
            clauze.Add("c.\"Nivel\" = ANY(@Niveluri)");
        if (domenii != null && domenii.Count > 0)
            clauze.Add("c.\"Domeniu\" = ANY(@Domenii)");

        string sql = $@"
        SELECT
            c.""Id""               AS CuvantId,
            c.""Termen"",
            c.""Definitie"",
            c.""DefinitieRo"",
            c.""ExemplePropozitii"",
            c.""CaleImagini"",
            c.""Pronuntie"",
            c.""Nivel"",
            0 AS NivelCunoastere,
            0 AS NrRaspunsuriCorecte,
            0 AS NrRaspunsuriGresite,
            0 AS EsteDeRevizuit,
            1 AS EsteNou
        FROM cuvinte c
        WHERE {string.Join(" AND ", clauze)}
        ORDER BY c.""Nivel"" ASC, RANDOM()
        LIMIT @Max";

        var rows = await conn.QueryAsync<CardRaw>(sql, new
        {
            UId = utilizatorId,
            Max = max,
            Niveluri = niveluri?.Select(n => (int)n).ToArray() ?? Array.Empty<int>(),
            Domenii = domenii?.Select(d => (int)d).ToArray() ?? Array.Empty<int>()
        });
        return rows.Select(MapToDto).ToList();
    }

    public async Task<DisponibilitateSesiuneDto> GetDisponibilitateAsync(
        int utilizatorId,
        List<NivelCuvant> niveluri,
        List<DomeniuCuvant> domenii)
    {
        await using var conn = new NpgsqlConnection(_connStr);

        // Pregătim parametrii o singură dată
        var paramsObj = new
        {
            UId = utilizatorId,
            Azi = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
            Niveluri = niveluri?.Select(n => (int)n).ToArray() ?? Array.Empty<int>(),
            Domenii = domenii?.Select(d => (int)d).ToArray() ?? Array.Empty<int>()
        };

        // Clauze pentru filtrare nivel/domeniu (folosite în ambele query-uri)
        var clauzeFiltru = new List<string>();
        if (niveluri != null && niveluri.Count > 0)
            clauzeFiltru.Add("c.\"Nivel\" = ANY(@Niveluri)");
        if (domenii != null && domenii.Count > 0)
            clauzeFiltru.Add("c.\"Domeniu\" = ANY(@Domenii)");

        string filterAnd = clauzeFiltru.Count > 0
            ? " AND " + string.Join(" AND ", clauzeFiltru)
            : "";

        // Revizuiri disponibile
        string sqlRev = $@"
        SELECT COUNT(*)
        FROM progres_cuvinte p
        INNER JOIN cuvinte c ON c.""Id"" = p.""CuvantId""
        WHERE p.""UtilizatorId"" = @UId
          AND p.""DataUrmatoareiRevizuiri"" <= @Azi::date
          AND p.""NivelCunoastere"" < 7
          {filterAnd}";

        int nrRev = await conn.ExecuteScalarAsync<int>(sqlRev, paramsObj);

        // Cuvinte noi disponibile
        string sqlNoi = $@"
        SELECT COUNT(*)
        FROM cuvinte c
        WHERE c.""Id"" NOT IN (
            SELECT ""CuvantId"" FROM progres_cuvinte
            WHERE ""UtilizatorId"" = @UId)
          {filterAnd}";

        int nrNoi = await conn.ExecuteScalarAsync<int>(sqlNoi, paramsObj);

        return new DisponibilitateSesiuneDto(nrRev, nrNoi, nrRev + nrNoi);
    }
}

// ===============================================================
// ProgresRepository — EF CORE
// ===============================================================

public class ProgresRepository : IProgresRepository
{
    private readonly LexaDbContext _ctx;
    public ProgresRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<ProgresCuvant?> GetAsync(int utilizatorId, int cuvantId) =>
        await _ctx.ProgresCuvinte
            .FirstOrDefaultAsync(p =>
                p.UtilizatorId == utilizatorId && p.CuvantId == cuvantId);

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

    public async Task<int> GetNrRevizuiriAziAsync(int utilizatorId)
    {
        var azi = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _ctx.ProgresCuvinte
            .CountAsync(p =>
                p.UtilizatorId == utilizatorId &&
                p.DataUrmatoareiRevizuiri <= azi &&
                p.NivelCunoastere < 7);
    }

    public async Task<int> GetNrCuvinteNoiAsync(int utilizatorId)
    {
        return await _ctx.Cuvinte
            .Where(c => !_ctx.ProgresCuvinte
                .Any(p => p.UtilizatorId == utilizatorId && p.CuvantId == c.Id))
            .CountAsync();
    }
}

// ===============================================================
// UtilizatorRepository — EF CORE
// ===============================================================

public class UtilizatorRepository : IUtilizatorRepository
{
    private readonly LexaDbContext _ctx;
    public UtilizatorRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<Utilizator?> GetByEmailAsync(string email) =>
        await _ctx.Utilizatori.FirstOrDefaultAsync(u => u.Email == email.ToLower());

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

// ===============================================================
// SesiuneRepository — EF CORE
// ===============================================================

public class SesiuneRepository : ISesiuneRepository
{
    private readonly LexaDbContext _ctx;
    public SesiuneRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<SesiuneStudiu> IncepeAsync(int utilizatorId)
    {
        var s = new SesiuneStudiu { UtilizatorId = utilizatorId, DataSesiunii = DateTime.UtcNow };
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

// ===============================================================
// RaspunsRepository — EF CORE
// ===============================================================

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


// ===============================================================
// AdminRepository — EF CORE
// CRUD complet pentru gestionarea cuvintelor de către admin
// ===============================================================

public class AdminRepository : IAdminRepository
{
    private readonly LexaDbContext _ctx;
    public AdminRepository(LexaDbContext ctx) => _ctx = ctx;

    public async Task<List<CuvantListaDto>> GetCuvinteAsync(FiltruCuvinteDto? filtru = null)
    {
        var query = _ctx.Cuvinte.AsNoTracking().AsQueryable();

        if (filtru != null)
        {
            if (!string.IsNullOrWhiteSpace(filtru.Cautare))
            {
                var c = filtru.Cautare.Trim().ToLower();
                // ILIKE = case-insensitive în PostgreSQL
                query = query.Where(x =>
                    EF.Functions.ILike(x.Termen, $"%{c}%") ||
                    EF.Functions.ILike(x.Definitie, $"%{c}%"));
            }
            if (filtru.Tip.HasValue)
                query = query.Where(x => x.Tip == filtru.Tip.Value);
            if (filtru.Domeniu.HasValue)
                query = query.Where(x => x.Domeniu == filtru.Domeniu.Value);
            if (filtru.Nivel.HasValue)
                query = query.Where(x => x.Nivel == filtru.Nivel.Value);
        }

        return await query
            .OrderByDescending(c => c.DataAdaugarii)
            .Select(c => new CuvantListaDto(
                c.Id,
                c.Termen,
                c.Definitie,
                c.Tip,
                c.Domeniu,
                c.Nivel,
                c.ExemplePropozitii == "" ? 0 :
                    c.ExemplePropozitii.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Length,
                c.CaleImagini == null || c.CaleImagini == "" ? 0 :
                    c.CaleImagini.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Length,
                c.DataAdaugarii))
            .ToListAsync();
    }

    public async Task<CuvantEditDto?> GetCuvantPentruEditAsync(int id)
    {
        var c = await _ctx.Cuvinte
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return null;

        return new CuvantEditDto
        {
            Id = c.Id,
            Termen = c.Termen,
            Definitie = c.Definitie,
            DefinitieRo = c.DefinitieRo,
            Pronuntie = c.Pronuntie,
            Tip = c.Tip,
            Domeniu = c.Domeniu,
            Nivel = c.Nivel,
            Limba = c.Limba,
            Etichete = c.Etichete,
            Exemple = c.ExemplePropozitii
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .ToList(),
            Imagini = (c.CaleImagini ?? "")
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .ToList()
        };
    }

    public async Task<int> CreeazaCuvantAsync(CuvantEditDto dto)
    {
        var cuvant = new Cuvant
        {
            Termen = dto.Termen.Trim(),
            Definitie = dto.Definitie.Trim(),
            DefinitieRo = string.IsNullOrWhiteSpace(dto.DefinitieRo) ? null : dto.DefinitieRo.Trim(),
            Pronuntie = string.IsNullOrWhiteSpace(dto.Pronuntie) ? null : dto.Pronuntie.Trim(),
            Tip = dto.Tip,
            Domeniu = dto.Domeniu,
            Nivel = dto.Nivel,
            Limba = dto.Limba,
            Etichete = string.IsNullOrWhiteSpace(dto.Etichete) ? null : dto.Etichete.Trim(),
            ExemplePropozitii = string.Join("|",
                dto.Exemple.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim())),
            CaleImagini = dto.Imagini.Count == 0 ? null :
                string.Join("|", dto.Imagini.Where(i => !string.IsNullOrWhiteSpace(i))),
            DataAdaugarii = DateTime.UtcNow
        };

        _ctx.Cuvinte.Add(cuvant);
        await _ctx.SaveChangesAsync();
        return cuvant.Id;
    }

    public async Task ActualizeazaCuvantAsync(CuvantEditDto dto)
    {
        if (!dto.Id.HasValue)
            throw new InvalidOperationException("Id-ul este obligatoriu pentru actualizare.");

        var cuvant = await _ctx.Cuvinte.FirstOrDefaultAsync(c => c.Id == dto.Id.Value);
        if (cuvant == null)
            throw new InvalidOperationException($"Cuvântul cu Id={dto.Id} nu există.");

        cuvant.Termen = dto.Termen.Trim();
        cuvant.Definitie = dto.Definitie.Trim();
        cuvant.DefinitieRo = string.IsNullOrWhiteSpace(dto.DefinitieRo) ? null : dto.DefinitieRo.Trim();
        cuvant.Pronuntie = string.IsNullOrWhiteSpace(dto.Pronuntie) ? null : dto.Pronuntie.Trim();
        cuvant.Tip = dto.Tip;
        cuvant.Domeniu = dto.Domeniu;
        cuvant.Nivel = dto.Nivel;
        cuvant.Limba = dto.Limba;
        cuvant.Etichete = string.IsNullOrWhiteSpace(dto.Etichete) ? null : dto.Etichete.Trim();
        cuvant.ExemplePropozitii = string.Join("|",
            dto.Exemple.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim()));
        cuvant.CaleImagini = dto.Imagini.Count == 0 ? null :
            string.Join("|", dto.Imagini.Where(i => !string.IsNullOrWhiteSpace(i)));

        await _ctx.SaveChangesAsync();
    }

    public async Task StergeCuvantAsync(int id)
    {
        var cuvant = await _ctx.Cuvinte.FirstOrDefaultAsync(c => c.Id == id);
        if (cuvant == null) return;

        // Cascade pe progrese e Restrict (definit în LexaDbContext)
        // → trebuie să ștergem manual progresele dependente întâi
        var progrese = await _ctx.ProgresCuvinte
            .Where(p => p.CuvantId == id)
            .ToListAsync();
        if (progrese.Any())
        {
            // Ștergem și răspunsurile (cascade pe ProgresCuvant)
            _ctx.ProgresCuvinte.RemoveRange(progrese);
        }

        _ctx.Cuvinte.Remove(cuvant);
        await _ctx.SaveChangesAsync();
    }

    public async Task<bool> ExistaTermenAsync(string termen, int? exceptId = null)
    {
        var t = termen.Trim().ToLower();
        var query = _ctx.Cuvinte.Where(c => c.Termen.ToLower() == t);
        if (exceptId.HasValue)
            query = query.Where(c => c.Id != exceptId.Value);
        return await query.AnyAsync();
    }

    public async Task<int> GetNrTotalCuvinteAsync() =>
        await _ctx.Cuvinte.CountAsync();

    public async Task<Dictionary<DomeniuCuvant, int>> GetNrPeDomeniiAsync()
    {
        return await _ctx.Cuvinte
            .GroupBy(c => c.Domeniu)
            .Select(g => new { Domeniu = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Domeniu, x => x.Count);
    }
}