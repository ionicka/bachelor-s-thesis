using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LexaCard.Core.Interfaces;
using LexaCard.Services;
using LexaCard.Views;

namespace LexaCard.ViewModels;

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
    [ObservableProperty] double _progressZi = 0.0;

    public MainViewModel(ICardService cardService, ISessionStateService session)
    {
        _cardService = cardService;
        _session = session;
    }

    public async Task IncarcaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        // Seteaza datele utilizatorului
        NumeUtilizator = _session.UtilizatorCurent.NumeUtilizator;
        InitialaUtilizator = NumeUtilizator.Length > 0
            ? NumeUtilizator[0].ToString().ToUpper() : "?";

        // Salut in functie de ora
        int ora = DateTime.Now.Hour;
        Salut = ora switch
        {
            >= 5 and < 12 => "Buna dimineata,",
            >= 12 and < 18 => "Buna ziua,",
            >= 18 and < 22 => "Buna seara,",
            _ => "Noapte buna,"
        };

        // Statistici
        try
        {
            var stats = await _cardService.GetStatisticiAsync(
                _session.UtilizatorCurent.Id);

            CarduriAzi = stats.CuvinteDeRevizuitAzi + stats.CuvinteNoi;
            Streak = stats.ZileCurenteStreak;
            Monede = stats.CuvinteInvatate * 10; // 10 monede per cuvant invatat
            NrNotificari = stats.CuvinteDeRevizuitAzi > 0 ? 1 : 0;
            AreNotificari = NrNotificari > 0;
            ProgressZi = stats.TotalCuvinte == 0 ? 0
                : Math.Min(1.0, (double)stats.CuvinteInvatate / stats.TotalCuvinte);
        }
        catch { /* ignora erori de statistici */ }
    }

    [RelayCommand]
    async Task MergeVocabularAsync()
    {
        await Shell.Current.GoToAsync("//FluxPage");
    }
}