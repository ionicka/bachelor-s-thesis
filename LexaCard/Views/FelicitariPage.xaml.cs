using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class FelicitariPage : ContentPage
{
    public FelicitariPage(FelicitariViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnContinuaClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        await Shell.Current.GoToAsync("//MainPage");
    }
}
