using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlashCards.Core.Enums;

namespace FlashCards.Core.Entities;

[Table("sesiuni_studiu")]
public class SesiuneStudiu
{
    [Key]
    public int Id { get; set; }

    public int UtilizatorId { get; set; }
    [ForeignKey(nameof(UtilizatorId))]
    public Utilizator? Utilizator { get; set; }

    public DateTime DataSesiunii { get; set; } = DateTime.UtcNow;
    public DateTime? DataSfarsitului { get; set; }

    public int NrCarduriVazute { get; set; } = 0;
    public int NrCorect { get; set; } = 0;
    public int NrGresit { get; set; } = 0;
    public int DurataSec { get; set; } = 0;

    public ICollection<RaspunsDetaliat> Raspunsuri { get; set; } =
        new List<RaspunsDetaliat>();
}

[Table("raspunsuri_detaliate")]
public class RaspunsDetaliat
{
    [Key]
    public int Id { get; set; }

    public int ProgresCuvantId { get; set; }
    [ForeignKey(nameof(ProgresCuvantId))]
    public ProgresCuvant? ProgresCuvant { get; set; }

    public int? SesiuneId { get; set; }
    [ForeignKey(nameof(SesiuneId))]
    public SesiuneStudiu? Sesiune { get; set; }

    public TipRaspuns TipRaspuns { get; set; } = TipRaspuns.Recunoastere;
    public CalitatRaspuns Calitate { get; set; }
    public bool EsteCorect { get; set; }
    public int TimpRaspunsSec { get; set; }

    [MaxLength(200)]
    public string? TextTastat { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
