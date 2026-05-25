using FlashCards.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Core.Entities;

[Table("utilizatori")]
public class Utilizator
{
    [Key]
    public int Id { get; set; }

    [Required][MaxLength(50)]
    public string NumeUtilizator { get; set; } = string.Empty;

    [Required][MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required][MaxLength(255)]
    public string ParolaHash { get; set; } = string.Empty;
    public RolUtilizator Rol { get; set; } = RolUtilizator.Utilizator;

    public DateTime DataInregistrarii { get; set; } = DateTime.UtcNow;
    public DateTime? UltimaAutentificare { get; set; }

    public int CarduriNoiPerZi { get; set; } = 10;
    public int MaxCarduriPerSesiune { get; set; } = 20;

    public ICollection<ProgresCuvant> Progrese { get; set; } = new List<ProgresCuvant>();
    public ICollection<SesiuneStudiu> Sesiuni { get; set; } = new List<SesiuneStudiu>();
}
