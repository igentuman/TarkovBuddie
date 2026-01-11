# Tarkov Buddie

**Offline companion application for Escape from Tarkov**

Tarkov Buddie is a lightweight C# desktop application designed to assist Escape from Tarkov players during raids.  
It provides map overlays, quest tracking, and item tracking through transparent topmost windows and configurable hotkeys.

> ⚠️ Tarkov Buddie does **not** interact with game memory or network traffic. It works purely as an external helper tool.

---

## Features

### 1. Map Overlay
- Toggleable via hotkey
- Topmost transparent window with PNG map
- Zoom in / zoom out
- Place and remove custom markers
- Markers are stored per map

**Planned:**
- Automatic quest-related markers

---

### 2. Quest Tracker

Tarkov Buddie supports **two independent quest systems**:
- **Trader quests**
- **Main storyline quests** (introduced in Escape from Tarkov 1.0)

#### Trader Quests
- Loaded from JSON files
- Tree view structure (Trader → Quest chain → Quest)
- Mark quests as completed
- Shows progress toward Kappa container
- Pin quests to overlay

#### Main Storyline Quests
- Stored separately from trader quests
- Displayed in a dedicated storyline section
- Independent progression tracking
- Can be pinned to overlay
- May influence future features such as map markers and progression hints

**Overlay:**
- Transparent topmost window
- Filtered by current map
- Can show both trader and storyline quests

---

### 3. App State – Current Map
- Manual map selection
- Affects:
  - Visible pinned quests
  - Map overlay
  - Quest markers (current and planned)

**Planned:**
- Automatic map recognition via exits

---

### 4. Items Tracker
- Tracks items required for:
  - Trader quests
  - Main storyline quests
  - Hideout upgrades
- Grid view with required / current quantity
- Increment / decrement item count
- Pin items to overlay

**Overlay:**
- Transparent topmost window showing pinned items

---

## Hotkeys

All hotkeys are configurable.

Available actions:
- Toggle map overlay
- Toggle pinned quests overlay
- Toggle quest tracker window
- Toggle pinned items overlay
- Toggle items tracker window

Hotkeys work globally while the game is focused.

---

## User Interface

The main window contains 4 tabs:

1. **Map**
   - Map selection
   - Map preview
   - Marker management
   - Zoom controls

2. **Quest Tracker**
   - Trader quests tree
   - Main storyline quests view
   - Completion toggles
   - Kappa progress (trader quests only)
   - Pin / unpin quests

3. **Items Tracker**
   - Items grid
   - Quantity controls
   - Pin / unpin items

4. **Settings**
   - Hotkey configuration
   - Language selection
   - Overlay transparency
   - Data file paths

---

## Localization

Supported languages:
- English
- Russian

Localization details:
- UI strings stored in resource files
- Language switchable at runtime
- JSON data supports localized names for:
  - Trader quests
  - Main storyline quests
  - Items

---

## Data Storage

### Trader Quest Data (JSON)
Includes:
- Quest ID
- Localized name
- Trader
- Required map(s)
- Kappa requirement flag
- Quest chain links

### Main Storyline Quest Data (JSON)
Includes:
- Quest ID
- Localized name
- Story chapter / stage
- Related map(s)
- Dependencies on previous storyline quests

### Item Data (JSON)
Includes:
- Item ID
- Localized name
- Required quantity
- Category:
  - Trader quest
  - Storyline quest
  - Hideout

### User Progress
Stored locally:
- Completed trader quests
- Completed storyline quests
- Item quantities
- Map markers
- Pinned quests and items
- Selected map

---

## Technical Information

- Platform: Windows
- Language: C# (.NET)
- Overlay windows:
  - Borderless
  - Transparent
  - Always on top
- No online connectivity
- No interaction with game process

---

## Planned Features
- Automatic map detection
- Quest-based map markers
- Storyline-driven hints and progression indicators
- Advanced filtering
- Profiles per wipe
- Optional cloud sync

---

## Disclaimer

Tarkov Buddie is a third-party companion tool and is not affiliated with or endorsed by Battlestate Games.

Use at your own discretion.
