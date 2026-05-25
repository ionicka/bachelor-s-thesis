using FlashCards.ViewModels;

namespace FlashCards.Views;

[QueryProperty(nameof(Streak), "streak")]
[QueryProperty(nameof(NrCorect), "corect")]
[QueryProperty(nameof(NrGresit), "gresit")]
[QueryProperty(nameof(PrimaAZilei), "prima")]
public partial class FelicitariPage : ContentPage
{
    public int Streak { get; set; }
    public int NrCorect { get; set; }
    public int NrGresit { get; set; }
    public bool PrimaAZilei { get; set; }

    public FelicitariPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Construim VM-ul după ce query params au sosit
        BindingContext = new FelicitariViewModel(
            Streak, NrCorect, NrGresit, PrimaAZilei);
    }

    private async void OnContinuaClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//MainPage");

    private async void OnPracticaClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//SesiuneConfigPage");
}