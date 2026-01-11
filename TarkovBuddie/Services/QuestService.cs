using System.IO;
using System.Text.Json;
using TarkovBuddie.Models;

namespace TarkovBuddie.Services;

public class QuestService
{
    private readonly SettingsService _settingsService;
    private readonly string _questProgressPath;

    public QuestService()
    {
        _settingsService = SettingsService.Instance;
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TarkovBuddie"
        );
        _questProgressPath = Path.Combine(appDataPath, "quest_progress.json");
        EnsureProgressDirectory();
    }

    private void EnsureProgressDirectory()
    {
        var directory = Path.GetDirectoryName(_questProgressPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public List<Quest> LoadQuests()
    {
        var quests = new List<Quest>();
        var questsJsonPath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
            "data",
            "raw_quests.json"
        );

        if (!File.Exists(questsJsonPath))
            return quests;

        try
        {
            var json = File.ReadAllText(questsJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in root.EnumerateArray())
                {
                    LoadQuestsRecursive(element, quests);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading quests: {ex.Message}");
        }

        return quests;
    }

    private void LoadQuestsRecursive(JsonElement element, List<Quest> quests)
    {
        var quest = ParseQuestFromJson(element);
        if (quest != null)
        {
            quests.Add(quest);
        }

        if (element.TryGetProperty("children_quests", out var childrenProperty) && 
            childrenProperty.ValueKind == JsonValueKind.Array)
        {
            foreach (var childElement in childrenProperty.EnumerateArray())
            {
                LoadQuestsRecursive(childElement, quests);
            }
        }
    }

    private Quest? ParseQuestFromJson(JsonElement element)
    {
        var id = "";
        if (element.TryGetProperty("id", out var idProperty))
        {
            id = idProperty.GetInt32().ToString();
        }

        var name = "";
        if (element.TryGetProperty("name", out var nameProperty))
        {
            name = nameProperty.GetString() ?? string.Empty;
        }

        var shortDescription = "";
        if (element.TryGetProperty("short_description", out var descProperty))
        {
            shortDescription = descProperty.GetString() ?? string.Empty;
        }

        var kappa = false;
        if (element.TryGetProperty("kappa", out var kappaProperty))
        {
            kappa = kappaProperty.GetBoolean();
        }

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            return null;

        return new Quest
        {
            Id = id,
            Name = name,
            ShortDescription = shortDescription,
            Kappa = kappa,
            IsCompleted = false,
            IsPinned = false
        };
    }

    public void SaveQuestProgress(QuestTracker tracker)
    {
        try
        {
            var data = new
            {
                completedQuestIds = tracker.CompletedQuestIds,
                pinnedQuestIds = tracker.PinnedQuestIds
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_questProgressPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving quest progress: {ex.Message}");
        }
    }

    public QuestTracker LoadQuestProgress()
    {
        var tracker = new QuestTracker();

        if (!File.Exists(_questProgressPath))
            return tracker;

        try
        {
            var json = File.ReadAllText(_questProgressPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("completedQuestIds", out var completedProperty) &&
                completedProperty.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in completedProperty.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.String)
                    {
                        tracker.CompletedQuestIds.Add(element.GetString() ?? "");
                    }
                }
            }

            if (root.TryGetProperty("pinnedQuestIds", out var pinnedProperty) &&
                pinnedProperty.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in pinnedProperty.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.String)
                    {
                        tracker.PinnedQuestIds.Add(element.GetString() ?? "");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading quest progress: {ex.Message}");
        }

        return tracker;
    }
}
