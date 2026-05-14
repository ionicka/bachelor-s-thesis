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

    // Slider foloseste double intern, dar noi afisam int
    private double _sliderVal = 10;
    public double SliderVal
    {
        get => _sliderVal;
        set
        {
            if (SetProperty(ref _sliderVal, value))
            {
                OnPropertyChanged(nameof(CuvinteNoi));
                ActualizeazaRezumat();
            }
        }
    }

    // Proprietate int derivata din slider
    public int CuvinteNoi => (int)Math.Round(_sliderVal);

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
        SliderVal = NrCuvinteNoi > 0
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

    // Butoanele rapide seteaza direct SliderVal
    [RelayCommand]
    void SeteazaCuvinte(string valoare)
    {
        if (int.TryParse(valoare, out int val))
        {
            int limita = NrCuvinteNoi > 0 ? NrCuvinteNoi : 0;
            SliderVal = Math.Min(val, limita);
        }
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