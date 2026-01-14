using System.Collections.Generic;

namespace TARKIT.Models;

public class MapViewportState
{
    public double ZoomLevel { get; set; } = 1.0;
    public double PanX { get; set; } = 0.0;
    public double PanY { get; set; } = 0.0;
}

public class ApplicationSettings
{
    public string Language { get; set; } = "English";
    public int OverlayTransparency { get; set; } = 100;
    public string QuestDataPath { get; set; } = "";
    public string ItemDataPath { get; set; } = "";
    public string UserDataPath { get; set; } = "";
    public string SelectedMap { get; set; } = "";
    public Dictionary<string, HotKeyBinding> HotKeys { get; set; } = new();
    public Dictionary<string, MapViewportState> MapViewportStates { get; set; } = new();
    public bool EnableExitTracker { get; set; } = false;
}
