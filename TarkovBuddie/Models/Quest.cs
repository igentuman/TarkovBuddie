namespace TarkovBuddie.Models;

public class Quest
{
    public string Id { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
