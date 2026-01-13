namespace TARKIT.Models;

public class Item
{
    public string Id { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameShort { get; set; } = string.Empty;
    public int RequiredQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public string RelatedQuestId { get; set; } = string.Empty;
    public string IconId { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
}
