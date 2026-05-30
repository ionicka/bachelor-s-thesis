namespace FlashCards.Core.Enums;

public enum CalitatRaspuns
{
    Nu_Stiu = 0,
    Stiu_Ezitare = 1,
    Stiu_Sigur = 2,
    Tastat_Corect = 3
}

public enum TipRaspuns
{
    Recunoastere = 0,
    RemintireActiva = 1
}

public enum NivelCuvant
{
    Incepator = 1,
    ElementarInferior = 2,
    ElementarSuperior = 3,
    Intermediar = 4,
    Avansat = 5
}

public enum RolUtilizator
{
    Utilizator = 0,
    Admin = 1
}
public enum TipCuvant
{
    Substantiv = 1,
    Verb = 2,
    Adjectiv = 3,
    VerbFrazal = 4,
    Expresie = 5
}

public enum DomeniuCuvant
{
    General = 0,
    Business = 1,
    Tehnologie = 2,
    Sanatate = 3,
    Educatie = 4,
    Cultura = 5,
    Sport = 6,
    Politica = 7,
    Calatorii = 8,
    Emotii = 9
}

public enum ModInvatare
{
    Toate = 0,        // Mix automat — recunoaștere + tastare (default)
    DoarFlashcards = 1,  // Doar recunoaștere (vizual, no typing)
    DoarTastare = 2   // Doar tastare (active recall pur)
}