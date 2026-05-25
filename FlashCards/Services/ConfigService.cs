using System.Text.Json;

namespace FlashCards.Services;

public class AppConfig
{
    public DatabaseConfig Database { get; set; } = new();
}

public class DatabaseConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Name { get; set; } = "FlashCards";
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = "";

    public string ToConnectionString() =>
        $"Host={Host};Port={Port};Database={Name};Username={Username};Password={Password}";
}

public static class ConfigLoader
{
    public static AppConfig Incarca()
    {
        try
        {
            // FileSystem.OpenAppPackageFileAsync nu e neapărat async pe toate platformele
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            var config = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
            {
                System.Diagnostics.Debug.WriteLine("[CONFIG] Deserialize returned null, using defaults");
                return new AppConfig { Database = new DatabaseConfig { Password = "postgres" } };
            }

            System.Diagnostics.Debug.WriteLine($"[CONFIG] Loaded: Host={config.Database.Host}, DB={config.Database.Name}");
            return config;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CONFIG] EROARE incarcare: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[CONFIG] Stack: {ex.StackTrace}");
            // Fallback la valori hardcodate ca să nu crape complet
            return new AppConfig
            {
                Database = new DatabaseConfig
                {
                    Host = "localhost",
                    Port = 5432,
                    Name = "FlashCards",
                    Username = "postgres",
                    Password = "postgres"
                }
            };
        }
    }
}