using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;

namespace FlashCards.Services;

public interface INavigationService
{
    Task MergeAsync(string ruta);
    Task InapoiAsync();
}

public interface ISessionStateService
{
    bool EsteAutentificat { get; }
    bool EsteAdmin { get; }
    UtilizatorDto? UtilizatorCurent { get; }
    int? SesiuneCurenta { get; }
    int CuvinteNoiSesiune { get; }
    int RevizuiriSesiune { get; }

    event Action? LaDeconectare;
    List<CuvantInvatat> CuvinteInvatateUltimaSesiune { get; }
    void SetCuvinteInvatate(List<CuvantInvatat> cuvinte);

    void SetUtilizator(UtilizatorDto utilizator);
    void Deconecteaza();
    void SetSesiune(int sesiuneId);
    void SetCuvinteNoi(int nr);
    void SetRevizuiriSesiune(int nr);
    ConfigSesiuneDto ConfigSesiune { get; }
    void SetConfigSesiune(ConfigSesiuneDto config);
    List<CuvantIncomplet> CuvinteIncompletUltimaSesiune { get; }
    void SetCuvinteIncomplete(List<CuvantIncomplet> cuvinte);
}

public class NavigationService : INavigationService
{
    public async Task MergeAsync(string ruta) =>
        await Shell.Current.GoToAsync(ruta);

    public async Task InapoiAsync() =>
        await Shell.Current.GoToAsync("..");
}

public class SessionStateService : ISessionStateService
{
    private UtilizatorDto? _utilizator;
    private int? _sesiune;
    private int _cuvinteNoi = 10;
    private int _revizuiri = 100;
    private ConfigSesiuneDto? _configCache;

    // Chei pentru Preferences — namespaced pentru a evita coliziuni
    private const string KEY_NIVELURI = "config.niveluri";
    private const string KEY_DOMENII = "config.domenii";
    private const string KEY_MOD = "config.mod";
    private const string KEY_NR_CARDURI = "config.nr_carduri";

    public event Action? LaDeconectare;
    private List<CuvantInvatat> _cuvinteInvatate = new();

    public List<CuvantInvatat> CuvinteInvatateUltimaSesiune => _cuvinteInvatate;

    public void SetCuvinteInvatate(List<CuvantInvatat> cuvinte)
    {
        _cuvinteInvatate = cuvinte;
    }

    public bool EsteAutentificat => _utilizator != null;
    public bool EsteAdmin => _utilizator?.Rol == RolUtilizator.Admin;
    public UtilizatorDto? UtilizatorCurent => _utilizator;
    public int? SesiuneCurenta => _sesiune;
    public int CuvinteNoiSesiune => _cuvinteNoi;
    public int RevizuiriSesiune => _revizuiri;

    // ─── ConfigSesiune cu lazy load din Preferences ───
    public ConfigSesiuneDto ConfigSesiune
    {
        get
        {
            // Cache în memorie ca să nu citim Preferences de fiecare dată
            if (_configCache != null) return _configCache;

            _configCache = IncarcaConfigDinPreferences();
            return _configCache;
        }
    }

    public void SetConfigSesiune(ConfigSesiuneDto config)
    {
        _configCache = config;
        SalveazaConfigInPreferences(config);
    }

    private static ConfigSesiuneDto IncarcaConfigDinPreferences()
    {
        var config = new ConfigSesiuneDto();

        try
        {
            // Niveluri — salvate ca string CSV ("1,3,4")
            string nivStr = Preferences.Default.Get(KEY_NIVELURI, "");
            if (!string.IsNullOrWhiteSpace(nivStr))
            {
                config.Niveluri = nivStr
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out int v) ? v : 0)
                    .Where(v => v > 0)
                    .Select(v => (NivelCuvant)v)
                    .ToList();
            }

            // Domenii
            string domStr = Preferences.Default.Get(KEY_DOMENII, "");
            if (!string.IsNullOrWhiteSpace(domStr))
            {
                config.Domenii = domStr
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out int v) ? v : -1)
                    .Where(v => v >= 0)
                    .Select(v => (DomeniuCuvant)v)
                    .ToList();
            }

            // Mod
            int modInt = Preferences.Default.Get(KEY_MOD, 0);
            config.Mod = (ModInvatare)modInt;

            // Nr carduri
            config.NrCarduri = Preferences.Default.Get(KEY_NR_CARDURI, 10);
            // Defensive: dacă cineva a salvat o valoare invalidă, clamp
            config.NrCarduri = Math.Clamp(config.NrCarduri, 1, 30);
        }
        catch
        {
            // Orice eroare → returnăm default curat
            return new ConfigSesiuneDto();
        }

        return config;
    }

    private static void SalveazaConfigInPreferences(ConfigSesiuneDto config)
    {
        try
        {
            // Niveluri ca CSV
            string nivStr = string.Join(",", config.Niveluri.Select(n => (int)n));
            Preferences.Default.Set(KEY_NIVELURI, nivStr);

            // Domenii ca CSV
            string domStr = string.Join(",", config.Domenii.Select(d => (int)d));
            Preferences.Default.Set(KEY_DOMENII, domStr);

            Preferences.Default.Set(KEY_MOD, (int)config.Mod);
            Preferences.Default.Set(KEY_NR_CARDURI, config.NrCarduri);
        }
        catch
        {
            // Silent fail — nu vrem să crape app-ul pentru o salvare de preferințe
        }
    }

    public void SetUtilizator(UtilizatorDto u) => _utilizator = u;
    public void SetSesiune(int id) => _sesiune = id;
    public void SetCuvinteNoi(int nr) => _cuvinteNoi = nr;
    public void SetRevizuiriSesiune(int nr) => _revizuiri = nr;

    public void Deconecteaza()
    {
        _utilizator = null;
        _sesiune = null;
        _cuvinteNoi = 10;
        _revizuiri = 100;
        // NU resetăm _configCache — preferințele de filtrare se păstrează 
        // chiar și după deconectare (sunt utile la următoarea logare)
        LaDeconectare?.Invoke();
    }
    private List<CuvantIncomplet> _cuvinteIncomplete = new();
    public List<CuvantIncomplet> CuvinteIncompletUltimaSesiune => _cuvinteIncomplete;
    public void SetCuvinteIncomplete(List<CuvantIncomplet> cuvinte) => _cuvinteIncomplete = cuvinte;
}