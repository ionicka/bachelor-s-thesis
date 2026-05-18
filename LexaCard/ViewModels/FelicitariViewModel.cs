namespace LexaCard.ViewModels;

public class FelicitariViewModel
{
    public int    Streak   { get; }
    public int    NrCorect { get; }
    public int    NrGresit { get; }
    public string Titlu    { get; }
    public string Mesaj    { get; }
    public string Emoji    { get; }
    public double RataSucces { get; }

    public FelicitariViewModel(int streak, int nrCorect, int nrGresit)
    {
        Streak   = streak;
        NrCorect = nrCorect;
        NrGresit = nrGresit;

        int total = nrCorect + nrGresit;
        RataSucces = total == 0 ? 100 : Math.Round((double)nrCorect / total * 100);

        (Emoji, Titlu, Mesaj) = streak switch
        {
            >= 30 => ("🏆", "Legendar!", "30 de zile consecutive! Esti extraordinar!"),
            >= 14 => ("🌟", "Incredible!", $"{streak} zile la rand! Continuarea e cheia!"),
            >= 7  => ("🔥", "In flacari!", $"{streak} zile consecutive! Mentine ritmul!"),
            >= 3  => ("⚡", "Excelent!", $"{streak} zile la rand! Progresezi rapid!"),
            2     => ("✨", "Bun inceput!", "2 zile consecutive! Continua maine!"),
            1     => ("🎉", "Felicitari!", "Sesiune finalizata! Revino maine!"),
            _     => ("🎉", "Felicitari!", "Sesiune finalizata! Revino maine!")
        };
    }
}
