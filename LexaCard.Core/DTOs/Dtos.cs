using LexaCard.Core.Enums;

namespace LexaCard.Core.DTOs;

public record CardDto(
    int CuvantId,
    string Termen,
    string Definitie,           // engleza
    string? DefinitieRo,        // romana
    string PropozitieBlur,      // primul exemplu cu blank
    string PropozitieRevelata,  // primul exemplu cu termen
    string CasuteLitere,
    List<string> Exemple,       // toate 3 exemplele cu [TERMEN]
    List<string> Imagini,       // caile imaginilor
    string? CaleImagine,        // prima imagine (compatibilitate)
    string? Pronuntie,
    NivelCuvant Nivel,
    byte NivelCunoastere,
    double RataSucces,
    bool EsteNou,
    bool EsteDeRevizuit,
    TipRaspuns TipRaspunsRecomandat
);

public class RaspunsCardDto
{
    public int CuvantId { get; set; }
    public CalitatRaspuns Calitate { get; set; }
    public TipRaspuns TipRaspuns { get; set; } = TipRaspuns.Recunoastere;
    public int TimpRaspunsSec { get; set; }
    public string? TextTastat { get; set; }
    public int? SesiuneId { get; set; }
}

public record RezultatRaspunsDto(
    byte NivelNou,
    int IntervalNouZile,
    DateOnly DataUrmatoareiRevizuiri,
    bool EsteConsolidat,
    string MesajMotivational
);

public record StatisticiDto(
    int TotalCuvinte,
    int CuvinteInvatate,
    int CuvinteConsolidate,
    int CuvinteDeRevizuitAzi,
    int CuvinteNoi,
    double RataSuccesGlobala,
    int ZileCurenteStreak,
    int SesiuniFinalizateAzi,
    List<StatisticiZilniceDto> IstoricSaptamana
);

public record StatisticiZilniceDto(
    DateOnly Data,
    int NrCarduri,
    int NrCorect,
    double RataSucces
);

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Parola { get; set; } = string.Empty;
}

public class InregistrareDto
{
    public string NumeUtilizator { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Parola { get; set; } = string.Empty;
    public string ConfirmaParola { get; set; } = string.Empty;
}

public record UtilizatorDto(
    int Id,
    string NumeUtilizator,
    string Email,
    int CarduriNoiPerZi,
    int MaxCarduriPerSesiune
);

public record RezultatSrs(
    byte NivelNou,
    int IntervalNou,
    DateOnly DataUrmatoareiRevizuiri
);