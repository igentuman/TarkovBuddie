---
description: Stage 3 - Map Overlay Implementation
alwaysApply: false
---

# Stage 3: Map Overlay Implementation

**Objective**: Build an interactive map viewer with zoom, pan, and marker management capabilities.

## Key Interactions

### Input Handling
- **Scroll Wheel**: Zoom in/out (centered zoom around cursor position)
- **Mouse 1 (Left Click) + Drag**: Pan the map across the canvas
- **Mouse 3 (Right Click/Middle Click)**: Add marker at click position
- **Double Click on Marker**: Remove marker
- **Mouse Wheel Click (Middle)**: Reserved for future implementation

### Viewport State Management
- Store zoom level, X offset, Y offset per map
- Save to `%AppData%/TarkovBuddie/viewports.json`
- Load saved viewport when switching maps
- Persist viewport automatically on map switch or zoom/pan

## Implementation Details

### Map Selection
- Dropdown with all map names
- Button grid alternative (optional)
- Map names: woods, terminal, streets, shoreline, reserve, lighthouse, labrynth, lab, interchange, ground_zero, factory, customs

### Map Preview & Canvas
- Display selected map WebP image
- Canvas or Image control with zoom support
- Transformation matrix for zoom/pan calculations
- Visual feedback for zoom level

### Marker Management
- Collection of Marker objects per map
- Markers drawn as visual elements on canvas (circles, pins, etc.)
- Click position converted to map coordinate space
- Data structure:
  ```csharp
  public class Marker
  {
      public string Id { get; set; } = Guid.NewGuid().ToString();
      public double X { get; set; }
      public double Y { get; set; }
      public string MapName { get; set; }
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
  ```

### Zoom Controls
- Zoom limits: min 1.0x, max 10.0x
- Default zoom: 1.0x (fit to view or 100%)
- Smooth zoom with scroll wheel
- Visual zoom level indicator

### Dragging
- Track mouse position on left-click
- Calculate delta on mouse move
- Update canvas offset
- Release on mouse up
- Boundary checking to prevent off-map dragging

## Code Structure

### MapViewModel
```csharp
public class MapViewModel : ViewModelBase
{
    private string _selectedMap = "woods";
    private double _zoomLevel = 1.0;
    private double _offsetX = 0;
    private double _offsetY = 0;
    private ObservableCollection<Marker> _markers = new();
    
    public string SelectedMap 
    { 
        get => _selectedMap;
        set { _selectedMap = value; OnPropertyChanged(); LoadMapViewport(); }
    }
    
    public double ZoomLevel 
    { 
        get => _zoomLevel;
        set { _zoomLevel = Math.Clamp(value, 0.5, 10.0); OnPropertyChanged(); }
    }
    
    public double OffsetX 
    { 
        get => _offsetX;
        set { _offsetX = value; OnPropertyChanged(); }
    }
    
    public double OffsetY 
    { 
        get => _offsetY;
        set { _offsetY = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<Marker> Markers => _markers;
    
    public ICommand AddMarkerCommand { get; }
    public ICommand RemoveMarkerCommand { get; }
    public ICommand MapScrollCommand { get; }
    public ICommand MapDragCommand { get; }
}
```

### MapView.xaml Structure
```xaml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    
    <!-- Map Selection -->
    <ComboBox Grid.Row="0" ItemsSource="{Binding AvailableMaps}" 
              SelectedItem="{Binding SelectedMap}" />
    
    <!-- Canvas with markers -->
    <Canvas Grid.Row="1" 
            MouseWheel="OnMapMouseWheel"
            MouseMove="OnMapMouseMove"
            MouseUp="OnMapMouseUp"
            MouseDown="OnMapMouseDown"
            MouseRightButtonUp="OnMapRightClick">
        <Image Source="{Binding CurrentMapSource}" 
               Width="{Binding ImageWidth}"
               Height="{Binding ImageHeight}"
               RenderTransform="{Binding ViewportTransform}" />
        
        <!-- Markers ItemsControl -->
        <ItemsControl ItemsSource="{Binding Markers}">
            <ItemsControl.ItemsPanel>
                <ItemsPanelTemplate>
                    <Canvas />
                </ItemsPanelTemplate>
            </ItemsControl.ItemsPanel>
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <Ellipse Width="20" Height="20" Fill="Red" 
                             Canvas.Left="{Binding X}" 
                             Canvas.Top="{Binding Y}"
                             MouseDoubleClick="OnMarkerDoubleClick" />
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </Canvas>
    
    <!-- Zoom Level Display -->
    <TextBlock Grid.Row="2" 
               Text="{Binding ZoomLevel, StringFormat='Zoom: {0:P0}'}" />
</Grid>
```

### MapService
```csharp
public class MapService
{
    public BitmapImage LoadMapImage(string mapName)
    {
        // Load WebP from maps/ directory
        // Convert WebP to BitmapImage if needed
    }
    
    public void SaveViewportState(string mapName, ViewportState state)
    {
        // Save to viewports.json
    }
    
    public ViewportState? LoadViewportState(string mapName)
    {
        // Load from viewports.json
    }
    
    public void AddMarker(string mapName, Marker marker)
    {
        // Save marker to markers.json per map
    }
    
    public void RemoveMarker(string mapName, string markerId)
    {
        // Remove marker from markers.json
    }
    
    public List<Marker> LoadMarkers(string mapName)
    {
        // Load markers from markers.json
    }
}

public class ViewportState
{
    public double ZoomLevel { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public string MapName { get; set; }
}
```

## Tasks Checklist

### Core Map Functionality
- [ ] Load map image from WebP files (mapName.webp in maps/ directory)
- [ ] Create MapViewModel with zoom/pan properties
- [ ] Implement Canvas with Image and marker rendering

### Zoom & Pan
- [ ] Implement scroll wheel zoom (centered zoom)
- [ ] Implement left-click drag for panning
- [ ] Add zoom limits (0.5x - 10.0x or appropriate range)
- [ ] Add boundary checking for pan offset
- [ ] Apply RenderTransform to image based on zoom/offset

### Markers
- [ ] Right-click to add marker at cursor position
- [ ] Convert cursor position to map coordinate space
- [ ] Double-click marker to remove it
- [ ] Visual marker representation (circle/pin/etc.)
- [ ] Markers collection binding to ObservableCollection

### Viewport State
- [ ] Save viewport state when map changes
- [ ] Save viewport state on zoom/pan changes (debounced)
- [ ] Load viewport state when switching maps
- [ ] Create viewports.json data file

### Data Persistence
- [ ] Create MapService to load/save markers
- [ ] Create markers.json per map or global
- [ ] Load markers when map is selected
- [ ] Save markers when added/removed

---

## Implementation Guidelines

### Coordinate Space Conversion
When user clicks on canvas, convert screen coordinates to map coordinates:
```csharp
double mapX = (canvasX - offsetX) / zoomLevel;
double mapY = (canvasY - offsetY) / zoomLevel;
```

### Centered Zoom
When zooming with mouse wheel:
```csharp
double oldZoom = ZoomLevel;
ZoomLevel = newZoom;
OffsetX = mouseX - (mouseX - OffsetX) * (newZoom / oldZoom);
OffsetY = mouseY - (mouseY - OffsetY) * (newZoom / oldZoom);
```

### WebP Loading
Check if System.Drawing can handle WebP, or use a library. Consider:
- System.Drawing (may not support WebP on all .NET versions)
- ImageSharp library (if available in project)
- Fallback: convert WebP to PNG separately

### RenderTransform
Use ScaleTransform and TranslateTransform:
```xaml
<Image.RenderTransform>
    <TransformGroup>
        <ScaleTransform ScaleX="{Binding ZoomLevel}" ScaleY="{Binding ZoomLevel}" />
        <TranslateTransform X="{Binding OffsetX}" Y="{Binding OffsetY}" />
    </TransformGroup>
</Image.RenderTransform>
```

### Marker Persistence
Store markers per map:
```json
{
  "woods": [
    { "id": "...", "x": 100, "y": 150, "createdAt": "2024-01-01T00:00:00Z" }
  ],
  "customs": [...]
}
```

---

## Testing & Verification

- [ ] All 12 maps load and display correctly
- [ ] Scroll wheel zooms in/out smoothly
- [ ] Centered zoom keeps mouse position aligned
- [ ] Left-click drag pans the map smoothly
- [ ] Pan stays within boundary (no off-map dragging)
- [ ] Right-click adds visible marker
- [ ] Double-click marker removes it
- [ ] Viewport state saves on map change
- [ ] Viewport state loads on map switch
- [ ] Markers persist and load on application restart
- [ ] Zoom limits enforced (0.5x - 10.0x)
- [ ] No rendering lag with multiple markers

