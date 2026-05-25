using FlashCards.Core.Enums;

namespace FlashCards.Core.Helpers;

public static class EnumLabels
{
    public static string Label(TipCuvant tip) => tip switch
    {
        TipCuvant.Substantiv => "Substantiv",
        TipCuvant.Verb => "Verb",
        TipCuvant.Adjectiv => "Adjectiv",
        TipCuvant.VerbFrazal => "Verb frazal",
        TipCuvant.Expresie => "Expresie",
        _ => tip.ToString()
    };

    public static string Label(DomeniuCuvant domeniu) => domeniu switch
    {
        DomeniuCuvant.General => "General",
        DomeniuCuvant.Business => "Business",
        DomeniuCuvant.Tehnologie => "Tehnologie",
        DomeniuCuvant.Sanatate => "Sănătate",
        DomeniuCuvant.Educatie => "Educație",
        DomeniuCuvant.Cultura => "Cultură",
        DomeniuCuvant.Sport => "Sport",
        DomeniuCuvant.Politica => "Politică",
        DomeniuCuvant.Calatorii => "Călătorii",
        DomeniuCuvant.Emotii => "Emoții",
        _ => domeniu.ToString()
    };

    public static string Label(NivelCuvant nivel) => nivel switch
    {
        NivelCuvant.Incepator => "Începător",
        NivelCuvant.ElementarInferior => "Elementar inferior",
        NivelCuvant.ElementarSuperior => "Elementar superior",
        NivelCuvant.Intermediar => "Intermediar",
        NivelCuvant.Avansat => "Avansat",
        _ => nivel.ToString()
    };

    // Iconițe emoji pentru fiecare domeniu — folosit ca semn vizual rapid
    public static string Icon(DomeniuCuvant domeniu) => domeniu switch
    {
        DomeniuCuvant.General => "📚",
        DomeniuCuvant.Business => "💼",
        DomeniuCuvant.Tehnologie => "💻",
        DomeniuCuvant.Sanatate => "❤️",
        DomeniuCuvant.Educatie => "🎓",
        DomeniuCuvant.Cultura => "🎭",
        DomeniuCuvant.Sport => "⚽",
        DomeniuCuvant.Politica => "🏛️",
        DomeniuCuvant.Calatorii => "✈️",
        DomeniuCuvant.Emotii => "💭",
        _ => "📖"
    };

    public static string Icon(TipCuvant tip) => tip switch
    {
        TipCuvant.Substantiv => "📦",
        TipCuvant.Verb => "🏃",
        TipCuvant.Adjectiv => "🎨",
        TipCuvant.VerbFrazal => "🔗",
        TipCuvant.Expresie => "💬",
        _ => "•"
    };

    // Liste pentru Picker — folosit în UI ca ItemsSource
    public static readonly List<TipCuvant> ToateTipurile = new()
    {
        TipCuvant.Substantiv,
        TipCuvant.Verb,
        TipCuvant.Adjectiv,
        TipCuvant.VerbFrazal,
        TipCuvant.Expresie
    };

    public static readonly List<DomeniuCuvant> ToateDomenii = new()
    {
        DomeniuCuvant.General,
        DomeniuCuvant.Business,
        DomeniuCuvant.Tehnologie,
        DomeniuCuvant.Sanatate,
        DomeniuCuvant.Educatie,
        DomeniuCuvant.Cultura,
        DomeniuCuvant.Sport,
        DomeniuCuvant.Politica,
        DomeniuCuvant.Calatorii,
        DomeniuCuvant.Emotii
    };

    public static readonly List<NivelCuvant> ToateNivelele = new()
    {
        NivelCuvant.Incepator,
        NivelCuvant.ElementarInferior,
        NivelCuvant.ElementarSuperior,
        NivelCuvant.Intermediar,
        NivelCuvant.Avansat
    };
}