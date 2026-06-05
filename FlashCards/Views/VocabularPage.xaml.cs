using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class VocabularPage : ContentPage
{
    private readonly VocabularViewModel _vm;

    public VocabularPage(VocabularViewModel vm)
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