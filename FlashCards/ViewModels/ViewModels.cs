using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using FlashCards.Views;

namespace FlashCards.ViewModels;

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
// CardSesiune — wrapper pentru card in sesiune
// ═══════════════════════════════════════════════════════
public class CardSesiune
{
    public CardDto Card { get; }
    public TipRaspuns Tip { get; }
    public bool EstePrimaVezut { get; }
    public int NrGresite { get; private set; }

    public CardSesiune(CardDto card, TipRaspuns tip,
                       bool estePrimaVezut = false, int nrGresite = 0)
    {
        Card = card;
        Tip = tip;
        EstePrimaVezut = estePrimaVezut;
        NrGresite = nrGresite;
    }

    public void IncrementeazaGresite() => NrGresite++;
}

// ═══════════════════════════════════════════════════════
// FluxViewModel — logica completa SRS
// ═══════════════════════════════════════════════════════
public partial class FluxViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;
    private readonly ISesiuneService _sesiuneService;

    private Queue<CardSesiune> _coada = new();
    private Queue<CardSesiune> _coadaRetry = new();
    private bool _sesiuneIncarcata = false;
    private DateTime _momentAfisare = DateTime.UtcNow;
    private CardSesiune? _cardSesiuneCurent;
    private int _totalCarduri = 0;
    private int _cardVazut = 0;

    [ObservableProperty] CardDto? _cardCurent;
    [ObservableProperty] bool _propozitieRevelata = false;
    [ObservableProperty] bool _seIncarca = false;
    [ObservableProperty] bool _modTastare = false;
    [ObservableProperty] string _textTastat = string.Empty;
    [ObservableProperty] bool _sesiuneGoala = false;
    [ObservableProperty] int _nrCorect = 0;
    [ObservableProperty] int _nrGresit = 0;
    [ObservableProperty] string _mesajFeedback = string.Empty;
    [ObservableProperty] string _colorFeedback = "Transparent";
    [ObservableProperty] bool _asteaptaMailDeparte = false;
    [ObservableProperty] string _progressText = "";
    [ObservableProperty] int _indexImagine = 0;
    [ObservableProperty] int _indexExemplu = 0;
    [ObservableProperty] bool _aratDefinitieRo = false;

    public string? ImagineCurenta =>
        CardCurent?.Imagini.ElementAtOrDefault(IndexImagine);

    public bool AreImagini =>
        CardCurent?.Imagini.Count > 0;

    public bool AreDuaImagini =>
        (CardCurent?.Imagini.Count ?? 0) >= 2;

    public string ExempluCurentBlur =>
        CardCurent == null ? "" :
        (CardCurent.Exemple.ElementAtOrDefault(IndexExemplu) ?? "")
            .Replace("[TERMEN]", new string('_', CardCurent.Termen.Length));

    public string ExempluCurentRevelat =>
        CardCurent == null ? "" :
        (CardCurent.Exemple.ElementAtOrDefault(IndexExemplu) ?? "")
            .Replace("[TERMEN]", CardCurent.Termen);

    // Returneaza partile exemplului pentru colorare: inainte|cuvant|dupa
   

    public string DefinitieAfisata =>
        _aratDefinitieRo && CardCurent?.DefinitieRo != null
            ? CardCurent.DefinitieRo
            : CardCurent?.Definitie ?? "";

    public string BtnTradLabel =>
        _aratDefinitieRo ? "🇬🇧 EN" : "🇷🇴 RO";

    public FluxViewModel(
     ICardService cardService,
     ISessionStateService session,
     ISesiuneService sesiuneService)
    {
        _cardService = cardService;
        _session = session;
        _sesiuneService = sesiuneService;

        _session.LaDeconectare += ResetSesiune;  // ← NOU
    }

    partial void OnCardCurentChanged(CardDto? value)
    {
        // Versiune sigură — DOAR notificări, fără setări care pot loop-a
        OnPropertyChanged(nameof(ImagineCurenta));
        OnPropertyChanged(nameof(AreImagini));
        OnPropertyChanged(nameof(AreDuaImagini));
        OnPropertyChanged(nameof(ExempluCurentBlur));
        OnPropertyChanged(nameof(ExempluCurentRevelat));
       
        OnPropertyChanged(nameof(DefinitieAfisata));
        OnPropertyChanged(nameof(BtnTradLabel));
    }

    public async Task InitializeazaAsync()
    {
        if (_session.UtilizatorCurent == null) return;

        // Sesiune noua marcata explicit din SesiuneConfigPage
        if (_sesiuneNoua)
        {
            _sesiuneIncarcata = false;
            _sesiuneNoua = false;
            ResetareCompleta();
        }

        if (_sesiuneIncarcata) return;

        ResetareCompleta();
        SeIncarca = true;
        try
        {
            // Porneste o sesiune noua in BD
            if (_session.UtilizatorCurent != null)
            {
                int sesId = await _sesiuneService.IncepeSesiuneAsync(
                    _session.UtilizatorCurent.Id);
                _session.SetSesiune(sesId);
            }

            int cuvinteNoi = _session.CuvinteNoiSesiune;
            int maxRevizuiri = _session.RevizuiriSesiune;
            var carduri = await _cardService.GetSesiuneAsync(
                _session.UtilizatorCurent.Id, cuvinteNoi, maxRevizuiri);

            _sesiuneIncarcata = true;

            foreach (var card in carduri)
            {
                if (card.EsteNou)
                {
                    // Cuvant nou: recunoastere prima
                    _coada.Enqueue(new CardSesiune(card, TipRaspuns.Recunoastere, estePrimaVezut: true));
                    // Tastarea se va adauga in coadaRetry dupa recunoastere (nu imediat dupa)
                }
                else
                {
                    _coada.Enqueue(new CardSesiune(card, card.TipRaspunsRecomandat, estePrimaVezut: false));
                }
            }

            // Totalul = carduri unice (nu dublate)
            _totalCarduri = carduri.Count;
            _cardVazut = 0;
            ActualizeazaProgress();

            await AfiseazaUrmatorCard();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!
                .DisplayAlert("Eroare", ex.Message, "OK");
        }
        finally { SeIncarca = false; }
    }

    // Setat explicit de SesiuneConfigPage inainte de navigare
    private bool _sesiuneNoua = false;

    public void MarcheazaSesiuneNoua()
    {
        _sesiuneNoua = true;
    }

    public void ResetSesiune()
    {
        _sesiuneIncarcata = false;
        _sesiuneNoua = false;
        ResetareCompleta();
    }

    private void ResetareCompleta()
    {
        _coada = new();
        _coadaRetry = new();
        _cardSesiuneCurent = null;
        _totalCarduri = 0;
        _cardVazut = 0;
        CardCurent = null;
        SesiuneGoala = false;
        SeIncarca = false;   // FIX: reseteaza loading
        PropozitieRevelata = false;
        ModTastare = false;
        TextTastat = string.Empty;
        MesajFeedback = string.Empty;
        ColorFeedback = "Transparent";
        AsteaptaMailDeparte = false;
        ProgressText = "";
        NrCorect = 0;
        NrGresit = 0;
    }

    private async Task AfiseazaUrmatorCard()
    {
        PropozitieRevelata = false;
        TextTastat = string.Empty;
        MesajFeedback = string.Empty;
        ColorFeedback = "Transparent";
        AsteaptaMailDeparte = false;
        IndexImagine = 0;      
        IndexExemplu = 0;       
        AratDefinitieRo = false;

        if (_coada.TryDequeue(out var urmator))
        {
            _cardSesiuneCurent = urmator;
            CardCurent = urmator.Card;
            ModTastare = urmator.Tip == TipRaspuns.RemintireActiva;
            _momentAfisare = DateTime.UtcNow;
            ActualizeazaProgress();
            return;
        }

        if (_coadaRetry.TryDequeue(out var retry))
        {
            _cardSesiuneCurent = retry;
            CardCurent = retry.Card;
            ModTastare = retry.Tip == TipRaspuns.RemintireActiva;
            _momentAfisare = DateTime.UtcNow;
            ActualizeazaProgress();
            return;
        }

        // SAFETY: marchează că nu mai e card înainte de orice operație async
        CardCurent = null;
        PropozitieRevelata = false;
        AsteaptaMailDeparte = false;

        // Închide sesiunea în BD doar dacă nu e deja închisă
        if (_session.SesiuneCurenta.HasValue && !SesiuneGoala)
        {
            try
            {
                await _sesiuneService.InchideSesiuneAsync(
                    _session.SesiuneCurenta.Value,
                    NrCorect + NrGresit,
                    NrCorect,
                    NrGresit);
            }
            catch { }
        }

        SesiuneGoala = true;
    }

    private void ActualizeazaProgress()
    {
        ProgressText = _totalCarduri > 0
            ? $"{Math.Min(_cardVazut + 1, _totalCarduri)} din {_totalCarduri}"
            : "";
    }

    private void FinalizatCard()
    {
        // Creste contorul doar cand cardul nu mai apare in sesiune
        _cardVazut = Math.Min(_cardVazut + 1, _totalCarduri);
    }

    [RelayCommand]
    async Task PronuntaCuvantAsync()
    {
        if (CardCurent == null) return;
        try
        {
            var settings = new SpeechOptions
            {
                Locale = await GetEnglishLocale(),
                Pitch = 1.0f,
                Volume = 1.0f
            };
            await TextToSpeech.Default.SpeakAsync(CardCurent.Termen, settings);
        }
        catch
        {
            // Fallback fara locale
            await TextToSpeech.Default.SpeakAsync(CardCurent.Termen);
        }
    }

    [RelayCommand]
    async Task PronuntaExempluAsync()
    {
        if (CardCurent == null) return;
        try
        {
            var settings = new SpeechOptions
            {
                Locale = await GetEnglishLocale(),
                Pitch = 1.0f,
                Volume = 1.0f
            };
            await TextToSpeech.Default.SpeakAsync(ExempluCurentRevelat, settings);
        }
        catch
        {
            await TextToSpeech.Default.SpeakAsync(ExempluCurentRevelat);
        }
    }

    private static async Task<Locale?> GetEnglishLocale()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        return locales.FirstOrDefault(l =>
            l.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }

    [RelayCommand]
    void SchimbaImagine()
    {
        if (CardCurent == null || CardCurent.Imagini.Count < 2) return;
        IndexImagine = IndexImagine == 0 ? 1 : 0;
        OnPropertyChanged(nameof(ImagineCurenta));
    }

    [RelayCommand]
    void SchimbaExemplu()
    {
        if (CardCurent == null || CardCurent.Exemple.Count == 0) return;
        IndexExemplu = (IndexExemplu + 1) % CardCurent.Exemple.Count;
        OnPropertyChanged(nameof(ExempluCurentBlur));
        OnPropertyChanged(nameof(ExempluCurentRevelat));
        
    }

    [RelayCommand]
    void ToggleTraducere()
    {
        AratDefinitieRo = !AratDefinitieRo;
        OnPropertyChanged(nameof(DefinitieAfisata));
        OnPropertyChanged(nameof(BtnTradLabel));
    }
    [RelayCommand]
    void RevelazaPropozitia()
    {
        // Idempotent: dacă e deja revelată, nu face nimic
        if (PropozitieRevelata) return;

        PropozitieRevelata = true;
        OnPropertyChanged(nameof(ExempluCurentRevelat));
       
    }

    [RelayCommand]
    async Task RaspundeRecunoastereAsync(string calitateStr)
    {
        if (CardCurent == null || _cardSesiuneCurent == null) return;

        var calitate = calitateStr switch
        {
            "sigur" => CalitatRaspuns.Stiu_Sigur,
            "ezitare" => CalitatRaspuns.Stiu_Ezitare,
            _ => CalitatRaspuns.Nu_Stiu
        };

        MesajFeedback = calitate switch
        {
            CalitatRaspuns.Stiu_Sigur => "✓ Excelent!",
            CalitatRaspuns.Stiu_Ezitare => "~ Bine!",
            _ => "✗ Data viitoare!"
        };
        ColorFeedback = calitate switch
        {
            CalitatRaspuns.Stiu_Sigur => "#4CAF50",
            CalitatRaspuns.Stiu_Ezitare => "#2980B9",
            _ => "#E94560"
        };

        if (calitate == CalitatRaspuns.Nu_Stiu)
        {
            NrGresit++;
            _cardSesiuneCurent.IncrementeazaGresite();

            // FIX: adaugă DOAR o singură copie, nu două
            // Cardul reapare doar ca recunoaștere — dacă greșește iar, atunci se va re-evalua
            if (_cardSesiuneCurent.NrGresite < 5)
            {
                _coadaRetry.Enqueue(new CardSesiune(
                    CardCurent, TipRaspuns.Recunoastere,
                    estePrimaVezut: false,
                    nrGresite: _cardSesiuneCurent.NrGresite));
            }
        }
        else
        {
            NrCorect++;
            if (_cardSesiuneCurent.EstePrimaVezut)
            {
                // FIX: o singură instanță, nu duplicat
                _coada.Enqueue(new CardSesiune(CardCurent, TipRaspuns.RemintireActiva));
            }
        }

        if (!_cardSesiuneCurent.EstePrimaVezut)
        {
            await TrimiteRaspunsAsync(calitate, TipRaspuns.Recunoastere, null);
            if (calitate != CalitatRaspuns.Nu_Stiu)
                FinalizatCard();
        }

        AsteaptaMailDeparte = true;
    }

    private bool _navigareInCurs = false;

    [RelayCommand]
    async Task MailDeparte()
    {
        if (_navigareInCurs) return;
        _navigareInCurs = true;
        try
        {
            await AfiseazaUrmatorCard();
        }
        finally
        {
            _navigareInCurs = false;
        }
    }

    [RelayCommand]
    async Task RaspundeTastareAsync()
    {
        if (CardCurent == null || _cardSesiuneCurent == null) return;

        string tastat = TextTastat.Trim().ToLowerInvariant();
        string corect = CardCurent.Termen.Trim().ToLowerInvariant();
        bool ok = tastat == corect || Levenshtein(tastat, corect) <= 1;

        PropozitieRevelata = true;
        MesajFeedback = ok ? "✓ Corect!" : $"✗ Era: {CardCurent.Termen}";
        ColorFeedback = ok ? "#4CAF50" : "#E94560";

        if (ok)
        {
            NrCorect++;
            var calitateSalvata = _cardSesiuneCurent.NrGresite switch
            {
                0 => CalitatRaspuns.Tastat_Corect,
                1 => CalitatRaspuns.Stiu_Sigur,
                2 => CalitatRaspuns.Stiu_Ezitare,
                _ => CalitatRaspuns.Nu_Stiu
            };
            await TrimiteRaspunsAsync(calitateSalvata, TipRaspuns.RemintireActiva, TextTastat);
            FinalizatCard();
            // NU mai adaugam in coada - cardul e terminat
        }
        else
        {
            NrGresit++;
            _cardSesiuneCurent.IncrementeazaGresite();
            await TrimiteRaspunsAsync(CalitatRaspuns.Nu_Stiu, TipRaspuns.RemintireActiva, TextTastat);

            // Reapare in retry — cu noul CardSesiune care mosteneste NrGresite
            int nrGresite = _cardSesiuneCurent.NrGresite;
            if (nrGresite <= 5)
            {
                _coadaRetry.Enqueue(new CardSesiune(
                    CardCurent, TipRaspuns.Recunoastere,
                    estePrimaVezut: false, nrGresite: nrGresite));
                _coadaRetry.Enqueue(new CardSesiune(
                    CardCurent, TipRaspuns.RemintireActiva,
                    estePrimaVezut: false, nrGresite: nrGresite));
            }
            // Daca a depasit 5 incercari, cardul e abandonat din sesiune
        }

        await Task.Delay(400);
        AsteaptaMailDeparte = true;
    }

    [RelayCommand]
    async Task MergeInapoiAsync() =>
        await Shell.Current.GoToAsync("//MainPage");

    [RelayCommand]
    async Task PracticaMailMultAsync()
    {
        // Navigheaza inapoi la config cu modul practica libera
        await Shell.Current.GoToAsync("//SesiuneConfigPage?practica=true");
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
        }
        catch { }
    }

    private static int Levenshtein(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
            }
        return dp[a.Length, b.Length];
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
    void AdaugaCarduri() =>
        CarduriNoiPerZi = Math.Min(100, CarduriNoiPerZi + 1);

    [RelayCommand]
    void ScadeCarduri() =>
        CarduriNoiPerZi = Math.Max(1, CarduriNoiPerZi - 1);

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