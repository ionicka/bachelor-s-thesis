using FlashCards.Services;
using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class FelicitariPage : ContentPage, IQueryAttributable
{
    private int _streak;
    private int _nrCorect;
    private int _nrGresit;
    private bool _primaAZilei;

    public FelicitariPage()
    {
        InitializeComponent();
    }

    // Apelat DE FIECARE DATĂ când navighezi cu query params,
    // chiar dacă pagina e deja instantiată (ShellContent reutilizat)
    private readonly ISessionStateService _session;

    public FelicitariPage(ISessionStateService session)
    {
        InitializeComponent();
        _session = session;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _streak = ToInt(query, "streak");
        _nrCorect = ToInt(query, "corect");
        _nrGresit = ToInt(query, "gresit");
        _primaAZilei = ToBool(query, "prima");

        BindingContext = new FelicitariViewModel(
    _streak, _nrCorect, _nrGresit, _primaAZilei,
    _session.CuvinteInvatateUltimaSesiune,
    _session.CuvinteIncompletUltimaSesiune);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine(
            $"[FELICITARI PAGE] OnAppearing: streak={_streak}, corect={_nrCorect}, gresit={_nrGresit}");
    }

    private async void OnContinuaClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//MainPage");

    private async void OnPracticaClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//SesiuneConfigPage");

    // ─── Helpers pentru conversia robustă a query params ───
    private static int ToInt(IDictionary<string, object> q, string key)
    {
        if (!q.TryGetValue(key, out var v) || v == null) return 0;
        return int.TryParse(v.ToString(), out int n) ? n : 0;
    }

    private static bool ToBool(IDictionary<string, object> q, string key)
    {
        if (!q.TryGetValue(key, out var v) || v == null) return false;
        return bool.TryParse(v.ToString(), out bool b) && b;
    }
}