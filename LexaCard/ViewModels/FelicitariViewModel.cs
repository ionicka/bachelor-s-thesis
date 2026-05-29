namespace LexaCard.ViewModels;

public class FelicitariViewModel
{
    public int Streak { get; }
    public int NrCorect { get; }
    public int NrGresit { get; }
    public string TitluSesiune { get; }
    public string EmojiSesiune { get; }
    public bool AratStreak { get; }
    public string TitluStreak { get; }
    public string MesajStreak { get; }
    public string EmojiStreak { get; }
    public double RataSucces { get; }
    public bool PrimaZi { get; }

    public FelicitariViewModel(int streak, int nrCorect, int nrGresit,
                                bool primaSessioneAZilei)
    {
        Streak = streak;
        NrCorect = nrCorect;
        NrGresit = nrGresit;
        PrimaZi = primaSessioneAZilei;

        int total = nrCorect + nrGresit;
        RataSucces = total == 0 ? 100
            : Math.Round((double)nrCorect / total * 100);

        // Mesaj sesiune — mereu
        (EmojiSesiune, TitluSesiune) = RataSucces switch
        {
            100 => ("🏆", "Perfect!"),
            >= 80 => ("🌟", "Excelent!"),
            >= 60 => ("👍", "Bine facut!"),
            >= 40 => ("📚", "Continua!"),
            _ => ("💪", "Nu te opri!")
        };

        // Mesaj streak — doar la prima sesiune a zilei
        AratStreak = primaSessioneAZilei && streak > 0;
        (EmojiStreak, TitluStreak, MesajStreak) = streak switch
        {
            >= 30 => ("🏆", $"{streak} zile la rand!", "Esti o legenda! Continua!"),
            >= 14 => ("🌟", $"{streak} zile la rand!", "Doua saptamani consecutive!"),
            >= 7 => ("🔥", $"{streak} zile la rand!", "O saptamana intreaga! Bravo!"),
            >= 3 => ("⚡", $"{streak} zile la rand!", "Mergi excelent! Continua!"),
            2 => ("✨", "2 zile la rand!", "Ai inceput un streak! Revino maine!"),
            1 => ("🎉", "Prima zi!", "Bun inceput! Revino maine!"),
            _ => ("🎉", "", "")
        };
    }
}