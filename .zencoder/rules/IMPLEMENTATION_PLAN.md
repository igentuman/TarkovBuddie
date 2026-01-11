---
description: TarkovBuddie Implementation Plan
alwaysApply: true
---

# TarkovBuddie Implementation Plan

A structured approach to building the offline Escape from Tarkov companion application with map overlays, quest tracking, and item management.

## Overview

This implementation plan breaks down the TarkovBuddie project into 5 stages, starting with core structure and progressively adding features:

1. **Initial Structure** - Set up application foundation with UI tabs and empty component classes
2. **Settings Tab** - Implement hotkey configuration, language selection, and overlay settings
3. **Map Overlay** - Build interactive map viewer with zoom, pan, and marker management
4. **Items Tracker** - Create item tracking interface for quest and hideout items
5. **Quest Tracker** - Implement trader and storyline quest management

---

## Stage 1: Initial Structure & Tabs

**Objective**: Establish the foundational application structure with a 4-tab interface and empty service/model classes.

### Key Tasks

- Create directory structure for services, models, and UI components
- Build MainWindow with TabControl containing 4 tabs (Map, Quest Tracker, Items Tracker, Settings)
- Create empty ViewModels and Services classes
- Set up basic App.xaml with application resources and theme

### Deliverables

- **UI Structure**
  - MainWindow.xaml with TabControl layout (800×450 base size)
  - Tab headers: Map, Quest Tracker, Items Tracker, Settings
  - Basic styling and Grid layout for content

- **Directory Structure**
  ```
  TarkovBuddie/
  ├── Models/
  │   ├── Quest.cs
  │   ├── Item.cs
  │   ├── QuestTracker.cs
  │   └── ItemTracker.cs
  ├── Services/
  │   ├── HotKeyService.cs
  │   ├── SettingsService.cs
  │   ├── QuestService.cs
  │   ├── ItemService.cs
  │   └── MapService.cs
  ├── ViewModels/
  │   ├── MainWindowViewModel.cs
  │   ├── SettingsViewModel.cs
  │   ├── MapViewModel.cs
  │   ├── QuestTrackerViewModel.cs
  │   └── ItemsTrackerViewModel.cs
  ├── Views/
  │   ├── SettingsView.xaml
  │   ├── MapView.xaml
  │   ├── QuestTrackerView.xaml
  │   └── ItemsTrackerView.xaml
  └── App.xaml
  ```

- **Empty Classes** (with basic constructors and property stubs)

---

## Stage 2: Settings Tab

**Objective**: Implement the Settings tab with hotkey configuration, language selection, and overlay transparency controls.

### Key Features

- **Hotkey Configuration**
  - Bind/unbind hotkeys for:
    - Toggle map overlay (default: unassigned)
    - Toggle quest tracker window (default: unassigned)
    - Toggle items tracker window (default: unassigned)
    - Toggle pinned quests overlay (default: unassigned)
    - Toggle pinned items overlay (default: unassigned)
  - Global hotkey listener (works when game is focused)
  - Display current hotkey bindings

- **Language Selection**
  - Dropdown: English / Russian
  - Runtime language switching

- **Overlay Settings**
  - Slider for overlay transparency (0-100%)
  - Preview of overlay appearance

- **Data File Paths**
  - Path inputs for:
    - Quest data JSON files
    - Item data JSON files
    - User progress storage

### Deliverables

- **SettingsView.xaml**
  - TabControl or GroupBox layout
  - Hotkey assignment UI with text boxes and buttons
  - Language dropdown
  - Transparency slider
  - File path inputs with browse buttons

- **SettingsViewModel**
  - INotifyPropertyChanged implementation
  - Hotkey binding/unbinding logic
  - Language selection handling
  - Settings persistence (load/save)

- **SettingsService**
  - Load/save settings to local storage
  - Hotkey registration/unregistration
  - Settings validation

---

## Stage 3: Map Overlay

**Objective**: Build an interactive map viewer with zoom, pan, and marker management capabilities.

### Key Interactions

- **Scroll Wheel**: Zoom in/out (with centered zoom)
- **Mouse 1 (Left Click)**: Drag map to pan
- **Mouse 3 (Right Click/Scroll Click)**: Add marker at click position
- **Double Click on Marker**: Remove marker (or context menu option)

### Viewport State Management

- **Remember State**: Zoom level, X position, Y position for each map
- **Persist**: Save viewport state to local storage
- **Load**: Restore viewport state when switching maps

### Key Features

- **Map Selection**
  - Dropdown or button grid with all map names
  - Map change updates both preview and overlay

- **Map Preview**
  - Display selected map WebP image
  - Show current zoom level and position

- **Marker Management**
  - Add markers with right-click
  - Remove markers with double-click or context menu
  - Visual representation (circles, pins, etc.)
  - Markers stored per map

- **Zoom Controls**
  - Scroll wheel with centered zoom
  - Optional buttons for zoom in/out
  - Zoom limits (min/max)

- **Dragging**
  - Left-click drag to pan across map
  - Smooth dragging experience
  - Boundary checking to prevent off-map dragging

### Deliverables

- **MapView.xaml**
  - Map selection dropdown/grid
  - Canvas or Image control with pan/zoom support
  - Marker visual elements
  - Zoom level display

- **MapViewModel**
  - Map selection and preview
  - Zoom/pan state management
  - Marker collection binding
  - Viewport state persistence

- **MapService**
  - Load map images from `maps/` directory
  - Manage marker data (add, remove, get by map)
  - Calculate zoom and pan coordinates
  - Save/load viewport state per map

---

## Stage 4: Items Tracker

**Objective**: Create an item tracking interface for quest and hideout items.

### Key Features

- **Item Grid View**
  - Display items in grid format
  - Show columns:
    - Item name (localized)
    - Required quantity
    - Current quantity
    - Category (Trader Quest / Storyline Quest / Hideout)

- **Quantity Controls**
  - +/- buttons to increment/decrement current quantity
  - Manual input for quantity
  - Visual feedback when item complete (required ≤ current)

- **Pin/Unpin**
  - Pin items to overlay for visibility during raids
  - Unpinned items still tracked but not visible on overlay
  - Pin status persisted

- **Filtering** (optional for Stage 4)
  - Filter by category
  - Filter by completion status
  - Search by item name

### Data Model

- **Item Structure**
  ```csharp
  public class Item
  {
      public string Id { get; set; }
      public string NameRu { get; set; }
      public string NameEn { get; set; }
      public int RequiredQuantity { get; set; }
      public ItemCategory Category { get; set; }
      public string RelatedQuestId { get; set; }
  }
  ```

### Deliverables

- **ItemsTrackerView.xaml**
  - DataGrid or ListBox with item display
  - +/- buttons for quantity
  - Pin/unpin toggle buttons
  - Category and search filters

- **ItemsTrackerViewModel**
  - Item collection binding
  - Quantity update handling
  - Pin/unpin logic
  - Filtering logic

- **ItemService**
  - Load items from JSON
  - Save/load item quantities (user progress)
  - Manage pinned items

---

## Stage 5: Quest Tracker

**Objective**: Implement trader and storyline quest management with independent progression tracking.

### Key Features

- **Trader Quests Section**
  - TreeView structure: Trader → Quest Chain → Individual Quest
  - Show quest details:
    - Quest name (localized)
    - Trader name
    - Required map(s)
    - Completion status

- **Storyline Quests Section**
  - Separate linear view of storyline quests
  - Show story chapter/stage
  - Dependencies on previous quests
  - Completion tracking

- **Completion Toggles**
  - Checkbox to mark quest complete
  - Completion state persisted
  - Visual indication (greyed out, strikethrough, etc.)

- **Kappa Progress**
  - Display count of Kappa-required trader quests completed
  - Total Kappa-required quests
  - Progress percentage

- **Pin/Unpin**
  - Pin quests to overlay for raid reference
  - Pinned quests shown on map overlay
  - Filtered by current selected map

### Data Models

- **Trader Quest**
  ```csharp
  public class TraderQuest
  {
      public string Id { get; set; }
      public string NameRu { get; set; }
      public string NameEn { get; set; }
      public string Trader { get; set; }
      public string[] RequiredMaps { get; set; }
      public bool RequiredForKappa { get; set; }
      public string[] ChainedQuestIds { get; set; }
  }
  ```

- **Storyline Quest**
  ```csharp
  public class StorylineQuest
  {
      public string Id { get; set; }
      public string NameRu { get; set; }
      public string NameEn { get; set; }
      public int Chapter { get; set; }
      public string[] RequiredMaps { get; set; }
      public string[] DependsOnQuestIds { get; set; }
  }
  ```

### Deliverables

- **QuestTrackerView.xaml**
  - Two-section layout (Trader Quests / Storyline Quests)
  - TreeView for trader quests
  - ListView/ListBox for storyline quests
  - Checkbox for quest completion
  - Pin/unpin buttons
  - Kappa progress display

- **QuestTrackerViewModel**
  - Trader quest tree data binding
  - Storyline quest list binding
  - Completion state management
  - Pin/unpin logic
  - Kappa calculation

- **QuestService**
  - Load trader and storyline quests from JSON
  - Save/load quest completion state
  - Calculate Kappa progress
  - Manage pinned quests

---

## Implementation Notes

### Architecture Decisions

1. **MVVM Pattern**: Use ViewModels with INotifyPropertyChanged for UI binding
2. **Service Layer**: Separate business logic from UI
3. **JSON Data**: Load quest and item data from JSON files (not embedded)
4. **Local Storage**: Use JSON files for user progress (quests completed, item quantities, etc.)
5. **Hotkeys**: Use Windows API for global hotkey registration

### Code Conventions

- Use C# file-scoped namespaces (`namespace TarkovBuddie;`)
- Nullable reference types enabled (`#nullable enable`)
- Use auto-properties where appropriate
- Follow WPF naming conventions (ViewModel, View, xaml.cs)

### Dependencies

- No external NuGet packages required (uses built-in .NET/WPF)
- Uses System.Configuration for settings
- Uses System.Windows for WPF
- Windows API calls for hotkeys and window management

### Testing Strategy

- Manual testing of each stage
- Data validation with sample JSON files
- Hotkey functionality verification
- UI responsiveness and layout testing

---

## Timeline & Milestones

| Stage | Tasks | Est. Complexity |
|-------|-------|-----------------|
| 1 | Structure, tabs, empty classes | Low |
| 2 | Settings UI, hotkey binding, language | Medium |
| 3 | Map viewer, zoom, pan, markers | High |
| 4 | Item grid, quantity tracking | Medium |
| 5 | Quest tree, storyline view, completion | Medium-High |

---

## Future Enhancements

- Overlay windows for pinned quests and items
- Automatic map detection via game memory (if required)
- Quest-based map markers overlay
- Profiles per wipe
- Optional cloud sync
- Advanced filtering and search
- Custom UI themes
