using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using FlashCards.Services;
using System.Collections.ObjectModel;

namespace FlashCards.ViewModels;

public partial class VocabularViewModel : ObservableObject
{
    private readonly ICardService _cardService;
    private readonly ISessionStateService _session;

    private List<CardDto> _toateCuvintele = new();

    [ObservableProperty] ObservableCollection<CardDto> _cuvinteFiltrate = new();
    [ObservableProperty] bool _seIncarca = false;
    [ObservableProperty] bool _aratDoarIgnorate = false;
    [ObservableProperty] string _textCautare = string.Empty;
    [ObservableProperty] int _indexNivel = 0; // 0=toate, 1=elementar, 2=intermediar, 3=avansat
    [ObservableProperty] int _indexCategorie = 0;
    partial void OnIndexCategorieChanged(int value) => Filtreaza();
    public VocabularViewModel(
      ICardService cardService,
      ISessionStateService session)
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
            _toateCuvintele = await _cardService.GetToateCuvinteleAsync(
                _session.UtilizatorCurent.Id);
            Filtreaza();
        }
        finally { SeIncarca = false; }
    }

    partial void OnTextCautareChanged(string value) => Filtreaza();
    partial void OnAratDoarIgnorateChanged(bool value) => Filtreaza();
    partial void OnIndexNivelChanged(int value) => Filtreaza();

    private void Filtreaza()
    {
        var rezultat = _toateCuvintele.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TextCautare))
            rezultat = rezultat.Where(c =>
                c.Termen.Contains(TextCautare, StringComparison.OrdinalIgnoreCase) ||
                c.Definitie.Contains(TextCautare, StringComparison.OrdinalIgnoreCase));

        // Filtru nivel
        if (IndexNivel > 0)
            rezultat = rezultat.Where(c => c.Nivel == (NivelCuvant)IndexNivel);

        // Filtru categorie — 10 = ignorate, altfel domeniu
        if (IndexCategorie == 11)
            rezultat = rezultat.Where(c => c.EsteIgnorat);
         
        else if (IndexCategorie > 0)
            rezultat = rezultat.Where(c =>
                !c.EsteIgnorat && c.Domeniu == (DomeniuCuvant)(IndexCategorie - 1));
        else
            rezultat = rezultat.Where(c => !c.EsteIgnorat);

        CuvinteFiltrate = new ObservableCollection<CardDto>(rezultat);
    }

    [RelayCommand]
    async Task ScoateIgnorareAsync(CardDto card)
    {
        if (_session.UtilizatorCurent == null) return;
        try
        {
            await _cardService.ScoateIgnorareAsync(
                _session.UtilizatorCurent.Id, card.CuvantId);
            await IncarcaAsync();
        }
        catch { }
    }

    [RelayCommand]
    async Task InapoiAsync() => await Shell.Current.GoToAsync("//SesiuneConfigPage");
    [RelayCommand]
    void ToggleIgnorate()
    {
        AratDoarIgnorate = !AratDoarIgnorate;
    }
}