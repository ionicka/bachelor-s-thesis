using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class FelicitariPage : ContentPage
{
    // Static ca sa putem trimite datele fara DI complex
    public static FelicitariViewModel? ViewModel { get; set; }

    public FelicitariPage()
    {
        InitializeComponent();
        if (ViewModel != null)
            BindingContext = ViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            BindingContext = ViewModel;
    }

    private async void OnContinuaClicked(object sender, EventArgs e)
    {
        ViewModel = null;
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnPracticaClicked(object sender, EventArgs e)
    {
        ViewModel = null;
        await Shell.Current.GoToAsync("//SesiuneConfigPage");
    }
}