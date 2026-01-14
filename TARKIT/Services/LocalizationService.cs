using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TARKIT.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private static LocalizationService? _instance;
    private static readonly object _lock = new();
    
    private string _currentLanguage = "English";
    private Dictionary<string, Dictionary<string, string>> _translations = new();
    private readonly SettingsService _settingsService;
    private readonly string _dataPath;

    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly Dictionary<string, string> LanguageToCode = new()
    {
        { "English", "en" },
        { "Russian", "ru" }
    };

    public static LocalizationService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new LocalizationService();
                }
            }
            return _instance;
        }
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            System.Diagnostics.Debug.WriteLine($"CurrentLanguage setter: {_currentLanguage} -> {value}");
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                LoadLanguageData(value);
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine($"CurrentLanguage changed, PropertyChanged fired");
            }
        }
    }

    private LocalizationService()
    {
        _settingsService = SettingsService.Instance;
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        System.Diagnostics.Debug.WriteLine($"LocalizationService: BaseDirectory = {AppDomain.CurrentDomain.BaseDirectory}");
        System.Diagnostics.Debug.WriteLine($"LocalizationService: DataPath = {_dataPath}");
        System.Diagnostics.Debug.WriteLine($"LocalizationService: Data folder exists = {Directory.Exists(_dataPath)}");
        _currentLanguage = _settingsService.GetLanguage();
        System.Diagnostics.Debug.WriteLine($"LocalizationService: Current language = {_currentLanguage}");
        LoadLanguageData(_currentLanguage);
    }

    private void LoadLanguageData(string language)
    {
        if (!LanguageToCode.TryGetValue(language, out var langCode))
            langCode = "en";

        var langPath = Path.Combine(_dataPath, langCode);
        _translations.Clear();

        try
        {
            System.Diagnostics.Debug.WriteLine($"Loading language '{language}' from path: {langPath}");
            
            if (!Directory.Exists(langPath))
            {
                System.Diagnostics.Debug.WriteLine($"Language path not found: {langPath}");
                return;
            }

            var localeFiles = new HashSet<string>
            {
                "errors.json",
                "settings.json",
                "ui.json"
            };

            var files = Directory.EnumerateFiles(langPath, "*.json")
                .Where(f => localeFiles.Contains(Path.GetFileName(f)))
                .ToArray();
            System.Diagnostics.Debug.WriteLine($"Found {files.Length} translation files");
            
            foreach (var file in files)
            {
                var category = Path.GetFileNameWithoutExtension(file);
                var json = File.ReadAllText(file);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
                _translations[category] = data;
                System.Diagnostics.Debug.WriteLine($"Loaded category '{category}' with {data.Count} entries");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading translations: {ex.Message}");
        }
    }

    public string Translate(string category, string key, string? fallback = null)
    {
        try
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(key))
            {
                return fallback ?? $"[{category}.{key}]";
            }

            if (_translations.TryGetValue(category, out var categoryDict) &&
                categoryDict.TryGetValue(key, out var value))
            {
                return value;
            }

            System.Diagnostics.Debug.WriteLine($"Translation not found: {category}.{key}");
            return fallback ?? $"[{category}.{key}]";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Translation error for {category}.{key}: {ex.Message}");
            return fallback ?? $"[{category}.{key}]";
        }
    }

    public void RegisterLanguage(string name, string code)
    {
        if (!LanguageToCode.ContainsKey(name))
        {
            LanguageToCode[name] = code;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
