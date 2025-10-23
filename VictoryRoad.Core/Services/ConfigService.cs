using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public class ConfigService : IConfigService
{
    private readonly string _configDirectory;
    private readonly string _configFilePath;

    public ConfigService()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _configDirectory = Path.Combine(homeDirectory, ".victory-road");
        _configFilePath = Path.Combine(_configDirectory, "config.ini");
    }

    public UserConfig LoadUserConfig()
    {
        try
        {
            if (!File.Exists(_configFilePath))
                return new UserConfig();

            var content = File.ReadAllText(_configFilePath);
            var settings = ParseIni(content);

            return new UserConfig
            {
                PlayerName = settings.GetValueOrDefault("PlayerName", string.Empty),
                PlayerId = settings.GetValueOrDefault("PlayerId", string.Empty),
                BirthDate = ParseBirthDate(settings.GetValueOrDefault("BirthDate", string.Empty))
            };
        }
        catch
        {
            return new UserConfig();
        }
    }

    public void SaveUserConfig(UserConfig config)
    {
        try
        {
            EnsureDirectoryExists();
            var content = BuildIni(config);
            File.WriteAllText(_configFilePath, content);
        }
        catch
        {
        }
    }

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_configDirectory))
            Directory.CreateDirectory(_configDirectory);
    }

    private Dictionary<string, string> ParseIni(string content)
    {
        var result = new Dictionary<string, string>();
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("[") || string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            var parts = trimmedLine.Split('=', 2);
            if (parts.Length == 2)
            {
                result[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return result;
    }

    private string BuildIni(UserConfig config)
    {
        var lines = new List<string>
        {
            "[UserSettings]",
            $"PlayerName={config.PlayerName}",
            $"PlayerId={config.PlayerId}",
            $"BirthDate={FormatBirthDate(config.BirthDate)}"
        };

        return string.Join(Environment.NewLine, lines);
    }

    private DateTime? ParseBirthDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParse(value, out var date))
            return date;

        return null;
    }

    private string FormatBirthDate(DateTime? birthDate)
    {
        return birthDate?.ToString("yyyy-MM-dd") ?? string.Empty;
    }
}
