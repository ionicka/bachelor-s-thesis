using FlashCards.Core.Entities;
using Microsoft.EntityFrameworkCore;

public class LexaDbContext : DbContext
{
    static LexaDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public LexaDbContext(DbContextOptions<LexaDbContext> options) : base(options) { }

    public DbSet<Utilizator> Utilizatori { get; set; }
    public DbSet<Cuvant> Cuvinte { get; set; }
    public DbSet<ProgresCuvant> ProgresCuvinte { get; set; }
    public DbSet<SesiuneStudiu> SesiuniStudiu { get; set; }
    public DbSet<RaspunsDetaliat> RaspunsuriDetaliate { get; set; }

    protected override void OnModelCreating(ModelBuilder mb) {

        mb.Entity<Cuvant>(e =>
        {
            e.HasIndex(c => c.Domeniu).HasDatabaseName("ix_cuvinte_domeniu");
            e.HasIndex(c => c.Tip).HasDatabaseName("ix_cuvinte_tip");
        });
    }
}