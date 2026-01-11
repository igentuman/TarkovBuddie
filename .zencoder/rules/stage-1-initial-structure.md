---
description: Stage 1 - Initial Structure & Tabs
alwaysApply: false
---

# Stage 1: Initial Structure & Tabs

**Objective**: Establish the foundational application structure with a 4-tab interface and empty service/model classes.

## Tasks Checklist

### UI Structure
- [ ] Create `Views/` directory
- [ ] Create `Models/` directory
- [ ] Create `Services/` directory
- [ ] Create `ViewModels/` directory
- [ ] Update MainWindow.xaml with TabControl (4 tabs: Map, Quest Tracker, Items Tracker, Settings)
- [ ] Style MainWindow (800×450 size, Grid layout)

### Empty Classes (Models)
- [ ] `Models/Quest.cs` - Base quest model
- [ ] `Models/Item.cs` - Base item model
- [ ] `Models/QuestTracker.cs` - Tracker state model
- [ ] `Models/ItemTracker.cs` - Tracker state model

### Empty Services
- [ ] `Services/HotKeyService.cs` - Hotkey management
- [ ] `Services/SettingsService.cs` - Settings I/O
- [ ] `Services/QuestService.cs` - Quest data management
- [ ] `Services/ItemService.cs` - Item data management
- [ ] `Services/MapService.cs` - Map data management

### ViewModels (Empty with MVVM structure)
- [ ] `ViewModels/MainWindowViewModel.cs` - Main window logic
- [ ] `ViewModels/SettingsViewModel.cs` - Settings tab logic
- [ ] `ViewModels/MapViewModel.cs` - Map tab logic
- [ ] `ViewModels/QuestTrackerViewModel.cs` - Quest tracker tab logic
- [ ] `ViewModels/ItemsTrackerViewModel.cs` - Items tracker tab logic

### Views (XAML + Code-behind)
- [ ] `Views/SettingsView.xaml` - Settings tab content
- [ ] `Views/MapView.xaml` - Map tab content
- [ ] `Views/QuestTrackerView.xaml` - Quest tracker tab content
- [ ] `Views/ItemsTrackerView.xaml` - Items tracker tab content

### App Configuration
- [ ] Update App.xaml with application resources (if needed)
- [ ] Update MainWindow.xaml to wire up ViewModels

---

## Implementation Guidelines

### Directory Structure
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
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
└── MainWindow.xaml.cs
```

### ViewModel Base Class (Optional)
Consider creating a `ViewModelBase.cs` with INotifyPropertyChanged implementation:
```csharp
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### MainWindow.xaml Structure
```xaml
<TabControl>
    <TabItem Header="Map" Content="{Binding MapView}" />
    <TabItem Header="Quest Tracker" Content="{Binding QuestTrackerView}" />
    <TabItem Header="Items Tracker" Content="{Binding ItemsTrackerView}" />
    <TabItem Header="Settings" Content="{Binding SettingsView}" />
</TabControl>
```

### Empty Model Template
All models should include:
- Auto-properties with null safety in mind
- Basic constructor
- Property for serialization (if needed)

### Empty Service Template
All services should include:
- Constructor
- Public methods (stubs)
- Private helper methods (stubs)

### Empty ViewModel Template
All ViewModels should include:
- Inherit from ViewModelBase (or implement INotifyPropertyChanged)
- Constructor
- Public properties with INotifyPropertyChanged bindings
- Command stubs (RelayCommand)

---

## Code Style Reminders

- Use file-scoped namespaces: `namespace TarkovBuddie;`
- Enable nullable reference types: `#nullable enable` (if needed)
- Use auto-properties
- Use record types for data models if appropriate
- Follow WPF naming: `*ViewModel.cs`, `*View.xaml`

---

## Testing & Verification

- [ ] Application builds without errors
- [ ] All 4 tabs visible in MainWindow
- [ ] Tab switching works smoothly
- [ ] Empty views render without errors
- [ ] No console warnings or exceptions

