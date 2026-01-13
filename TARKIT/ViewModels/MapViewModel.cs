using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TARKIT.Models;
using TARKIT.Services;

namespace TARKIT.ViewModels;

public class MapViewModel : ViewModelBase
{
    private readonly MapService _mapService;
    private readonly SettingsService _settingsService;
    private string _selectedMap = "";
    private BitmapImage? _currentMapImage;
    private double _mapWidth = 4096;
    private double _mapHeight = 4096;
    private double _zoomLevel = 1.0;
    private double _panX = 0.0;
    private double _panY = 0.0;

    public ObservableCollection<string> AvailableMaps { get; }
    
    public string SelectedMap
    {
        get => _selectedMap;
        set
        {
            if (SetProperty(ref _selectedMap, value))
            {
                LoadMapImage();
                LoadViewportState();
                SaveSelectedMap();
            }
        }
    }

    public BitmapImage? CurrentMapImage
    {
        get => _currentMapImage;
        set => SetProperty(ref _currentMapImage, value);
    }

    public double MapWidth
    {
        get => _mapWidth;
        set => SetProperty(ref _mapWidth, value);
    }

    public double MapHeight
    {
        get => _mapHeight;
        set => SetProperty(ref _mapHeight, value);
    }

    public double ZoomLevel
    {
        get => _zoomLevel;
        set => SetProperty(ref _zoomLevel, value);
    }

    public double PanX
    {
        get => _panX;
        set => SetProperty(ref _panX, value);
    }

    public double PanY
    {
        get => _panY;
        set => SetProperty(ref _panY, value);
    }

    public MapViewModel()
    {
        _mapService = new MapService();
        _settingsService = SettingsService.Instance;
        AvailableMaps = new ObservableCollection<string>();
        
        LoadAvailableMaps();
    }

    private void LoadAvailableMaps()
    {
        var maps = _mapService.GetAvailableMaps();
        foreach (var map in maps)
        {
            AvailableMaps.Add(map);
        }

        var savedMap = _settingsService.GetCurrentSettings().SelectedMap;
        if (!string.IsNullOrEmpty(savedMap) && AvailableMaps.Contains(savedMap))
        {
            SelectedMap = savedMap;
        }
        else if (AvailableMaps.Count > 0)
        {
            SelectedMap = AvailableMaps[0];
        }
    }

    private void LoadMapImage()
    {
        if (string.IsNullOrEmpty(SelectedMap))
        {
            CurrentMapImage = null;
            return;
        }

        var image = _mapService.LoadMapImage(SelectedMap);
        CurrentMapImage = image;
        
        if (image != null)
        {
            MapWidth = image.PixelWidth;
            MapHeight = image.PixelHeight;
        }
    }

    private void LoadViewportState()
    {
        var settings = _settingsService.GetCurrentSettings();
        if (!string.IsNullOrEmpty(SelectedMap) && settings.MapViewportStates.TryGetValue(SelectedMap, out var state))
        {
            ZoomLevel = state.ZoomLevel;
            PanX = state.PanX;
            PanY = state.PanY;
        }
        else
        {
            ZoomLevel = 1.0;
            PanX = 0.0;
            PanY = 0.0;
        }
    }

    public void SaveViewportState()
    {
        if (string.IsNullOrEmpty(SelectedMap))
            return;

        var settings = _settingsService.GetCurrentSettings();
        settings.MapViewportStates[SelectedMap] = new MapViewportState
        {
            ZoomLevel = ZoomLevel,
            PanX = PanX,
            PanY = PanY
        };
        _settingsService.SaveSettings(settings);
    }

    private void SaveSelectedMap()
    {
        var settings = _settingsService.GetCurrentSettings();
        settings.SelectedMap = _selectedMap;
        _settingsService.SaveSettings(settings);
    }
}
