using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LexaCard.Core.DTOs;
using LexaCard.Core.Enums;
using LexaCard.Core.Interfaces;
using LexaCard.Services;
using LexaCard.Views;

namespace LexaCard.ViewModels;

// ═══════════════════════════════════════════════════════
// LoginViewModel
// ═══════════════════════════════════════════════════════
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ISessionStateService _session;
    private readonly ISesiuneService _sesiuneService;

    [ObservableProperty] string _email = string.Empty;
    [ObservableProperty] string _parola = string.Empty;
    [ObservableProperty] string _mesajEroare = string.Empty;
    [ObservableProperty] bool _seIncarca = false;

    public LoginViewModel(
        IAuthService auth,
        ISessionStateService session,
        ISesiuneService sesiuneService)
    {
        _auth = auth;
        _session = session;
        _sesiuneService = sesiuneService;
    }

    [RelayCommand]
    async Task LogheazaAsync()
    {
        if (SeIncarca) return;
        MesajEroare = string.Empty;
        SeIncarca = true;
        try
        {
            var r = await _auth.LoginAsync(
                new LoginDto { Email = Email, Parola = Parola });

            if (r.Succes && r.Utilizator != null)
            {
                _session.SetUtilizator(r.Utilizator);
                int sesId = await _sesiuneService
                    .IncepeSesuineAsync(r.Utilizator.Id);
                _session.SetSesiune(sesId);
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                MesajEroare = r.Eroare ?? "Eroare necunoscuta.";
            }
        }
        catch (Exception ex)
        {
            MesajEroare = $"Eroare: {ex.Message}";
        }
        finally { SeIncarca = false; }
    }

    [RelayCommand]
    async Task MergeInregistrareAsync() =>
        await Shell.Current.GoToAsync(nameof(InregistrarePage));
}

// ═══════════════════════════════════════════════════════
// InregistrareViewModel
// ═══════════════════════════════════════════════════════
public partial class InregistrareViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] string _numeUtilizator = string.Empty;
    [ObservableProperty] string _email = string.Empty;
    [ObservableProperty] string _parola = string.Empty;
    [ObservableProperty] string _confirmaParola = string.Empty;
    [ObservableProperty] string _mesajEroare = string.Empty;
    [ObservableProperty] bool _seIncarca = false;

    public InregistrareViewModel(IAuthService auth) => _auth = auth;

    [RelayCommand]
    async Task InregistreazaAsync()
    {
        MesajEroare = string.Empty;
        SeIncarca = true;
        try
        {
            var dto = new InregistrareDto
            {
                NumeUtilizator = NumeUtilizator,
                Email = Email,
                Parola = Parola,
                ConfirmaParola = ConfirmaParola
            };
            var (succes, eroare) = await _auth.InregistreazaAsync(dto);
            if (succes)
            {
                await Application.Current!.MainPage!
                    .DisplayAlert("Succes", "Cont creat! Te poti autentifica.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                MesajEroare = eroare ?? "Eroare.";
            }
        }
        catch (Exception ex) { MesajEroare = ex.Message; }
        finally { SeIncarca = false; }
    }

    [RelayCommand]
    async Task InapoiAsync() => await Shell.Current.GoToAsync("..");
}

// ═══════════════════════════════════════════════════════
// FluxViewModel
// ═══════════════════════════════════════════════════════
public partial class FluxViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;

    private List<CardDto> _carduri = new();
    private int _index = 0;
    private DateTime _momentAfisare = DateTime.UtcNow;

    [ObservableProperty] CardDto? _cardCurent;
    [ObservableProperty] bool _propozitieRevelata = false;
    [ObservableProperty] bool _seIncarca = false;
    [ObservableProperty] bool _modTastare = false;
    [ObservableProperty] string _textTastat = string.Empty;
    [ObservableProperty] bool _sesiuneGoala = false;
    [ObservableProperty] int _nrCorect = 0;
    [ObservableProperty] int _nrGresit = 0;

    public FluxViewModel(ICardService cardService, ISessionStateService session)
    {
        _cardService = cardService;
        _session = session;
    }

    public async Task InitializeazaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        // Reseteaza starea
        _carduri = new();
        _index = 0;
        SesiuneGoala = false;
        PropozitieRevelata = false;
        CardCurent = null;

        SeIncarca = true;
        try
        {
            _carduri = await _cardService.GetFluxAsync(
                _session.UtilizatorCurent.Id);

            if (_carduri.Any())
                AfiseazaCard();
            else
                SesiuneGoala = true;
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!
                .DisplayAlert("Eroare", ex.Message, "OK");
        }
        finally { SeIncarca = false; }
    }

    [RelayCommand]
    void TreceLaUrmatorCard()
    {
        PropozitieRevelata = false;
        TextTastat = string.Empty;
        ModTastare = false;
        _index++;
        if (_index < _carduri.Count) AfiseazaCard();
        else SesiuneGoala = true;
    }

    [RelayCommand]
    void RevelazaPropozitia() => PropozitieRevelata = true;

    [RelayCommand]
    async Task RaspundeRecunoastereAsync(string calitateStr)
    {
        if (CardCurent == null) return;
        var calitate = calitateStr switch
        {
            "sigur" => CalitatRaspuns.Stiu_Sigur,
            "ezitare" => CalitatRaspuns.Stiu_Ezitare,
            _ => CalitatRaspuns.Nu_Stiu
        };
        await TrimiteRaspunsAsync(calitate, TipRaspuns.Recunoastere, null);
        TreceLaUrmatorCard();
    }

    [RelayCommand]
    async Task RaspundeTastareAsync()
    {
        if (CardCurent == null) return;
        await TrimiteRaspunsAsync(
            CalitatRaspuns.Stiu_Sigur,
            TipRaspuns.RemintireActiva,
            TextTastat);
        TreceLaUrmatorCard();
    }

    [RelayCommand]
    async Task MergeInapoiAsync() =>
        await Shell.Current.GoToAsync("//MainPage");

    [RelayCommand]
    async Task PracticaMailMultAsync()
    {
        // Incarca toate cuvintele indiferent de data revizuirii
        if (_session.UtilizatorCurent == null) return;
        SeIncarca = true;
        SesiuneGoala = false;
        try
        {
            _carduri = await _cardService.GetFluxAsync(
                _session.UtilizatorCurent.Id);

            // Daca tot nu sunt carduri, incarca aleatoriu toate cuvintele
            if (!_carduri.Any())
            {
                _carduri = await _cardService.GetToateCuvinteleAsync(
                    _session.UtilizatorCurent.Id);
            }

            if (_carduri.Any())
            {
                _index = 0;
                NrCorect = 0;
                NrGresit = 0;
                AfiseazaCard();
            }
            else
            {
                await Application.Current!.MainPage!
                    .DisplayAlert("Info",
                        "Nu exista cuvinte disponibile.", "OK");
            }
        }
        finally { SeIncarca = false; }
    }

    private async Task TrimiteRaspunsAsync(
        CalitatRaspuns calitate, TipRaspuns tip, string? text)
    {
        if (CardCurent == null || _session.UtilizatorCurent == null) return;
        try
        {
            int timp = (int)(DateTime.UtcNow - _momentAfisare).TotalSeconds;
            await _cardService.ProceseazaRaspunsAsync(
                _session.UtilizatorCurent.Id,
                new RaspunsCardDto
                {
                    CuvantId = CardCurent.CuvantId,
                    Calitate = calitate,
                    TipRaspuns = tip,
                    TimpRaspunsSec = timp,
                    TextTastat = text,
                    SesiuneId = _session.SesiuneCurenta
                });
            if (calitate != CalitatRaspuns.Nu_Stiu) NrCorect++;
            else NrGresit++;
        }
        catch { /* logam si continuam */ }
    }

    private void AfiseazaCard()
    {
        if (_index < _carduri.Count)
        {
            CardCurent = _carduri[_index];
            ModTastare = CardCurent.TipRaspunsRecomandat == TipRaspuns.RemintireActiva;
            _momentAfisare = DateTime.UtcNow;
        }
    }
}

// ═══════════════════════════════════════════════════════
// StatisticiViewModel
// ═══════════════════════════════════════════════════════
public partial class StatisticiViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;

    [ObservableProperty] StatisticiDto? _statistici;
    [ObservableProperty] bool _seIncarca = false;

    public StatisticiViewModel(ICardService cardService, ISessionStateService session)
    {
        _cardService = cardService;
        _session = session;
    }

    public async Task IncarcaAsync()
    {
        if (_session.UtilizatorCurent == null) return;
        SeIncarca = true;
        try
        {
            Statistici = await _cardService.GetStatisticiAsync(
                _session.UtilizatorCurent.Id);
        }
        finally { SeIncarca = false; }
    }
}

// ═══════════════════════════════════════════════════════
// SetariViewModel
// ═══════════════════════════════════════════════════════
public partial class SetariViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ISessionStateService _session;

    [ObservableProperty] string _numeUtilizator = string.Empty;
    [ObservableProperty] string _email = string.Empty;
    [ObservableProperty] int _carduriNoiPerZi = 10;
    [ObservableProperty] int _maxCarduriPerSesiune = 20;

    public SetariViewModel(IAuthService auth, ISessionStateService session)
    {
        _auth = auth;
        _session = session;

        if (session.UtilizatorCurent != null)
        {
            NumeUtilizator = session.UtilizatorCurent.NumeUtilizator;
            Email = session.UtilizatorCurent.Email;
            CarduriNoiPerZi = session.UtilizatorCurent.CarduriNoiPerZi;
            MaxCarduriPerSesiune = session.UtilizatorCurent.MaxCarduriPerSesiune;
        }
    }

    [RelayCommand]
    async Task SalveazaSetarileAsync()
    {
        if (_session.UtilizatorCurent == null) return;
        await _auth.ActualizeazaSetariAsync(
            _session.UtilizatorCurent.Id,
            CarduriNoiPerZi,
            MaxCarduriPerSesiune);
        await Application.Current!.MainPage!
            .DisplayAlert("Succes", "Setari salvate.", "OK");
    }

    [RelayCommand]
    async Task DeconecteazaAsync()
    {
        bool ok = await Application.Current!.MainPage!
            .DisplayAlert("Deconectare", "Esti sigur?", "Da", "Nu");
        if (!ok) return;
        _session.Deconecteaza();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}