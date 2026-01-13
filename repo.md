# TARKIT Repository

## Project Overview

**Tarkov Buddie** is an offline companion application for Escape from Tarkov. This Windows desktop application provides map overlays, quest tracking, and item tracking through transparent topmost windows with configurable hotkeys.

The application works purely as an external helper tool and does **not** interact with game memory or network traffic.

## Technology Stack

- **Language**: C# (.NET 9.0)
- **Framework**: Windows Presentation Foundation (WPF)
- **Platform**: Windows only
- **IDE**: JetBrains Rider (project structure indicates `.idea` configuration)
- **Architecture**: Single-project solution with XAML UI

## Project Structure

```
TARKIT/
├── TARKIT/                    # Main application project
│   ├── App.xaml                     # Application root XAML
│   ├── App.xaml.cs                  # Application code-behind
│   ├── MainWindow.xaml              # Main window UI definition
│   ├── MainWindow.xaml.cs           # Main window logic
│   ├── AssemblyInfo.cs              # Assembly metadata
│   ├── TARKIT.csproj          # Project file
│   ├── README.md                    # Feature documentation
│   ├── bin/                         # Build output
│   └── obj/                         # Intermediate build files
├── TARKIT.sln                 # Visual Studio solution file
└── .zencoder/                       # Zencoder configuration
```

## Key Features

### 1. **Map Overlay**
- Toggleable transparent window overlay
- Display PNG maps for different Tarkov locations
- Zoom in/out functionality
- Custom marker placement and removal
- Per-map marker persistence

### 2. **Quest Tracking**
Supports two independent quest systems:
- **Trader Quests**: Loaded from JSON, organized in tree structure with Kappa progress tracking
- **Main Storyline Quests**: Story-driven quests with progression tracking
- Pin quests to overlay for quick reference
- Quest filtering by current map

### 3. **Items Tracker**
- Tracks items for trader quests, storyline quests, and hideout upgrades
- Grid view with required vs. current quantity
- Increment/decrement controls
- Pin items to overlay
- Persistent storage

### 4. **App State Management**
- Current map selection (manual, with auto-detection planned)
- Hotkey configuration (all actions customizable)
- Overlay transparency settings
- Data file path management

## Supported Languages

- English
- Russian

Localization is handled through resource files and JSON data, allowing runtime language switching.

## Configuration & Hotkeys

Configurable hotkey actions:
- Toggle map overlay
- Toggle pinned quests overlay
- Toggle quest tracker window
- Toggle pinned items overlay
- Toggle items tracker window

All hotkeys work globally when the game window is focused.

## User Interface

The main application window has 4 tabs:

1. **Map Tab**: Map selection, preview, marker management, zoom controls
2. **Quest Tracker Tab**: Trader quests tree, storyline quests, completion toggles, Kappa progress
3. **Items Tracker Tab**: Items grid, quantity controls, pin/unpin functionality
4. **Settings Tab**: Hotkey configuration, language selection, overlay transparency, data paths

## Data Storage

### Data Format
- Trader Quest Data (JSON): ID, localized name, trader, required maps, Kappa flag, chain links
- Main Storyline Quest Data (JSON): ID, name, chapter/stage, related maps, dependencies
- Item Data (JSON): ID, name, required quantity, category

### User Progress Storage
Stored locally with:
- Completed trader quests
- Completed storyline quests
- Item quantities
- Map markers
- Pinned quests and items
- Selected map

## Development Setup

### Requirements
- Windows OS
- .NET 9.0 SDK
- JetBrains Rider or Visual Studio 2022+

### Build & Run
```
dotnet build
dotnet run
```

### Configuration
All settings are configurable through the Settings tab in the application.

## Planned Features

- Automatic map detection via quest exits
- Quest-based automatic map markers
- Storyline-driven hints and progression indicators
- Advanced filtering options
- Per-wipe profiles
- Optional cloud synchronization

## Disclaimer

Tarkov Buddie is a third-party companion tool and is not affiliated with or endorsed by Battlestate Games. Use at your own discretion.

## Project Status

Active development with core features implemented and additional enhancements planned.
