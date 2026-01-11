---
description: Repository Information Overview
alwaysApply: true
---

# TarkovBuddie Information

## Summary

**Tarkov Buddie** is a lightweight Windows desktop application designed as an offline companion tool for Escape from Tarkov. It provides map overlays, quest tracking (both trader and storyline quests), and item tracking through transparent topmost windows with configurable hotkeys. The application does not interact with game memory or network traffic—it operates purely as an external helper tool.

## Structure

**Root Level:**
- `TarkovBuddie.sln` - Visual Studio solution file
- `TarkovBuddie/` - Main application project
  - `App.xaml` / `App.xaml.cs` - WPF application entry point
  - `MainWindow.xaml` / `MainWindow.xaml.cs` - Primary UI window (4-tab interface)
  - `maps/` - Map image assets (WebP format for all Tarkov maps)
  - `TarkovBuddie.csproj` - Project configuration

**Key Directories:**
- `maps/` - Contains WebP map images: woods, terminal, streets, shoreline, reserve, lighthouse, labrynth, lab, interchange, ground_zero, factory, customs

## Language & Runtime

**Language**: C#  
**Target Framework**: .NET 9.0 Windows (`net9.0-windows`)  
**UI Framework**: WPF (Windows Presentation Foundation)  
**Platform**: Windows only (Windows 7.0+)  
**Language Features**: 
- Nullable reference types enabled
- Implicit usings enabled

## Dependencies

**NuGet Dependencies**: None (project uses only built-in .NET and WPF libraries)

**Framework References**:
- `Microsoft.WindowsDesktop.App.WPF` - WPF rendering and controls
- `Microsoft.NETCore.App` - Core .NET runtime

**Built-in Libraries Used**:
- `System.Windows` - WPF UI framework
- `System.Windows.Controls` - WPF controls
- `System.Windows.Media.Imaging` - Image handling
- `System.Configuration` - Configuration management

## Build & Installation

**Build Command** (via dotnet):
```bash
dotnet build TarkovBuddie.sln
```

**Debug Build**:
```bash
dotnet build -c Debug TarkovBuddie.sln
```

**Release Build**:
```bash
dotnet build -c Release TarkovBuddie.sln
```

**Run Application**:
```bash
dotnet run --project TarkovBuddie/TarkovBuddie.csproj
```

**Output Type**: Windows Executable (WinExe) - produces `TarkovBuddie.exe`

## Main Components

### Application Entry Point
- **Class**: `TarkovBuddie.App` (App.xaml.cs:10) - WPF Application class
- **Primary Window**: `TarkovBuddie.MainWindow` (MainWindow.xaml.cs:17) - Main application window with Grid-based layout

### User Interface Structure
The application features a 4-tab tabbed interface (MainWindow dimensions: 800×450):

1. **Map Tab** - Map selection, preview, marker management, zoom controls
2. **Quest Tracker Tab** - Trader quest tree view, storyline quest display, completion toggles, Kappa progress, pin/unpin functionality
3. **Items Tracker Tab** - Item grid, quantity controls, pin/unpin items
4. **Settings Tab** - Hotkey configuration, language selection, overlay transparency, data file paths

### Map Assets
Embedded WebP images for all Tarkov maps (in `maps/` directory):
- woods, terminal, streets, shoreline, reserve, lighthouse, labrynth, lab, interchange, ground_zero, factory, customs

### Key Features
- **Map Overlay** - Topmost transparent window with toggleable hotkey, zoom controls, custom markers per map
- **Quest Tracking** - Trader quests (JSON-based) and main storyline quests with independent progression
- **Item Tracking** - Tracks items for quests, hideout upgrades with grid-based quantity control
- **Hotkey System** - Global configurable hotkeys for overlay toggling and window management
- **Localization** - English and Russian language support with runtime switching
- **Local Data Storage** - Completed quests, item quantities, markers, pinned items, selected map state

## Testing

**No test framework currently implemented** - Project structure does not contain dedicated test projects or test files.

## Localization

**Supported Languages**:
- English
- Russian

Localization approach:
- UI strings stored in WPF resource files
- Language switchable at runtime from Settings tab
- JSON data supports localized names for quests and items

## Data Storage

Application stores user progress locally:
- Completed trader and storyline quests
- Item quantities and tracking
- Map markers per map
- Pinned quests and items
- Selected map state

Data format: JSON files for quest/item definitions; local storage for user progress and application state
