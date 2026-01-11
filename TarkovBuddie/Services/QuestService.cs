using TarkovBuddie.Models;

namespace TarkovBuddie.Services;

public class QuestService
{
    public QuestService()
    {
    }

    public List<Quest> LoadTraderQuests()
    {
        return new List<Quest>();
    }

    public List<Quest> LoadStorylineQuests()
    {
        return new List<Quest>();
    }

    public void SaveQuestProgress(QuestTracker tracker)
    {
    }

    public QuestTracker LoadQuestProgress()
    {
        return new QuestTracker();
    }
}
