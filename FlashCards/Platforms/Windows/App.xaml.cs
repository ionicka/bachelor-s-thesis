using Microsoft.UI.Xaml;

namespace FlashCards.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var mauiWindow = Microsoft.Maui.Controls.Application.Current?.Windows[0];
        if (mauiWindow?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                    WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow)));

            // Dimensiune telefon
            appWindow.Resize(new Windows.Graphics.SizeInt32(430, 920));

            // Centreaza pe ecran
            var display = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(
                appWindow.Id,
                Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);

            int x = (display.WorkArea.Width - 430) / 2;
            int y = (display.WorkArea.Height - 920) / 2;
            appWindow.Move(new Windows.Graphics.PointInt32(x, Math.Max(0, y)));
        }
    }
}