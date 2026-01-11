namespace TarkovBuddie.Services;

public class SettingsService
{
    public SettingsService()
    {
    }

    public void SaveSettings(Dictionary<string, object> settings)
    {
    }

    public Dictionary<string, object> LoadSettings()
    {
        return new Dictionary<string, object>();
    }

    public void SetLanguage(string language)
    {
    }

    public string GetLanguage()
    {
        return "en";
    }
}
