using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LexaCard.Core.Enums;

namespace LexaCard.Core.Entities;

[Table("cuvinte")]
public class Cuvant
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Termen { get; set; } = string.Empty;

    // Definitia in engleza
    [Required]
    public string Definitie { get; set; } = string.Empty;

    // Traducerea definitiei in romana
    public string? DefinitieRo { get; set; }

    // 3 propozitii exemple separate prin "|"
    // Format: "She decided to [TERMEN] a career.|He wants to [TERMEN] his dreams.|They [TERMEN] the path ahead."
    [Required]
    public string ExemplePropozitii { get; set; } = string.Empty;

    // 2 cai imagini separate prin "|"
    // Format: "images/pursue_1.jpg|images/pursue_2.jpg"
    public string? CaleImagini { get; set; }

    [MaxLength(200)]
    public string? Pronuntie { get; set; }

    [MaxLength(50)]
    public string Limba { get; set; } = "engleza";

    public NivelCuvant Nivel { get; set; } = NivelCuvant.Incepator;

    [MaxLength(500)]
    public string? Etichete { get; set; }

    public DateTime DataAdaugarii { get; set; } = DateTime.UtcNow;

    public ICollection<ProgresCuvant> Progrese { get; set; } = new List<ProgresCuvant>();

    // Helper: lista de exemple
    [NotMapped]
    public List<string> ListaExemple =>
        ExemplePropozitii.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

    // Helper: lista de imagini
    [NotMapped]
    public List<string> ListaImagini =>
        CaleImagini?.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList()
        ?? new List<string>();
}