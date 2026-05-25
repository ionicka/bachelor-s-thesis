using FlashCards.ViewModels;

namespace FlashCards.Views;

[QueryProperty(nameof(IdCuvant), "id")]
public partial class EditeazaCuvantPage : ContentPage
{
    private readonly EditeazaCuvantViewModel _vm;

    public string? IdCuvant { get; set; }

    public EditeazaCuvantPage(EditeazaCuvantViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        _vm.Imagini.CollectionChanged += (s, e) =>
          PlaceholderImagini.IsVisible = _vm.Imagini.Count == 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(IdCuvant) && int.TryParse(IdCuvant, out int id))
        {
            await _vm.IncarcaModEditAsync(id);
        }
        else
        {
            _vm.IncarcaModAdaugare();
        }

        // Reset după citire ca să nu rămână stale la următoare deschidere
        IdCuvant = null;
    }
}