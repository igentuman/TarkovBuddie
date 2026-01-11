namespace TarkovBuddie.Services;

public class MapService
{
    public MapService()
    {
    }

    public List<string> GetAvailableMaps()
    {
        return new List<string>();
    }

    public void SaveMapMarkers(string mapName, List<(int x, int y)> markers)
    {
    }

    public List<(int x, int y)> LoadMapMarkers(string mapName)
    {
        return new List<(int x, int y)>();
    }

    public void SaveViewportState(string mapName, int zoom, int x, int y)
    {
    }

    public (int zoom, int x, int y) LoadViewportState(string mapName)
    {
        return (100, 0, 0);
    }
}
