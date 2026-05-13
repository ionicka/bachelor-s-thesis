using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LexaCard.Core.Enums;

namespace LexaCard.Core.Entities;

[Table("cuvinte")]
public class Cuvant
{
    [Key]
    public int Id { get; set; }

    [Required][MaxLength(200)]
    public string Termen { get; set; } = string.Empty;

    [Required]
    public string Definitie { get; set; } = string.Empty;

    [Required]
    public string ExempluPropozitie { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CaleImagine { get; set; }

    [MaxLength(200)]
    public string? Pronuntie { get; set; }

    [MaxLength(50)]
    public string Limba { get; set; } = "engleza";

    public NivelCuvant Nivel { get; set; } = NivelCuvant.Incepator;

    [MaxLength(500)]
    public string? Etichete { get; set; }

    public DateTime DataAdaugarii { get; set; } = DateTime.UtcNow;

    public ICollection<ProgresCuvant> Progrese { get; set; } = new List<ProgresCuvant>();
}
