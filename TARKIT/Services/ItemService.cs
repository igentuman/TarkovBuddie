using System.IO;
using System.Text.Json;
using TARKIT.Models;

namespace TARKIT.Services;

public class ItemService
{
    private readonly SettingsService _settingsService;
    private readonly string _itemProgressPath;

    public ItemService()
    {
        _settingsService = SettingsService.Instance;
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TARKIT"
        );
        _itemProgressPath = Path.Combine(appDataPath, "item_progress.json");
        EnsureProgressDirectory();
    }

    private void EnsureProgressDirectory()
    {
        var directory = Path.GetDirectoryName(_itemProgressPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public List<Item> LoadItems()
    {
        var items = new List<Item>();
        var itemsJsonPath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
            "data",
            "items_grouped.json"
        );

        if (!File.Exists(itemsJsonPath))
            return items;

        try
        {
            var json = File.ReadAllText(itemsJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                int itemId = 0;
                foreach (var element in root.EnumerateArray())
                {
                    var name = "";
                    if (element.TryGetProperty("name", out var nameProperty))
                    {
                        name = nameProperty.GetString() ?? string.Empty;
                    }

                    var shortName = "";
                    if (element.TryGetProperty("name_short", out var shortNameProperty))
                    {
                        shortName = shortNameProperty.GetString() ?? string.Empty;
                    }

                    var qty = 1;
                    if (element.TryGetProperty("total_qty", out var qtyProperty))
                    {
                        qty = qtyProperty.GetInt32();
                    }

                    var iconId = "";
                    if (element.TryGetProperty("icon", out var iconProperty))
                    {
                        iconId = iconProperty.GetString() ?? string.Empty;
                    }

                    var item = new Item
                    {
                        Id = itemId.ToString(),
                        NameEn = name,
                        NameRu = name,
                        NameShort = shortName,
                        RequiredQuantity = qty,
                        IconId = iconId,
                        Category = "Hideout",
                        CurrentQuantity = 0,
                        IsPinned = false
                    };
                    items.Add(item);
                    itemId++;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading items: {ex.Message}");
        }

        return items;
    }

    public void SaveItemProgress(ItemTracker tracker)
    {
        try
        {
            EnsureProgressDirectory();
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(tracker, options);
            File.WriteAllText(_itemProgressPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving item progress: {ex.Message}");
        }
    }

    public ItemTracker LoadItemProgress()
    {
        var tracker = new ItemTracker();

        if (!File.Exists(_itemProgressPath))
            return tracker;

        try
        {
            var json = File.ReadAllText(_itemProgressPath);
            var loadedTracker = JsonSerializer.Deserialize<ItemTracker>(json);
            if (loadedTracker != null)
            {
                tracker = loadedTracker;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading item progress: {ex.Message}");
        }

        return tracker;
    }
}
