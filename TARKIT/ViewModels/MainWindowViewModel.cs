namespace TARKIT.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _mapOverlayHotKeyDisplay = "[Not Bound]";

    public MapViewModel MapViewModel { get; }
    public QuestTrackerViewModel QuestTrackerViewModel { get; }
    public ItemsTrackerViewModel ItemsTrackerViewModel { get; }
    
    public string MapOverlayHotKeyDisplay
    {
        get => _mapOverlayHotKeyDisplay;
        set => SetProperty(ref _mapOverlayHotKeyDisplay, value);
    }

    public MainWindowViewModel()
    {
        MapViewModel = new MapViewModel();
        QuestTrackerViewModel = new QuestTrackerViewModel();
        ItemsTrackerViewModel = new ItemsTrackerViewModel();
    }

    public void SetMapOverlayHotKey(string displayText)
    {
        MapOverlayHotKeyDisplay = displayText;
    }
}
