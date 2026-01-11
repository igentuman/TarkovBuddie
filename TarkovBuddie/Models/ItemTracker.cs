namespace TarkovBuddie.Models;

public class ItemTracker
{
    public Dictionary<string, int> ItemQuantities { get; set; } = new();
    public List<string> PinnedItemIds { get; set; } = new();
}
