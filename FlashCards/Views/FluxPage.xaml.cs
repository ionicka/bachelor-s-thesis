using FlashCards.Core.Interfaces;
using FlashCards.Services;
using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class FluxPage : ContentPage
{
    private readonly FluxViewModel _vm;
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;
    private bool _abonata = false;

    public FluxPage(FluxViewModel vm, ICardService cardService,
                    ISessionStateService session)
    {
        InitializeComponent();
        _vm = vm;
        _cardService = cardService;
        _session = session;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_abonata)
        {
            _vm.PropertyChanged += OnVmPropertyChanged;
            _abonata = true;
        }

        await _vm.InitializeazaAsync();
    }

    private async void OnVmPropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(FluxViewModel.SesiuneGoala)) return;
        if (!_vm.SesiuneGoala) return;
        await AfiseazaFelicitari();
    }

    private async Task AfiseazaFelicitari()
    {
        if (_session.UtilizatorCurent == null) return;
        try
        {
            var stats = await _cardService.GetStatisticiAsync(
                _session.UtilizatorCurent.Id);
            bool primaAZilei = stats.SesiuniFinalizateAzi == 1;

            await Shell.Current.GoToAsync(
                $"//FelicitariPage?streak={stats.ZileCurenteStreak}" +
                $"&corect={_vm.NrCorect}" +
                $"&gresit={_vm.NrGresit}" +
                $"&prima={primaAZilei}");
        }
        catch { }
    }
}