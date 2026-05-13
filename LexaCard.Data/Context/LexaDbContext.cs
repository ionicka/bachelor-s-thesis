using LexaCard.Core.Entities;
using LexaCard.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace LexaCard.Data.Context;

public class LexaDbContext : DbContext
{
    public LexaDbContext(DbContextOptions<LexaDbContext> options)
        : base(options) { }

    public DbSet<Utilizator>      Utilizatori         { get; set; }
    public DbSet<Cuvant>          Cuvinte             { get; set; }
    public DbSet<ProgresCuvant>   ProgresCuvinte      { get; set; }
    public DbSet<SesiuneStudiu>   SesiuniStudiu       { get; set; }
    public DbSet<RaspunsDetaliat> RaspunsuriDetaliate { get; set; }

    // Connection string pentru PostgreSQL
    public const string ConnectionString =
        "Host=localhost;Port=5432;Database=lexacard;Username=postgres;Password=postgres";

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Utilizator>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.NumeUtilizator).IsUnique();
        });

        mb.Entity<ProgresCuvant>(e =>
        {
            e.HasIndex(p => new { p.UtilizatorId, p.CuvantId })
             .IsUnique()
             .HasDatabaseName("uq_progres_utilizator_cuvant");

            e.HasIndex(p => new { p.UtilizatorId, p.DataUrmatoareiRevizuiri })
             .HasDatabaseName("ix_progres_azi");

            e.HasOne(p => p.Utilizator)
             .WithMany(u => u.Progrese)
             .HasForeignKey(p => p.UtilizatorId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Cuvant)
             .WithMany(c => c.Progrese)
             .HasForeignKey(p => p.CuvantId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<SesiuneStudiu>(e =>
        {
            e.HasOne(s => s.Utilizator)
             .WithMany(u => u.Sesiuni)
             .HasForeignKey(s => s.UtilizatorId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<RaspunsDetaliat>(e =>
        {
            e.HasOne(r => r.ProgresCuvant)
             .WithMany(p => p.Raspunsuri)
             .HasForeignKey(r => r.ProgresCuvantId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Sesiune)
             .WithMany(s => s.Raspunsuri)
             .HasForeignKey(r => r.SesiuneId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed: 10 cuvinte engleze cu imagini si propozitii complete
        mb.Entity<Cuvant>().HasData(
            new Cuvant { Id = 1, Termen = "enigma",
                Definitie = "Ceva sau cineva misterios, greu de inteles sau explicat.",
                ExempluPropozitie = "The disappearance of the ship remained an [TERMEN] for decades.",
                CaleImagine = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?w=400",
                Pronuntie = "/ɪˈnɪɡ.mə/", Nivel = NivelCuvant.Intermediar, Etichete = "substantiv" },
            new Cuvant { Id = 2, Termen = "resilient",
                Definitie = "Capabil sa se recupereze rapid din dificultati; tenace.",
                ExempluPropozitie = "She proved to be incredibly [TERMEN] after losing her job.",
                CaleImagine = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400",
                Pronuntie = "/rɪˈzɪl.i.ənt/", Nivel = NivelCuvant.Intermediar, Etichete = "adjectiv" },
            new Cuvant { Id = 3, Termen = "ephemeral",
                Definitie = "Care dureaza doar o perioada scurta; trecator, efemer.",
                ExempluPropozitie = "Social media fame is often [TERMEN], lasting only a few days.",
                CaleImagine = "https://images.unsplash.com/photo-1490750967868-88df5691cc9a?w=400",
                Pronuntie = "/ɪˈfem.ər.əl/", Nivel = NivelCuvant.Avansat, Etichete = "adjectiv" },
            new Cuvant { Id = 4, Termen = "serendipity",
                Definitie = "Descoperiri placute si valoroase facute din intamplare.",
                ExempluPropozitie = "Meeting my best friend on that train was pure [TERMEN].",
                CaleImagine = "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7?w=400",
                Pronuntie = "/ˌser.ənˈdɪp.ɪ.ti/", Nivel = NivelCuvant.Avansat, Etichete = "substantiv" },
            new Cuvant { Id = 5, Termen = "ambiguous",
                Definitie = "Care poate fi inteles in mai mult de un sens; neclar.",
                ExempluPropozitie = "The manager gave an [TERMEN] answer that confused everyone.",
                CaleImagine = "https://images.unsplash.com/photo-1495364141860-b0d03eccd065?w=400",
                Pronuntie = "/æmˈbɪɡ.ju.əs/", Nivel = NivelCuvant.ElementarSuperior, Etichete = "adjectiv" },
            new Cuvant { Id = 6, Termen = "melancholy",
                Definitie = "O stare de tristete profunda si nostalgie fara o cauza clara.",
                ExempluPropozitie = "A deep [TERMEN] came over him as he looked at old photographs.",
                CaleImagine = "https://images.unsplash.com/photo-1474552226712-ac0f0961a954?w=400",
                Pronuntie = "/ˈmel.ən.kɒl.i/", Nivel = NivelCuvant.Intermediar, Etichete = "substantiv" },
            new Cuvant { Id = 7, Termen = "perseverance",
                Definitie = "Continuarea unui efort in ciuda dificultatilor; tenacitate.",
                ExempluPropozitie = "Her [TERMEN] in learning the language finally paid off after two years.",
                CaleImagine = "https://images.unsplash.com/photo-1461897104016-0b3b00cc81ee?w=400",
                Pronuntie = "/ˌpɜː.sɪˈvɪər.əns/", Nivel = NivelCuvant.Intermediar, Etichete = "substantiv" },
            new Cuvant { Id = 8, Termen = "eloquent",
                Definitie = "Capabil sa exprime idei cu claritate si forta; expresiv.",
                ExempluPropozitie = "The president gave an [TERMEN] speech that moved the entire audience.",
                CaleImagine = "https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400",
                Pronuntie = "/ˈel.ə.kwənt/", Nivel = NivelCuvant.Intermediar, Etichete = "adjectiv" },
            new Cuvant { Id = 9, Termen = "nostalgia",
                Definitie = "Dor sau afectiune pentru trecut, pentru momente sau locuri indepartate.",
                ExempluPropozitie = "Smelling fresh bread filled her with [TERMEN] for her grandmother's kitchen.",
                CaleImagine = "https://images.unsplash.com/photo-1476703993599-0035a21b17a9?w=400",
                Pronuntie = "/nɒˈstæl.dʒə/", Nivel = NivelCuvant.ElementarSuperior, Etichete = "substantiv" },
            new Cuvant { Id = 10, Termen = "meticulous",
                Definitie = "Foarte atent la detalii; care face lucrurile cu grija si precizie.",
                ExempluPropozitie = "The surgeon was [TERMEN] in her preparation before every operation.",
                CaleImagine = "https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400",
                Pronuntie = "/məˈtɪk.jʊ.ləs/", Nivel = NivelCuvant.Avansat, Etichete = "adjectiv" }
        );
    }
}
