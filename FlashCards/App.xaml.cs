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

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

#if WINDOWS
    var displayInfo = DeviceDisplay.MainDisplayInfo;
    double densityFactor = displayInfo.Density;
    double ecranWidth = displayInfo.Width / densityFactor;
    double ecranHeight = displayInfo.Height / densityFactor;

    // 85% din înălțimea ecranului, dar max 850px
    int inaltime = (int)Math.Min(850, ecranHeight * 0.85);
    // Raport mobil 9:19.5 — împarte la 2.17
    int latime = (int)(inaltime / 2.17);
    // Clamp lățimea între 360-440
    latime = Math.Clamp(latime, 360, 440);

    window.Width = latime;
    window.Height = inaltime;
    window.MinimumWidth = latime;
    window.MinimumHeight = inaltime;
    window.MaximumWidth = latime;
    window.MaximumHeight = inaltime;

    window.Created += (s, e) =>
    {
        window.X = (ecranWidth - latime) / 2;
        window.Y = (ecranHeight - inaltime) / 2;
    };
#endif

        return window;
    }

    protected override async void OnStart()
    {
        base.OnStart();
        if (_session.EsteAutentificat)
            await Shell.Current.GoToAsync("//MainPage");
    }
}