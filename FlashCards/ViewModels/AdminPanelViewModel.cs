using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Helpers;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using System.Collections.ObjectModel;

namespace FlashCards.ViewModels;

public partial class AdminPanelViewModel : ObservableObject
{
    private readonly IAdminService _adminService;
    private readonly ISessionStateService _session;
    private readonly IImageStorageService _imageStorage;

    [ObservableProperty] ObservableCollection<CuvantListItemVm> _cuvinte = new();
    [ObservableProperty] bool _seIncarca = false;
    [ObservableProperty] int _nrTotal = 0;
    [ObservableProperty] string _cautareText = string.Empty;

    // Filtre — null = "toate"
    [ObservableProperty] TipCuvant? _filtruTip;
    [ObservableProperty] DomeniuCuvant? _filtruDomeniu;
    [ObservableProperty] string _labelFiltruTip = "Toate tipurile";
    [ObservableProperty] string _labelFiltruDomeniu = "Toate domeniile";

    // Liste pentru Picker-uri
    public List<string> TipuriOptiuni { get; }
    public List<string> DomeniiOptiuni { get; }

    public AdminPanelViewModel(
        IAdminService adminService,
        ISessionStateService session,
        IImageStorageService imageStorage)
    {
        _adminService = adminService;
        _session = session;
        _imageStorage = imageStorage;

        // Pregătim opțiunile pentru Picker (cu "Toate" pe prima poziție)
        TipuriOptiuni = new List<string> { "Toate tipurile" };
        TipuriOptiuni.AddRange(EnumLabels.ToateTipurile.Select(EnumLabels.Label));

        DomeniiOptiuni = new List<string> { "Toate domeniile" };
        DomeniiOptiuni.AddRange(EnumLabels.ToateDomenii.Select(EnumLabels.Label));
    }

    public async Task IncarcaAsync()
    {
        SeIncarca = true;
        try
        {
            var filtru = new FiltruCuvinteDto
            {
                Cautare = string.IsNullOrWhiteSpace(CautareText) ? null : CautareText,
                Tip = FiltruTip,
                Domeniu = FiltruDomeniu
            };

            var lista = await _adminService.GetCuvinteAsync(filtru);
            NrTotal = await _adminService.GetNrTotalCuvinteAsync();

            Cuvinte.Clear();
            foreach (var c in lista)
                Cuvinte.Add(new CuvantListItemVm(c));
        }
        finally { SeIncarca = false; }
    }

    // Apelat automat când userul tastează în search box
    partial void OnCautareTextChanged(string value) => _ = IncarcaAsync();

    // Apelat când userul selectează altceva din Picker
    public void SeteazaFiltruTip(int index)
    {
        // index 0 = "Toate", restul = enum values
        FiltruTip = index == 0 ? null : EnumLabels.ToateTipurile[index - 1];
        LabelFiltruTip = TipuriOptiuni[index];
        _ = IncarcaAsync();
    }

    public void SeteazaFiltruDomeniu(int index)
    {
        FiltruDomeniu = index == 0 ? null : EnumLabels.ToateDomenii[index - 1];
        LabelFiltruDomeniu = DomeniiOptiuni[index];
        _ = IncarcaAsync();
    }

    [RelayCommand]
    void ReseteazaFiltre()
    {
        CautareText = string.Empty;
        FiltruTip = null;
        FiltruDomeniu = null;
        LabelFiltruTip = "Toate tipurile";
        LabelFiltruDomeniu = "Toate domeniile";
        _ = IncarcaAsync();
    }

    [RelayCommand]
    async Task AdaugaCuvantNou() =>
        await Shell.Current.GoToAsync("//EditeazaCuvantPage");

    [RelayCommand]
    async Task EditeazaCuvant(CuvantListItemVm item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"//EditeazaCuvantPage?id={item.Id}");
    }

    [RelayCommand]
    async Task StergeCuvant(CuvantListItemVm item)
    {
        if (item == null) return;

        bool ok = await Application.Current!.MainPage!.DisplayAlert(
            "Confirmă ștergerea",
            $"Sigur vrei să ștergi cuvântul '{item.Termen}'?\n\n" +
            "Această acțiune este IREVERSIBILĂ și va șterge și tot progresul " +
            "utilizatorilor pe acest cuvânt.",
            "Da, șterge",
            "Anulează");

        if (!ok) return;

        var rezultat = await _adminService.StergeCuvantAsync(item.Id);
        if (rezultat.Succes)
        {
            // Șterge și imaginile asociate din storage local
            foreach (var img in item.Imagini)
                _imageStorage.Sterge(img);

            Cuvinte.Remove(item);
            NrTotal--;

            await Application.Current!.MainPage!.DisplayAlert(
                "Șters",
                "Cuvântul a fost șters cu succes.",
                "OK");
        }
        else
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Eroare",
                rezultat.Mesaj ?? "Nu s-a putut șterge cuvântul.",
                "OK");
        }
    }

    [RelayCommand]
    async Task InapoiLaMain() =>
        await Shell.Current.GoToAsync("//MainPage");
}

// ═══════════════════════════════════════════════════════════════
// ViewModel per item în listă — wrapper peste CuvantListaDto
// cu metadate pre-calculate pentru UI (labels, iconițe, culori)
// ═══════════════════════════════════════════════════════════════
public class CuvantListItemVm
{
    public int Id { get; }
    public string Termen { get; }
    public string Definitie { get; }
    public string TipLabel { get; }
    public string DomeniuLabel { get; }
    public string TipIcon { get; }
    public string DomeniuIcon { get; }
    public string DomeniuCuloare { get; }
    public int NrExemple { get; }
    public int NrImagini { get; }
    public string MetaInfo { get; }

    // Necesar pentru ștergerea imaginilor la delete
    public List<string> Imagini { get; }

    public CuvantListItemVm(CuvantListaDto dto)
    {
        Id = dto.Id;
        Termen = dto.Termen;
        Definitie = dto.Definitie.Length > 80
            ? dto.Definitie[..80] + "..."
            : dto.Definitie;
        TipLabel = EnumLabels.Label(dto.Tip);
        DomeniuLabel = EnumLabels.Label(dto.Domeniu);
        TipIcon = EnumLabels.Icon(dto.Tip);
        DomeniuIcon = EnumLabels.Icon(dto.Domeniu);
        DomeniuCuloare = CuloareDomeniu(dto.Domeniu);
        NrExemple = dto.NrExemple;
        NrImagini = dto.NrImagini;
        MetaInfo = $"{NrExemple} ex • {NrImagini} img • {EnumLabels.Label(dto.Nivel)}";
        Imagini = new List<string>(); // populat doar la nevoie
    }

    private static string CuloareDomeniu(DomeniuCuvant d) => d switch
    {
        DomeniuCuvant.General => "#556688",
        DomeniuCuvant.Business => "#FFD700",
        DomeniuCuvant.Tehnologie => "#4A90E2",
        DomeniuCuvant.Sanatate => "#E94560",
        DomeniuCuvant.Educatie => "#7B2FBE",
        DomeniuCuvant.Cultura => "#FF8C00",
        DomeniuCuvant.Sport => "#4CAF50",
        DomeniuCuvant.Politica => "#8E44AD",
        DomeniuCuvant.Calatorii => "#16A085",
  
        _ => "#556688"
    };
}