using FlashCards.ViewModels;

namespace FlashCards.Views;

[QueryProperty(nameof(IdCuvant), "id")]
public partial class EditeazaCuvantPage : ContentPage
{
    private readonly EditeazaCuvantViewModel _vm;
    private bool _incarcat = false;

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

        // Evită reîncărcarea la fiecare revenire pe pagină
        if (_incarcat) return;

        // Forțează un mic delay ca binding-ul să fie complet propagat
        await Task.Delay(50);

        System.Diagnostics.Debug.WriteLine($"[EDIT] OnAppearing, IdCuvant='{IdCuvant}'");

        if (!string.IsNullOrEmpty(IdCuvant) && int.TryParse(IdCuvant, out int id))
        {
            System.Diagnostics.Debug.WriteLine($"[EDIT] Loading mod edit cu id={id}");
            await _vm.IncarcaModEditAsync(id);
            System.Diagnostics.Debug.WriteLine($"[EDIT] Dupa load: Termen='{_vm.Termen}', Exemple={_vm.Exemple.Count}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[EDIT] Loading mod adaugare");
            _vm.IncarcaModAdaugare();
        }

        PlaceholderImagini.IsVisible = _vm.Imagini.Count == 0;
        _incarcat = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Reset pentru următoarea deschidere
        _incarcat = false;
        IdCuvant = null;
    }
}