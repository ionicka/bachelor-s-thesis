using FlashCards.Core.DTOs;

namespace FlashCards.Services;

public interface INavigationService
{
    Task MergeAsync(string ruta);
    Task InapoiAsync();
}

public interface ISessionStateService
{
    bool EsteAutentificat { get; }
    UtilizatorDto? UtilizatorCurent { get; }
    int? SesiuneCurenta { get; }
    int CuvinteNoiSesiune { get; }
    int RevizuiriSesiune { get; }
    void SetUtilizator(UtilizatorDto utilizator);
    void Deconecteaza();
    void SetSesiune(int sesiuneId);
    void SetCuvinteNoi(int nr);
    void SetRevizuiriSesiune(int nr);
}

public class NavigationService : INavigationService
{
    public async Task MergeAsync(string ruta) =>
        await Shell.Current.GoToAsync(ruta);

    public async Task InapoiAsync() =>
        await Shell.Current.GoToAsync("..");
}

public class SessionStateService : ISessionStateService
{
    private UtilizatorDto? _utilizator;
    private int? _sesiune;
    private int _cuvinteNoi = 10;
    private int _revizuiri = 100;

    public bool EsteAutentificat => _utilizator != null;
    public UtilizatorDto? UtilizatorCurent => _utilizator;
    public int? SesiuneCurenta => _sesiune;
    public int CuvinteNoiSesiune => _cuvinteNoi;
    public int RevizuiriSesiune => _revizuiri;

    public void SetUtilizator(UtilizatorDto u) => _utilizator = u;
    public void SetSesiune(int id) => _sesiune = id;
    public void SetCuvinteNoi(int nr) => _cuvinteNoi = nr;
    public void SetRevizuiriSesiune(int nr) => _revizuiri = nr;

    public void Deconecteaza()
    {
        _utilizator = null;
        _sesiune = null;
        _cuvinteNoi = 10;
        _revizuiri = 100;
    }
}