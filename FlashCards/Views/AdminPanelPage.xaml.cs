using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class AdminPanelPage : ContentPage
{
    private readonly AdminPanelViewModel _vm;

    public AdminPanelPage(AdminPanelViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        // Hookuim PropertyChanged pentru a comuta vizibilitatea "lista goală"
        _vm.Cuvinte.CollectionChanged += (s, e) =>
        {
            MesajGol.IsVisible = _vm.Cuvinte.Count == 0 && !_vm.SeIncarca;
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.IncarcaAsync();
        MesajGol.IsVisible = _vm.Cuvinte.Count == 0 && !_vm.SeIncarca;
    }

    private void OnPickerTipChanged(object sender, EventArgs e)
    {
        if (sender is Picker p && p.SelectedIndex >= 0)
            _vm.SeteazaFiltruTip(p.SelectedIndex);
    }

    private void OnPickerDomeniuChanged(object sender, EventArgs e)
    {
        if (sender is Picker p && p.SelectedIndex >= 0)
            _vm.SeteazaFiltruDomeniu(p.SelectedIndex);
    }
}