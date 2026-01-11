# TarkovBuddie Implementation Rules

This directory contains comprehensive implementation plans and Zencoder rules for building TarkovBuddie, an offline Escape from Tarkov companion application.

## Files Overview

### Main Documents

- **IMPLEMENTATION_PLAN.md** - High-level overview of all 5 stages with timeline, architecture decisions, and future enhancements
- **stage-1-initial-structure.md** - Setting up the project foundation with tabs and empty classes
- **stage-2-settings-tab.md** - Implementing hotkey configuration, language selection, and overlay settings
- **stage-3-map-overlay.md** - Building interactive map viewer with zoom, pan, and markers
- **stage-4-items-tracker.md** - Creating item tracking interface with quantities and filtering
- **stage-5-quest-tracker.md** - Implementing trader and storyline quest management

## Quick Start

1. **Read** `IMPLEMENTATION_PLAN.md` first to understand the overall architecture and approach
2. **Start Stage 1** by following `stage-1-initial-structure.md`
3. **Progress sequentially** through stages 2-5
4. Each stage file contains:
   - Detailed objectives and key features
   - Code structure and ViewModel examples
   - XAML layout templates
   - Service implementation patterns
   - Tasks checklist
   - Testing & verification guidelines

## Project Structure

The application is organized in a clean MVVM architecture:

```
TarkovBuddie/
├── Models/              # Data models (Quest, Item, etc.)
├── Services/            # Business logic (QuestService, MapService, etc.)
├── ViewModels/          # UI logic (MVVM pattern)
├── Views/               # XAML UI files (SettingsView, MapView, etc.)
├── maps/                # Map image assets (WebP)
└── App.xaml             # Application entry point
```

## Key Technologies

- **Framework**: .NET 9.0 Windows (C#)
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM pattern
- **Data**: JSON-based (no database)
- **Hotkeys**: Windows API for global hotkey registration
- **No external dependencies**: Uses only built-in .NET/WPF libraries

## Implementation Approach

### MVVM Pattern
- **Models**: Data structures (Quest, Item, Marker, etc.)
- **ViewModels**: UI logic with INotifyPropertyChanged
- **Views**: XAML UI files
- **Services**: Business logic and data access

### Data Persistence
- Settings: `%AppData%/TarkovBuddie/settings.json`
- User Progress: Separate JSON files for quests, items, markers
- Viewport State: `viewports.json` per map

### Code Style
- File-scoped namespaces: `namespace TarkovBuddie;`
- Nullable reference types enabled
- Auto-properties for data models
- Clear separation of concerns

## Stage Progression

| Stage | Focus | Complexity | Est. Time |
|-------|-------|-----------|-----------|
| 1 | Project structure, empty classes, tabs | Low | 1-2 hours |
| 2 | Settings, hotkeys, language, persistence | Medium | 2-3 hours |
| 3 | Map viewer, zoom, pan, markers | High | 4-6 hours |
| 4 | Item tracking, quantities, filtering | Medium | 2-3 hours |
| 5 | Quest tracking, trees, Kappa progress | Medium-High | 3-4 hours |

## Important Guidelines

### Before Starting Each Stage
1. Read the corresponding stage file completely
2. Review the code structure and templates provided
3. Check the tasks checklist
4. Understand the data models

### During Implementation
1. Follow the MVVM pattern consistently
2. Maintain separation of concerns
3. Use proper localization support (English/Russian)
4. Implement data persistence for user state
5. Add error handling for file I/O

### Testing Each Stage
1. Build without errors
2. Verify all UI elements render correctly
3. Test data persistence (save/load)
4. Check language switching works
5. Validate user interactions

## Data Models Reference

### Quest Models
- `TraderQuest` - Individual trader quest with Kappa flag
- `StorylineQuest` - Story progression quest with chapters
- `QuestProgressState` - Completion and pin state

### Item Models
- `Item` - Item definition with required quantity
- `ItemTrackerState` - Current quantity and pin state
- `ItemCategory` - Enum: TraderQuest, StorylineQuest, Hideout

### Map Models
- `Marker` - Custom map marker (position, map name)
- `ViewportState` - Zoom and pan state per map

### Settings Models
- `ApplicationSettings` - User preferences and paths
- `HotKeyBinding` - Hotkey action and key combination

## Common Patterns

### ViewModel Base Class
All ViewModels inherit from `ViewModelBase` with INotifyPropertyChanged support.

### Command Pattern
Use ICommand for all UI interactions (button clicks, checkboxes, etc.).

### Data Binding
Bind all dynamic content to ViewModels using {Binding} syntax.

### Localization
Always support both English and Russian:
```csharp
public string LocalizedName => AppSettings.CurrentLanguage == "Russian" ? NameRu : NameEn;
```

### JSON Persistence
Load/save all user data to JSON files using System.Text.Json.

## Troubleshooting

### Common Issues

**WebP Image Loading**
- Check if System.Drawing supports WebP on your .NET version
- If not, consider using ImageSharp NuGet package or pre-convert to PNG

**Global Hotkeys Not Working**
- Verify RegisterHotKey API calls are correct
- Check that window handle is valid
- Ensure modifiers (Ctrl, Shift, Alt) are properly encoded

**JSON Serialization Issues**
- Use `[JsonPropertyName]` attributes for property naming
- Handle null/default values properly
- Test with sample JSON files before deployment

## Future Enhancements

- Automatic map detection via game memory
- Quest-based map markers overlay
- Profiles per wipe
- Optional cloud sync
- Advanced filtering and search
- Custom UI themes
- Overlay windows for pinned items/quests

## References

- **Escape from Tarkov**: Official game
- **WPF Documentation**: Microsoft Docs
- **System.Text.Json**: JSON serialization
- **Windows API**: Hotkey registration

## Notes

- Application does NOT interact with game memory or network
- All operations are offline and local
- No telemetry or data collection
- Safe to use with any anti-cheat system

