using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Helpers;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using System.Collections.ObjectModel;

namespace FlashCards.ViewModels;
public partial class SesiuneConfigViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;
    private readonly FluxViewModel _fluxVm;

    private const int MIN_CARDURI = 1;
    private const int MAX_CARDURI = 30;

    private bool _initializat = false;

    // ─── Disponibilitate live ───
    [ObservableProperty] int _nrRevizuiri = 0;
    [ObservableProperty] int _nrCuvinteNoi = 0;
    [ObservableProperty] int _totalDisponibil = 0;
    [ObservableProperty] bool _seCalculeazaDisponibilitate = false;

    // ─── Selecții ───
    [ObservableProperty] ObservableCollection<ChipFiltruNivelVm> _nivelChips = new();
    [ObservableProperty] ObservableCollection<ChipFiltruDomeniuVm> _domeniuChips = new();

    // Mod învățare — single select
    private int _modIndex = 0;
    public int ModIndex
    {
        get => _modIndex;
        set
        {
            if (SetProperty(ref _modIndex, value))
            {
                OnPropertyChanged(nameof(EstimareTimp));
                OnPropertyChanged(nameof(ModToateSelectat));
                OnPropertyChanged(nameof(ModFlashcardsSelectat));
                OnPropertyChanged(nameof(ModTastareSelectat));

                OnPropertyChanged(nameof(ModToateBg));
                OnPropertyChanged(nameof(ModFlashcardsBg));
                OnPropertyChanged(nameof(ModTastareBg));
                OnPropertyChanged(nameof(ModToateText));
                OnPropertyChanged(nameof(ModFlashcardsText));
                OnPropertyChanged(nameof(ModTastareText));
            }
        }
    }

    public bool ModToateSelectat => ModIndex == 0;
    public bool ModFlashcardsSelectat => ModIndex == 1;
    public bool ModTastareSelectat => ModIndex == 2;

    // Culori pentru cele 3 carduri mod
    public string ModToateBg => ModToateSelectat ? "#8B7FE8" : "#FFFFFF";
    public string ModFlashcardsBg => ModFlashcardsSelectat ? "#5FA8D3" : "#FFFFFF";
    public string ModTastareBg => ModTastareSelectat ? "#F2685C" : "#FFFFFF";

    public string ModToateText => ModToateSelectat ? "#FFFFFF" : "#6B7A99";
    public string ModFlashcardsText => ModFlashcardsSelectat ? "#FFFFFF" : "#6B7A99";
    public string ModTastareText => ModTastareSelectat ? "#FFFFFF" : "#6B7A99";

    // Nr carduri
    private int _nrCarduri = 10;
    public int NrCarduri
    {
        get => _nrCarduri;
        set
        {
            int clamped = Math.Clamp(value, MIN_CARDURI, MAX_CARDURI);
            if (SetProperty(ref _nrCarduri, clamped))
            {
                OnPropertyChanged(nameof(EstimareTimp));
                OnPropertyChanged(nameof(PoateIncepe));
                OnPropertyChanged(nameof(RevizuiriInSesiune));
                OnPropertyChanged(nameof(CuvinteNoiInSesiune));
                OnPropertyChanged(nameof(NrCarduriLabel));
            }
        }
    }

    public string NrCarduriLabel => $"{NrCarduri} carduri";

    // ─── Computed: compoziția sesiunii ───
    public int RevizuiriInSesiune => Math.Min(NrRevizuiri, NrCarduri);
    public int CuvinteNoiInSesiune
    {
        get
        {
            int ramas = NrCarduri - RevizuiriInSesiune;
            return Math.Min(NrCuvinteNoi, ramas);
        }
    }

    public string EstimareTimp
    {
        get
        {
            int totalEfectiv = RevizuiriInSesiune + CuvinteNoiInSesiune;
            if (totalEfectiv == 0) return "";

            double secPerCard = ModIndex switch
            {
                1 => 25,
                2 => 50,
                _ => 35
            };
            int totalSec = (int)(totalEfectiv * secPerCard);
            int min = (int)Math.Ceiling(totalSec / 60.0);
            return min <= 1 ? "~1 minut" : $"~{min} minute";
        }
    }

    public bool PoateIncepe => TotalDisponibil > 0 && NrCarduri > 0;

    public SesiuneConfigViewModel(
        ICardService cardService,
        ISessionStateService session,
        FluxViewModel fluxVm)
    {
        _cardService = cardService;
        _session = session;
        _fluxVm = fluxVm;

        ConstruiesteChips();
    }

    private void ConstruiesteChips()
    {
        NivelChips.Clear();
        foreach (var nivel in EnumLabels.ToateNivelele)
        {
            var chip = new ChipFiltruNivelVm(nivel, EnumLabels.Label(nivel), CuloareNivel(nivel));
            chip.PropertyChanged += OnFiltruChanged;
            NivelChips.Add(chip);
        }

        DomeniuChips.Clear();
        foreach (var domeniu in EnumLabels.ToateDomenii)
        {
            var chip = new ChipFiltruDomeniuVm(
                domeniu,
                EnumLabels.Label(domeniu),
                EnumLabels.Icon(domeniu),
                CuloareDomeniu(domeniu));
            chip.PropertyChanged += OnFiltruChanged;
            DomeniuChips.Add(chip);
        }
    }

    private void OnFiltruChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!_initializat) return;
        if (e.PropertyName != "EsteSelectat") return;
        _ = RecalculeazaDisponibilitateAsync();
    }

    public async Task IncarcaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        _initializat = false;

        var cfg = _session.ConfigSesiune;

        foreach (var chip in NivelChips)
            chip.EsteSelectatSilent(cfg.Niveluri.Contains(chip.Valoare));

        foreach (var chip in DomeniuChips)
            chip.EsteSelectatSilent(cfg.Domenii.Contains(chip.Valoare));

        ModIndex = (int)cfg.Mod;
        NrCarduri = cfg.NrCarduri;

        _initializat = true;

        await RecalculeazaDisponibilitateAsync();
    }

    public async Task OnAppearingAsync()
    {
        await IncarcaAsync();
    }
    private async Task RecalculeazaDisponibilitateAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        SeCalculeazaDisponibilitate = true;
        try
        {
            var niveluri = NivelChips.Where(c => c.EsteSelectat).Select(c => c.Valoare).ToList();
            var domenii = DomeniuChips.Where(c => c.EsteSelectat).Select(c => c.Valoare).ToList();

            var taskStats = _cardService.GetDisponibilitateAsync(
     _session.UtilizatorCurent.Id, niveluri, domenii);

            var disp = await taskStats;

            NrRevizuiri = disp.NrRevizuiri;
            NrCuvinteNoi = disp.NrCuvinteNoi;
            TotalDisponibil = disp.Total;

            OnPropertyChanged(nameof(RevizuiriInSesiune));
            OnPropertyChanged(nameof(CuvinteNoiInSesiune));
            OnPropertyChanged(nameof(EstimareTimp));
            OnPropertyChanged(nameof(PoateIncepe));
        }
        finally
        {
            SeCalculeazaDisponibilitate = false;
        }
    }
    [RelayCommand]
    void SeteazaModToate() => ModIndex = 0;

    [RelayCommand]
    void SeteazaModFlashcards() => ModIndex = 1;

    [RelayCommand]
    void SeteazaModTastare() => ModIndex = 2;

    [RelayCommand]
    void AdaugaCarduri() => NrCarduri = Math.Min(MAX_CARDURI, NrCarduri + 1);

    [RelayCommand]
    void ScadeCarduri() => NrCarduri = Math.Max(MIN_CARDURI, NrCarduri - 1);

    [RelayCommand]
    void SeteazaCarduri(string valoare)
    {
        if (int.TryParse(valoare, out int val))
            NrCarduri = val;
    }

    [RelayCommand]
    void ReseteazaToateFiltre()
    {
        foreach (var c in NivelChips) c.EsteSelectatSilent(false);
        foreach (var c in DomeniuChips) c.EsteSelectatSilent(false);
        ModIndex = 0;
        _ = RecalculeazaDisponibilitateAsync();
    }

    [RelayCommand]
    async Task InceptSesiuneAsync()
    {
        if (!PoateIncepe || _session.UtilizatorCurent == null) return;

        var config = new ConfigSesiuneDto
        {
            Niveluri = NivelChips.Where(c => c.EsteSelectat).Select(c => c.Valoare).ToList(),
            Domenii = DomeniuChips.Where(c => c.EsteSelectat).Select(c => c.Valoare).ToList(),
            Mod = (ModInvatare)ModIndex,
            NrCarduri = NrCarduri
        };

        _session.SetConfigSesiune(config);
        _session.SetRevizuiriSesiune(RevizuiriInSesiune);
        _session.SetCuvinteNoi(CuvinteNoiInSesiune);

        _fluxVm.MarcheazaSesiuneNoua();
        await Shell.Current.GoToAsync("//FluxPage");
    }

    [RelayCommand]
    async Task InapoiAsync() => await Shell.Current.GoToAsync("//MainPage");

    [RelayCommand]
    async Task MergeListaCarduriAsync() =>
    await Shell.Current.GoToAsync("//VocabularPage");
    private static string CuloareNivel(NivelCuvant n) => n switch
    {
        NivelCuvant.Elementar => "#6FBF73",   // verde — ușor
        NivelCuvant.Intermediar => "#F2A65A", // portocaliu — mediu
        NivelCuvant.Avansat => "#F2685C",     // roșu — greu
        _ => "#9AA8C2"
    };

    private static string CuloareDomeniu(DomeniuCuvant d) => d switch
    {
        DomeniuCuvant.General => "#9AA8C2",
        DomeniuCuvant.Business => "#FFD700",
        DomeniuCuvant.Tehnologie => "#5FA8D3",
        DomeniuCuvant.Sanatate => "#F2685C",
        DomeniuCuvant.Educatie => "#8B7FE8",
        DomeniuCuvant.Cultura => "#F2A65A",
        DomeniuCuvant.Sport => "#6FBF73",
        DomeniuCuvant.Politica => "#8E44AD",
        DomeniuCuvant.Calatorii => "#16A085",
       

        _ => "#9AA8C2"
    };
}

// ═══════════════════════════════════════════════════════════════
// Chip pentru filtru NIVEL — non-generic, ușor de folosit în XAML
// ═══════════════════════════════════════════════════════════════
public partial class ChipFiltruNivelVm : ObservableObject
{
    public NivelCuvant Valoare { get; }
    public string Eticheta { get; }
    public string Culoare { get; }

    [ObservableProperty] bool _esteSelectat = false;

    public string BackgroundColor => EsteSelectat ? Culoare : "#FFFFFF";
    public string TextColor => EsteSelectat ? "#FFFFFF" : "#6B7A99";
    public string BorderColor => Culoare;

    public ChipFiltruNivelVm(NivelCuvant valoare, string eticheta, string culoare)
    {
        Valoare = valoare;
        Eticheta = eticheta;
        Culoare = culoare;
    }

    public void EsteSelectatSilent(bool valoare)
    {
        SetProperty(ref _esteSelectat, valoare, nameof(EsteSelectat));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
    }

    partial void OnEsteSelectatChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
    }

    [RelayCommand]
    void Toggle() => EsteSelectat = !EsteSelectat;
}

// ═══════════════════════════════════════════════════════════════
// Chip pentru filtru DOMENIU — non-generic
// ═══════════════════════════════════════════════════════════════
public partial class ChipFiltruDomeniuVm : ObservableObject
{
    public DomeniuCuvant Valoare { get; }
    public string Eticheta { get; }
    public string Icon { get; }
    public string Culoare { get; }

    [ObservableProperty] bool _esteSelectat = false;

    public string BackgroundColor => EsteSelectat ? Culoare : "#FFFFFF";
    public string TextColor => EsteSelectat ? "#FFFFFF" : "#6B7A99";
    public string BorderColor => Culoare;

    public ChipFiltruDomeniuVm(DomeniuCuvant valoare, string eticheta, string icon, string culoare)
    {
        Valoare = valoare;
        Eticheta = eticheta;
        Icon = icon;
        Culoare = culoare;
    }

    public void EsteSelectatSilent(bool valoare)
    {
        SetProperty(ref _esteSelectat, valoare, nameof(EsteSelectat));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
    }

    partial void OnEsteSelectatChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
    }

    [RelayCommand]
    void Toggle() => EsteSelectat = !EsteSelectat;
  
}