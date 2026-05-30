using FlashCards.Core.Enums;

namespace FlashCards.Core.DTOs;

// ═════════════════════════════════════════════════════════════
// Pentru lista din AdminPanel (subțire — doar ce afișezi în listă)
// ═════════════════════════════════════════════════════════════
public record CuvantListaDto(
    int Id,
    string Termen,
    string Definitie,
    TipCuvant Tip,
    DomeniuCuvant Domeniu,
    NivelCuvant Nivel,
    int NrExemple,
    int NrImagini,
    DateTime DataAdaugarii
);

// ═════════════════════════════════════════════════════════════
// Pentru formularul Edit (pre-populat cu valorile existente)
// ═════════════════════════════════════════════════════════════
public class CuvantEditDto
{
    public int? Id { get; set; }  // null = mod adăugare, valoare = mod editare
    public string Termen { get; set; } = string.Empty;
    public string Definitie { get; set; } = string.Empty;
    public string? DefinitieRo { get; set; }
    public string? Pronuntie { get; set; }
    public TipCuvant Tip { get; set; } = TipCuvant.Substantiv;
    public DomeniuCuvant Domeniu { get; set; } = DomeniuCuvant.General;
    public NivelCuvant Nivel { get; set; } = NivelCuvant.Elementar;
    public string Limba { get; set; } = "engleza";
    public string? Etichete { get; set; }

    // Exemple — listă mutabilă pentru UI (admin poate adăuga/scoate)
    public List<string> Exemple { get; set; } = new() { "", "" };

    // Imagini — căi salvate local după FilePicker
    public List<string> Imagini { get; set; } = new();
}

// ═════════════════════════════════════════════════════════════
// Rezultatul operațiilor (pentru feedback la UI)
// ═════════════════════════════════════════════════════════════
public record RezultatOperatieDto(
    bool Succes,
    string? Mesaj,
    int? IdCuvant  // util la creare — primești înapoi noul ID
);

// ═════════════════════════════════════════════════════════════
// Filtru pentru listă (compus, opțional)
// ═════════════════════════════════════════════════════════════
public class FiltruCuvinteDto
{
    public string? Cautare { get; set; }  // caută în Termen sau Definitie
    public TipCuvant? Tip { get; set; }
    public DomeniuCuvant? Domeniu { get; set; }
    public NivelCuvant? Nivel { get; set; }
}