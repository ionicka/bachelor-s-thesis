using LexaCard.Core.Entities;
using LexaCard.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace LexaCard.Data.Context;

public class LexaDbContext : DbContext
{
    static LexaDbContext()
    {
        // Rezolva eroarea "timestamp with time zone vs without time zone"
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public LexaDbContext(DbContextOptions<LexaDbContext> options)
        : base(options) { }

    public DbSet<Utilizator> Utilizatori { get; set; }
    public DbSet<Cuvant> Cuvinte { get; set; }
    public DbSet<ProgresCuvant> ProgresCuvinte { get; set; }
    public DbSet<SesiuneStudiu> SesiuniStudiu { get; set; }
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

        // Seed data se insereaza manual prin script SQL
    }
}