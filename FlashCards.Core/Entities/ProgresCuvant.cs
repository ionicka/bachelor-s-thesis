using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Core.Entities;

[Table("progres_cuvinte")]
public class ProgresCuvant
{
    [Key]
    public int Id { get; set; }
    public bool EsteIgnorat { get; set; } = false;
    public int UtilizatorId { get; set; }
    [ForeignKey(nameof(UtilizatorId))]
    public Utilizator? Utilizator { get; set; }

    public int CuvantId { get; set; }
    [ForeignKey(nameof(CuvantId))]
    public Cuvant? Cuvant { get; set; }

    public byte NivelCunoastere { get; set; } = 1;

    public DateOnly DataUrmatoareiRevizuiri { get; set; } =
        DateOnly.FromDateTime(DateTime.UtcNow);

    public int IntervalCurentZile { get; set; } = 1;
    public int NrRaspunsuriCorecte { get; set; } = 0;
    public int NrRaspunsuriGresite { get; set; } = 0;

    public DateTime DataPrimeiIntalniri { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimeiRevizuiri { get; set; }

    public ICollection<RaspunsDetaliat> Raspunsuri { get; set; } =
        new List<RaspunsDetaliat>();

    [NotMapped]
    public double RataSucces
    {
        get
        {
            int total = NrRaspunsuriCorecte + NrRaspunsuriGresite;
            return total == 0 ? 0.0
                : Math.Round((double)NrRaspunsuriCorecte / total * 100, 1);
        }
    }

    [NotMapped]
    public bool EsteDeRevizuitAzi =>
        DataUrmatoareiRevizuiri <= DateOnly.FromDateTime(DateTime.UtcNow);

    [NotMapped]
    public bool EsteConsolidat => NivelCunoastere >= 7;
}
