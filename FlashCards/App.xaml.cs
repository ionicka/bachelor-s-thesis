using FlashCards.Services;
using FlashCards.Views;

namespace FlashCards;

public partial class App : Application
{
    private readonly ISessionStateService _session;

    public App(ISessionStateService session)
    {
        InitializeComponent();
        _session = session;
        Routing.RegisterRoute(nameof(InregistrarePage), typeof(InregistrarePage));
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        if (_session.EsteAutentificat)
            await Shell.Current.GoToAsync("//MainPage");
        // Daca nu e autentificat ramane pe LoginPage (primul ShellContent)
    }
}