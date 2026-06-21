namespace FlashCards;

public static class ApiConfig
{
    // ─── UNICUL loc unde se schimbă IP-ul când se schimbă rețeaua ───
    private const string ServerIp = "192.168.100.26";
    private const string Port = "5202";

#if ANDROID
    public static readonly string ApiBaseUrl = $"http://{ServerIp}:{Port}/";
#else
    public static readonly string ApiBaseUrl = $"http://localhost:{Port}/";
#endif

    public static string GetImageUrl(string numeFisier) =>
        $"{ApiBaseUrl}imagini/{numeFisier}";
}