using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class StatisticiPage : ContentPage
{
    private readonly StatisticiViewModel _vm;
    public StatisticiPage(StatisticiViewModel vm)
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
