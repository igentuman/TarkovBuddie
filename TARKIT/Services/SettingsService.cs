using System.IO;
using System.Text.Json;
using TARKIT.Models;

namespace TARKIT.Services;

public class SettingsService
{
    private static SettingsService? _instance;
    private static readonly object _lock = new();

    private readonly string _settingsDirectory;
    private readonly string _settingsFilePath;
    private ApplicationSettings _currentSettings;

    public event Action<ApplicationSettings>? SettingsChanged;

    public static SettingsService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SettingsService();
                }
            }
            return _instance;
        }
    }

    private SettingsService()
    {
        _settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TARKIT");
        _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
        _currentSettings = new ApplicationSettings();
        
        EnsureSettingsDirectory();
    }

    private void EnsureSettingsDirectory()
    {
        if (!Directory.Exists(_settingsDirectory))
        {
            Directory.CreateDirectory(_settingsDirectory);
        }
    }

    public ApplicationSettings LoadSettings()
    {
        if (!File.Exists(_settingsFilePath))
        {
            _currentSettings = new ApplicationSettings();
            return _currentSettings;
        }

        try
        {
            string json = File.ReadAllText(_settingsFilePath);
            _currentSettings = JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings();
        }
        catch
        {
            _currentSettings = new ApplicationSettings();
        }

        return _currentSettings;
    }

    public void SaveSettings(ApplicationSettings settings)
    {
        try
        {
            EnsureSettingsDirectory();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_settingsFilePath, json);
            _currentSettings = settings;
            SettingsChanged?.Invoke(settings);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public ApplicationSettings GetCurrentSettings()
    {
        return _currentSettings;
    }

    public void SetLanguage(string language)
    {
        _currentSettings.Language = language;
        SaveSettings(_currentSettings);
    }

    public string GetLanguage()
    {
        return _currentSettings.Language;
    }

    public bool ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        try
        {
            return Directory.Exists(path);
        }
        catch
        {
            return false;
        }
    }
}
