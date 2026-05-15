using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LexaCard.Core.Interfaces;
using LexaCard.Services;

namespace LexaCard.ViewModels;

public partial class SesiuneConfigViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;
    private readonly FluxViewModel _fluxVm;

    [ObservableProperty] int _nrRevizuiri = 0;
    [ObservableProperty] int _nrCuvinteNoi = 0;
    [ObservableProperty] int _maxCuvinteNoi = 50;
    [ObservableProperty] int _totalSesiune = 0;
    [ObservableProperty] string _estimareTimp = "";

    private int _cuvinteNoi = 10;
    public int CuvinteNoi
    {
        get => _cuvinteNoi;
        set
        {
            int clamped = Math.Clamp(value, 0, _maxCuvinteNoi);
            if (SetProperty(ref _cuvinteNoi, clamped))
            {
                OnPropertyChanged(nameof(ProgressCuvinte));
                ActualizeazaRezumat();
            }
        }
    }

    public double ProgressCuvinte =>
        _maxCuvinteNoi > 0 ? (double)_cuvinteNoi / _maxCuvinteNoi : 0;

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
        MaxCuvinteNoi = Math.Max(50, NrCuvinteNoi);

        int defaultNoi = _session.UtilizatorCurent.CarduriNoiPerZi;
        CuvinteNoi = NrCuvinteNoi > 0
            ? Math.Min(defaultNoi, NrCuvinteNoi)
            : 0;

        ActualizeazaRezumat();
    }

    private void ActualizeazaRezumat()
    {
        TotalSesiune = NrRevizuiri + CuvinteNoi;
        int min = (int)Math.Ceiling(TotalSesiune * 0.5);
        EstimareTimp = TotalSesiune > 0
            ? $"Estimat: ~{min} minute"
            : "Nimic programat — alege cuvinte noi";
    }

    [RelayCommand]
    void AdaugaCuvinte() => CuvinteNoi += 1;

    [RelayCommand]
    void ScadeCuvinte() => CuvinteNoi -= 1;

    [RelayCommand]
    void SeteazaCuvinte(string valoare)
    {
        if (int.TryParse(valoare, out int val))
            CuvinteNoi = val;
    }

    [RelayCommand]
    async Task InceptSesiuneAsync()
    {
        if (_session.UtilizatorCurent == null) return;
        _session.SetCuvinteNoi(CuvinteNoi);
        _fluxVm.ResetSesiune();
        await Shell.Current.GoToAsync("//FluxPage");
    }

    [RelayCommand]
    async Task InapoiAsync() =>
        await Shell.Current.GoToAsync("//MainPage");
}