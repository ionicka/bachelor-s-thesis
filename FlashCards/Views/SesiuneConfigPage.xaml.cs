using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class SesiuneConfigPage : ContentPage
{
    private readonly SesiuneConfigViewModel _vm;

    public SesiuneConfigPage(SesiuneConfigViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.IncarcaAsync();
    }
}