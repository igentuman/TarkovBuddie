---
description: Stage 2 - Settings Tab Implementation
alwaysApply: false
---

# Stage 2: Settings Tab Implementation

**Objective**: Implement the Settings tab with hotkey configuration, language selection, and overlay transparency controls.

## Key Features to Implement

### 1. Hotkey Configuration
- Bind/unbind hotkeys for 5 actions:
  - Toggle map overlay
  - Toggle quest tracker window
  - Toggle items tracker window
  - Toggle pinned quests overlay
  - Toggle pinned items overlay
- Global hotkey listener (Windows API)
- Display current hotkey bindings
- Button to assign/clear each hotkey

### 2. Language Selection
- Dropdown with English / Russian options
- Runtime language switching (update all bound text)
- Persist selected language to settings file

### 3. Overlay Settings
- Slider for transparency (0-100%)
- Real-time preview (if applicable)
- Persist transparency value

### 4. Data File Paths
- Text inputs for:
  - Quest data JSON directory
  - Item data JSON directory
  - User progress storage directory
- Browse buttons to select directories
- Validation for directory paths

## Implementation Details

### SettingsViewModel
```csharp
public class SettingsViewModel : ViewModelBase
{
    private string _selectedLanguage = "English";
    private int _overlayTransparency = 100;
    private string _questDataPath = "";
    private string _itemDataPath = "";
    private string _userDataPath = "";
    
    public string SelectedLanguage 
    { 
        get => _selectedLanguage;
        set { _selectedLanguage = value; OnPropertyChanged(); }
    }
    
    public int OverlayTransparency 
    { 
        get => _overlayTransparency;
        set { _overlayTransparency = value; OnPropertyChanged(); }
    }
    
    // Properties for file paths
    // Commands for hotkey binding
    // Commands for browse buttons
}
```

### SettingsService
- **Load Settings**: Load from `%AppData%/TarkovBuddie/settings.json`
- **Save Settings**: Save all settings to JSON file
- **Register Hotkeys**: Windows API calls for global hotkey registration
- **Unregister Hotkeys**: Clean up hotkey registrations
- **Validate Paths**: Check if directories exist and are accessible

### Hotkey Registration Pattern
Use `RegisterHotKey` / `UnregisterHotKey` from Windows API:
```csharp
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

[DllImport("user32.dll")]
private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
```

### Settings Data Model
```csharp
public class ApplicationSettings
{
    public string Language { get; set; } = "English";
    public int OverlayTransparency { get; set; } = 100;
    public string QuestDataPath { get; set; } = "";
    public string ItemDataPath { get; set; } = "";
    public string UserDataPath { get; set; } = "";
    public Dictionary<string, HotKeyBinding> HotKeys { get; set; } = new();
}

public class HotKeyBinding
{
    public string Action { get; set; }
    public uint VirtualKey { get; set; }
    public uint Modifiers { get; set; }
}
```

### SettingsView.xaml Structure
```xaml
<Grid>
    <TabControl>
        <!-- Hotkeys Tab -->
        <TabItem Header="Hotkeys">
            <Grid>
                <!-- Rows for each hotkey with TextBox + Bind/Clear buttons -->
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <!-- Repeat for each hotkey -->
                </Grid.RowDefinitions>
            </Grid>
        </TabItem>
        
        <!-- Language Tab -->
        <TabItem Header="Language">
            <StackPanel>
                <Label>Language:</Label>
                <ComboBox ItemsSource="{Binding AvailableLanguages}" 
                          SelectedItem="{Binding SelectedLanguage}" />
            </StackPanel>
        </TabItem>
        
        <!-- Overlay Tab -->
        <TabItem Header="Overlay">
            <StackPanel>
                <Label>Transparency:</Label>
                <Slider Minimum="0" Maximum="100" 
                        Value="{Binding OverlayTransparency}" />
                <TextBlock Text="{Binding OverlayTransparency, StringFormat='{0}%'}" />
            </StackPanel>
        </TabItem>
        
        <!-- File Paths Tab -->
        <TabItem Header="File Paths">
            <Grid>
                <!-- Text inputs with browse buttons for each path -->
            </Grid>
        </TabItem>
    </TabControl>
</Grid>
```

## Tasks Checklist

### Code Implementation
- [ ] Update `SettingsViewModel` with all properties
- [ ] Create `HotKeyService` with Windows API calls
- [ ] Update `SettingsService` for persistence
- [ ] Create `ApplicationSettings` data model
- [ ] Create `HotKeyBinding` data model

### UI Implementation (SettingsView.xaml)
- [ ] Create 4-tab TabControl
- [ ] Hotkeys tab with 5 hotkey rows
- [ ] Language selection tab with ComboBox
- [ ] Overlay transparency tab with Slider
- [ ] File paths tab with TextBox + Browse button pairs

### Command Bindings
- [ ] `BindHotKeyCommand` - Listen for key press and assign
- [ ] `ClearHotKeyCommand` - Remove hotkey binding
- [ ] `BrowseQuestDataCommand` - Folder browser dialog
- [ ] `BrowseItemDataCommand` - Folder browser dialog
- [ ] `BrowseUserDataCommand` - Folder browser dialog
- [ ] `SaveSettingsCommand` - Persist all settings

### Integration
- [ ] Wire up SettingsViewModel to MainWindowViewModel
- [ ] Load default settings on application start
- [ ] Apply language changes immediately across all tabs
- [ ] Validate all file paths before saving

---

## Implementation Guidelines

### Hotkey Binding Flow
1. User clicks "Bind" button for a hotkey
2. HotKeyService enters listening mode
3. User presses key combination
4. Service captures the key and modifiers
5. Display the captured key in the UI
6. Store in memory (persist when user clicks Save)

### Language Switching
- Store language setting in AppSettings
- On language change, raise PropertyChanged events
- Bound UI elements automatically update

### File Path Validation
- Check if directory exists
- Check if user has read/write permissions
- Show error message if invalid
- Don't allow save if paths are invalid

### Settings Persistence
- Save to JSON file in AppData directory
- Create directory if it doesn't exist
- Load on application startup
- Apply cached settings to services

---

## Testing & Verification

- [ ] All hotkeys can be bound and unbound
- [ ] Language dropdown changes UI text
- [ ] Transparency slider value persists
- [ ] File path browse dialogs work correctly
- [ ] Settings load on application start
- [ ] Settings save correctly to JSON
- [ ] Hotkeys work globally (when game is focused)
- [ ] No console errors or warnings

