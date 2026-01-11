using TarkovBuddie.Models;

namespace TarkovBuddie.Services;

public class ItemService
{
    public ItemService()
    {
    }

    public List<Item> LoadItems()
    {
        return new List<Item>();
    }

    public void SaveItemProgress(ItemTracker tracker)
    {
    }

    public ItemTracker LoadItemProgress()
    {
        return new ItemTracker();
    }
}
