using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Helpers;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using System.Collections.ObjectModel;

namespace FlashCards.ViewModels;

public partial class EditeazaCuvantViewModel : ObservableObject
{
    private readonly IAdminService _adminService;
    private readonly IImageStorageService _imageStorage;

    // Modul curent — adăugare sau editare
    [ObservableProperty] bool _esteModEdit = false;
    [ObservableProperty] string _titluPagina = "Adaugă cuvânt nou";
    [ObservableProperty] string _textButonSalvare = "Salvează";
    private int? _idCurent;

    // Câmpurile de bază
    [ObservableProperty] string _termen = string.Empty;
    [ObservableProperty] string _definitie = string.Empty;
    [ObservableProperty] string _definitieRo = string.Empty;
    [ObservableProperty] string _pronuntie = string.Empty;
    [ObservableProperty] string _etichete = string.Empty;

    // Selectori — folosim string în UI (Picker.SelectedItem) și convertim la enum
    [ObservableProperty] int _selectedTipIndex = 0;
    [ObservableProperty] int _selectedDomeniuIndex = 0;
    [ObservableProperty] int _selectedNivelIndex = 0;

    // Exemple — listă mutabilă cu wrapper observable
    [ObservableProperty] ObservableCollection<ExempluVm> _exemple = new();

    // Imagini
    [ObservableProperty] ObservableCollection<ImagineVm> _imagini = new();
    [ObservableProperty] bool _poateAdaugaImagine = true;

    // State UI
    [ObservableProperty] bool _seSalveaza = false;
    [ObservableProperty] string _mesajEroare = string.Empty;

    // Opțiunile pentru Picker-uri
    public List<string> TipuriOptiuni { get; }
    public List<string> DomeniiOptiuni { get; }
    public List<string> NivelOptiuni { get; }

    public EditeazaCuvantViewModel(
        IAdminService adminService,
        IImageStorageService imageStorage)
    {
        _adminService = adminService;
        _imageStorage = imageStorage;

        TipuriOptiuni = EnumLabels.ToateTipurile.Select(EnumLabels.Label).ToList();
        DomeniiOptiuni = EnumLabels.ToateDomenii.Select(EnumLabels.Label).ToList();
        NivelOptiuni = EnumLabels.ToateNivelele.Select(EnumLabels.Label).ToList();

        // Pornește cu 2 exemple goale pentru adăugare
        ResetExemple();
    }

    // ═════════════════════════════════════════════════════════════
    // Mod ADĂUGARE — apelat când userul vine cu butonul +
    // ═════════════════════════════════════════════════════════════
    public void IncarcaModAdaugare()
    {
        _idCurent = null;
        EsteModEdit = false;
        TitluPagina = "Adaugă cuvânt nou";
        TextButonSalvare = "Adaugă cuvântul";

        Termen = "";
        Definitie = "";
        DefinitieRo = "";
        Pronuntie = "";
        Etichete = "";
        SelectedTipIndex = 0;
        SelectedDomeniuIndex = 0;
        SelectedNivelIndex = 0;
        MesajEroare = "";

        ResetExemple();
        Imagini.Clear();
        ActualizeazaStareImagini();
    }

    // ═════════════════════════════════════════════════════════════
    // Mod EDITARE — apelat când userul vine cu ?id=N în URL
    // ═════════════════════════════════════════════════════════════
    public async Task IncarcaModEditAsync(int id)
    {
        var dto = await _adminService.GetCuvantPentruEditAsync(id);
        if (dto == null)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Eroare",
                $"Cuvântul cu Id={id} nu a fost găsit.",
                "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _idCurent = dto.Id;
        EsteModEdit = true;
        TitluPagina = $"Editează: {dto.Termen}";
        TextButonSalvare = "Salvează modificările";

        Termen = dto.Termen;
        Definitie = dto.Definitie;
        DefinitieRo = dto.DefinitieRo ?? "";
        Pronuntie = dto.Pronuntie ?? "";
        Etichete = dto.Etichete ?? "";

        // ─── Picker-e: setăm și forțăm notificare explicită ───
        int tipIdx = EnumLabels.ToateTipurile.IndexOf(dto.Tip);
        int domeniuIdx = EnumLabels.ToateDomenii.IndexOf(dto.Domeniu);
        int nivelIdx = EnumLabels.ToateNivelele.IndexOf(dto.Nivel);

        // Setăm temporar la -1 pentru a forța schimbarea
        SelectedTipIndex = -1;
        SelectedDomeniuIndex = -1;
        SelectedNivelIndex = -1;

        await Task.Delay(10); // mic respiro pentru Picker

        SelectedTipIndex = tipIdx >= 0 ? tipIdx : 0;
        SelectedDomeniuIndex = domeniuIdx >= 0 ? domeniuIdx : 0;
        SelectedNivelIndex = nivelIdx >= 0 ? nivelIdx : 0;

        // ─── Exemple ───
        Exemple.Clear();
        foreach (var ex in dto.Exemple)
            Exemple.Add(new ExempluVm(this) { Text = ex });
        if (Exemple.Count == 0)
            Exemple.Add(new ExempluVm(this));
        ActualizeazaStareExemple();

        // ─── Imagini ───
        Imagini.Clear();
        foreach (var img in dto.Imagini)
            Imagini.Add(new ImagineVm(this, _imageStorage) { NumeFisier = img });
        ActualizeazaStareImagini();

        MesajEroare = "";
    }

    private void ResetExemple()
    {
        Exemple.Clear();
        Exemple.Add(new ExempluVm(this));
        Exemple.Add(new ExempluVm(this));
        ActualizeazaStareExemple();
    }

    internal void ActualizeazaStareExemple()
    {
        // Permite +/- pe exemple (min 1, max 5)
        foreach (var ex in Exemple)
        {
            ex.PoateSterge = Exemple.Count > 1;
        }
    }

    internal void ActualizeazaStareImagini()
    {
        PoateAdaugaImagine = Imagini.Count < 2;
    }

    [RelayCommand]
    void AdaugaExemplu()
    {
        if (Exemple.Count >= 5) return;
        Exemple.Add(new ExempluVm(this));
        ActualizeazaStareExemple();
    }

    [RelayCommand]
    async Task AdaugaImagine()
    {
        int loc_ramas = 2 - Imagini.Count;
        if (loc_ramas <= 0) return;

        // Permite multi-select — admin alege 1 sau 2 imagini odată
        var numeFisiereSalvate = await _imageStorage.AlegeSiSalveazaMultipleAsync(loc_ramas);
        if (numeFisiereSalvate.Count == 0) return;

        foreach (var nume in numeFisiereSalvate)
        {
            Imagini.Add(new ImagineVm(this, _imageStorage) { NumeFisier = nume });
        }
        ActualizeazaStareImagini();
    }

    [RelayCommand]
    async Task Salveaza()
    {
        if (SeSalveaza) return;
        MesajEroare = "";

        // Construiește DTO din state-ul curent
        var dto = new CuvantEditDto
        {
            Id = _idCurent,
            Termen = Termen,
            Definitie = Definitie,
            DefinitieRo = string.IsNullOrWhiteSpace(DefinitieRo) ? null : DefinitieRo,
            Pronuntie = string.IsNullOrWhiteSpace(Pronuntie) ? null : Pronuntie,
            Etichete = string.IsNullOrWhiteSpace(Etichete) ? null : Etichete,
            Tip = EnumLabels.ToateTipurile[SelectedTipIndex],
            Domeniu = EnumLabels.ToateDomenii[SelectedDomeniuIndex],
            Nivel = EnumLabels.ToateNivelele[SelectedNivelIndex],
            Limba = "engleza",
            Exemple = Exemple.Select(e => e.Text).ToList(),
            Imagini = Imagini.Select(i => i.NumeFisier).ToList()
        };

        SeSalveaza = true;
        try
        {
            var rezultat = EsteModEdit
                ? await _adminService.ActualizeazaCuvantAsync(dto)
                : await _adminService.CreeazaCuvantAsync(dto);

            if (rezultat.Succes)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Succes",
                    rezultat.Mesaj ?? "Salvat.",
                    "OK");
                await Shell.Current.GoToAsync("//AdminPanelPage");
            }
            else
            {
                MesajEroare = rezultat.Mesaj ?? "Eroare necunoscută.";
            }
        }
        finally { SeSalveaza = false; }
    }

    [RelayCommand]
    async Task Anuleaza()
    {
        if (HasModificari())
        {
            bool ok = await Application.Current!.MainPage!.DisplayAlert(
                "Părăsește pagina?",
                "Ai modificări nesalvate. Sigur vrei să ieși?",
                "Da, ieși",
                "Rămâi");
            if (!ok) return;
        }

        // Dacă suntem în mod adăugare și am încărcat imagini, le ștergem
        // (sunt orfane în storage)
        if (!EsteModEdit)
        {
            foreach (var img in Imagini)
                _imageStorage.Sterge(img.NumeFisier);
        }

        await Shell.Current.GoToAsync("//AdminPanelPage");
    }

    private bool HasModificari()
    {
        if (EsteModEdit) return true; // în edit assume modificări
        return !string.IsNullOrWhiteSpace(Termen) ||
               !string.IsNullOrWhiteSpace(Definitie) ||
               Imagini.Count > 0;
    }

    // ═══════════════════════════════════════════════════════════════
    // Apelat din ExempluVm când userul apasă X pe un exemplu
    // ═══════════════════════════════════════════════════════════════
    internal void StergeExemplu(ExempluVm exemplu)
    {
        if (Exemple.Count <= 1) return;
        Exemple.Remove(exemplu);
        ActualizeazaStareExemple();
    }

    internal void StergeImagine(ImagineVm imagine)
    {
        // Șterge fizic doar dacă e o imagine nouă (în mod adăugare)
        // sau dacă confirma explicit (în mod editare)
        // Pentru simplitate: o ștergem fizic imediat
        _imageStorage.Sterge(imagine.NumeFisier);
        Imagini.Remove(imagine);
        ActualizeazaStareImagini();
    }
}

// ═══════════════════════════════════════════════════════════════
// Wrapper observable pentru un exemplu — permite TextChanged pe Entry
// ═══════════════════════════════════════════════════════════════
public partial class ExempluVm : ObservableObject
{
    private readonly EditeazaCuvantViewModel _parent;

    [ObservableProperty] string _text = string.Empty;
    [ObservableProperty] bool _poateSterge = true;

    public ExempluVm(EditeazaCuvantViewModel parent)
    {
        _parent = parent;
    }

    [RelayCommand]
    void Sterge() => _parent.StergeExemplu(this);
}

// ═══════════════════════════════════════════════════════════════
// Wrapper pentru o imagine — afișează preview + buton Șterge
// ═══════════════════════════════════════════════════════════════
public partial class ImagineVm : ObservableObject
{
    private readonly EditeazaCuvantViewModel _parent;
    private readonly IImageStorageService _imageStorage;

    [ObservableProperty] string _numeFisier = string.Empty;

    public string CaleAbsoluta => _imageStorage.GetCaleAbsoluta(NumeFisier);

    public ImagineVm(EditeazaCuvantViewModel parent, IImageStorageService imageStorage)
    {
        _parent = parent;
        _imageStorage = imageStorage;
    }

    partial void OnNumeFisierChanged(string value) =>
        OnPropertyChanged(nameof(CaleAbsoluta));

    [RelayCommand]
    void Sterge() => _parent.StergeImagine(this);
}