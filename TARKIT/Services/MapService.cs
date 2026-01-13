using System.IO;
using System.Windows.Media.Imaging;

namespace TARKIT.Services;

public class MapService
{
    private readonly string _mapsDirectory;
    private readonly Dictionary<string, BitmapImage> _mapCache = new();

    public MapService()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _mapsDirectory = Path.Combine(baseDirectory, "maps");
        
        if (!Directory.Exists(_mapsDirectory))
        {
            var projectDirectory = Path.Combine(baseDirectory, "..", "..", "..", "maps");
            _mapsDirectory = Path.GetFullPath(projectDirectory);
        }
    }

    public List<string> GetAvailableMaps()
    {
        if (!Directory.Exists(_mapsDirectory))
            return new List<string>();

        var maps = Directory.GetFiles(_mapsDirectory, "*.webp")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .OrderBy(m => m)
            .ToList();

        return maps;
    }

    public BitmapImage? LoadMapImage(string mapName)
    {
        if (string.IsNullOrEmpty(mapName))
            return null;

        if (_mapCache.TryGetValue(mapName, out var cached))
            return cached;

        var mapPath = Path.Combine(_mapsDirectory, $"{mapName}.webp");
        
        if (!File.Exists(mapPath))
            return null;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(mapPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            _mapCache[mapName] = bitmap;
            return bitmap;
        }
        catch
        {
            return null;
        }
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
