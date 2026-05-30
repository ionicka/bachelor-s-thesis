using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.Interfaces;
using FlashCards.Services;

namespace FlashCards.ViewModels;

public partial class SesiuneConfigViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;
    private readonly FluxViewModel _fluxVm;

    private const int MAX_CARDURI = 50;
    private const int MIN_CARDURI = 1;

    [ObservableProperty] int _nrRevizuiri = 0;
    [ObservableProperty] int _nrCuvinteNoi = 0;
    [ObservableProperty] int _totalDisponibil = 0;
    [ObservableProperty] string _estimareTimp = "";
    [ObservableProperty] bool _poateIncepe = false;
    [ObservableProperty] int _revizuiriInSesiune = 0;
    [ObservableProperty] int _noiFInSesiune = 0;

    private int _totalSesiune = 10;
    public int TotalSesiune
    {
        get => _totalSesiune;
        set
        {
            int max = Math.Min(MAX_CARDURI, _totalDisponibil);
            int clamped = Math.Clamp(value, 0, Math.Max(0, max));
            if (SetProperty(ref _totalSesiune, clamped))
                ActualizeazaTot();
        }
    }

    public SesiuneConfigViewModel(
        ICardService cardService,
        ISessionStateService session,
        FluxViewModel fluxVm)
    {
        _cardService = cardService;
        _session = session;
        _fluxVm = fluxVm;
    }

    public async Task IncarcaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        var stats = await _cardService.GetStatisticiAsync(
            _session.UtilizatorCurent.Id);

        NrRevizuiri = stats.CuvinteDeRevizuitAzi;
        NrCuvinteNoi = stats.CuvinteNoi;
        TotalDisponibil = NrRevizuiri + NrCuvinteNoi;

        // Default: 10 sau totalul disponibil
        int defaultVal = Math.Min(10, TotalDisponibil);
        TotalSesiune = defaultVal;

        ActualizeazaTot();
    }

    private void ActualizeazaTot()
    {
        // Calculeaza compozitia sesiunii
        RevizuiriInSesiune = Math.Min(NrRevizuiri, TotalSesiune);
        int ramas = TotalSesiune - RevizuiriInSesiune;
        NoiFInSesiune = Math.Min(NrCuvinteNoi, ramas);

        int min = (int)Math.Ceiling(TotalSesiune * 0.5);
        EstimareTimp = TotalSesiune > 0
            ? $"Estimat: ~{min} minute"
            : "";

        PoateIncepe = TotalSesiune > 0 && TotalDisponibil > 0;
    }

    [RelayCommand]
    void AdaugaCarduri() => TotalSesiune += 1;

    [RelayCommand]
    void ScadeCarduri() => TotalSesiune -= 1;

    [RelayCommand]
    void SeteazaCarduri(string valoare)
    {
        if (int.TryParse(valoare, out int val))
            TotalSesiune = val;
    }

    [RelayCommand]
    async Task InceptSesiuneAsync()
    {
        if (_session.UtilizatorCurent == null) return;
        if (!PoateIncepe) return;

        // Seteaza compozitia sesiunii
        _session.SetRevizuiriSesiune(RevizuiriInSesiune);
        _session.SetCuvinteNoi(NoiFInSesiune);
        _fluxVm.MarcheazaSesiuneNoua();
        await Shell.Current.GoToAsync("//FluxPage");
    }

    [RelayCommand]
    async Task InapoiAsync() =>
        await Shell.Current.GoToAsync("//MainPage");
}