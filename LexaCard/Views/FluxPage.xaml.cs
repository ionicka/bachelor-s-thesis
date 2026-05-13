using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class FluxPage : ContentPage
{
    private readonly FluxViewModel _vm;

    public FluxPage(FluxViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeazaAsync();
    }
}
