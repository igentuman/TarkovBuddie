namespace TarkovBuddie.Models;

public class QuestTracker
{
    public List<string> CompletedQuestIds { get; set; } = new();
    public List<string> PinnedQuestIds { get; set; } = new();
}
