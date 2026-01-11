namespace TarkovBuddie.Models;

public class Quest
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public bool Kappa { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPinned { get; set; }
}
