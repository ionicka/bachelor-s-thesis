using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Interfaces;
using FlashCards.Services;

namespace FlashCards.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;

    [ObservableProperty] string _numeUtilizator = string.Empty;
    [ObservableProperty] string _initialaUtilizator = "?";
    [ObservableProperty] string _salut = "Buna ziua,";
    [ObservableProperty] int _streak = 0;
    [ObservableProperty] int _monede = 0;
    [ObservableProperty] int _nrNotificari = 0;
    [ObservableProperty] bool _areNotificari = false;
    [ObservableProperty] int _carduriAzi = 0;
    [ObservableProperty] bool _esteAdmin = false;
    [ObservableProperty] double _progressZi = 0.0;

    // Calendarul saptamanal — 7 zile L-D
    [ObservableProperty] List<ZiCalendar> _zileSaptamana = new();

    public MainViewModel(ICardService cardService, ISessionStateService session)
    {
        _cardService = cardService;
        _session = session;
    }

    public async Task IncarcaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        NumeUtilizator = _session.UtilizatorCurent.NumeUtilizator;
        InitialaUtilizator = NumeUtilizator.Length > 0
            ? NumeUtilizator[0].ToString().ToUpper() : "?";
        EsteAdmin = _session.EsteAdmin;

        int ora = DateTime.Now.Hour;
        Salut = ora switch
        {
            >= 5 and < 12 => "Buna dimineata,",
            >= 12 and < 18 => "Buna ziua,",
            >= 18 and < 22 => "Buna seara,",
            _ => "Noapte buna,"
        };

        try
        {
            var stats = await _cardService.GetStatisticiAsync(
                _session.UtilizatorCurent.Id);

            CarduriAzi = stats.CuvinteDeRevizuitAzi;
            Streak = stats.ZileCurenteStreak;
            Monede = stats.CuvinteInvatate * 10;
            NrNotificari = stats.CuvinteDeRevizuitAzi > 0 ? 1 : 0;
            AreNotificari = NrNotificari > 0;
            ProgressZi = stats.TotalCuvinte == 0 ? 0
                : Math.Min(1.0, (double)stats.CuvinteInvatate / stats.TotalCuvinte);

            // Construieste calendarul saptamanal
            ZileSaptamana = ConstruiesteCalendar(stats.IstoricSaptamana);
        }
        catch { }
    }

    private static List<ZiCalendar> ConstruiesteCalendar(
        List<StatisticiZilniceDto> istoric)
    {
        var azi = DateOnly.FromDateTime(DateTime.Now);

        // Gaseste inceputul saptamanii curente (Luni)
        int ziSapt = (int)azi.DayOfWeek;
        if (ziSapt == 0) ziSapt = 7; // Duminica = 7
        var luni = azi.AddDays(-(ziSapt - 1));

        var zileCuStudiu = istoric
            .Select(z => z.Data)
            .ToHashSet();

        var zile = new List<ZiCalendar>();
        string[] etichete = { "L", "M", "M", "J", "V", "S", "D" };

        for (int i = 0; i < 7; i++)
        {
            var zi = luni.AddDays(i);
            bool esteAzi = zi == azi;
            bool aStudiat = zileCuStudiu.Contains(zi);
            bool esteViitor = zi > azi;

            zile.Add(new ZiCalendar
            {
                Eticheta = etichete[i],
                Data = zi,
                AStudiat = aStudiat,
                EsteAzi = esteAzi,
                EsteViitor = esteViitor,
                Culoare = esteViitor ? "#1E3A5C" :
                              aStudiat ? "#4CAF50" :
                              esteAzi ? "#E94560" :
                                            "#2E3A5C"
            });
        }

        return zile;
    }

    [RelayCommand]
    async Task MergeVocabularAsync() =>
        await Shell.Current.GoToAsync("//SesiuneConfigPage");
    [RelayCommand]
    async Task DeschideAdminPanel() =>
    await Shell.Current.GoToAsync("//AdminPanelPage");
}

public class ZiCalendar
{
    public string Eticheta { get; set; } = "";
    public DateOnly Data { get; set; }
    public bool AStudiat { get; set; }
    public bool EsteAzi { get; set; }
    public bool EsteViitor { get; set; }
    public string Culoare { get; set; } = "#2E3A5C";

}