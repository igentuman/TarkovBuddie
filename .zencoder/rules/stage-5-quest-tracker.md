---
description: Stage 5 - Quest Tracker Implementation
alwaysApply: false
---

# Stage 5: Quest Tracker Implementation

**Objective**: Implement trader and storyline quest management with independent progression tracking.

## Key Features

### Trader Quests Section
- **TreeView Structure**: Trader → Quest Chain → Individual Quest
- **Quest Details**:
  - Quest name (localized)
  - Trader name
  - Required map(s)
  - Completion status
  - Chain dependencies

### Storyline Quests Section
- **Linear View**: Separate section from trader quests
- **Quest Details**:
  - Quest name (localized)
  - Story chapter/stage
  - Dependencies on previous quests
  - Related map(s)
  - Completion status

### Completion Toggles
- Checkbox to mark quest complete/incomplete
- Completion state persisted to local storage
- Visual indication:
  - Completed: greyed out, strikethrough, or different color
  - Incomplete: normal display

### Kappa Progress
- Display: X/Y Kappa-required quests completed
- Percentage indicator
- Updates automatically when quest completion changes
- Only for trader quests

### Pin/Unpin Functionality
- Pin quests to overlay for raid reference
- Pinned quests visible on map overlay
- Filtered by current selected map (only show relevant quests)
- Pin status persisted
- Visual indicator (star, pin icon, etc.)

## Implementation Details

### Quest Data Models
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
    
    public string LocalizedName => AppSettings.CurrentLanguage == "Russian" ? NameRu : NameEn;
}

public class StorylineQuest
{
    public string Id { get; set; }
    public string NameRu { get; set; }
    public string NameEn { get; set; }
    public int Chapter { get; set; }
    public int Stage { get; set; }
    public string[] RequiredMaps { get; set; }
    public string[] DependsOnQuestIds { get; set; }
    
    public string LocalizedName => AppSettings.CurrentLanguage == "Russian" ? NameRu : NameEn;
}
```

### Quest Progress State
```csharp
public class QuestProgressState
{
    public string QuestId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsPinned { get; set; } = false;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
```

### QuestTrackerViewModel
```csharp
public class QuestTrackerViewModel : ViewModelBase
{
    private ObservableCollection<TraderQuestTreeItem> _traderQuestTree = new();
    private ObservableCollection<StorylineQuestViewModel> _storylineQuests = new();
    private int _kappaCompleted = 0;
    private int _kappaTotal = 0;
    
    public ObservableCollection<TraderQuestTreeItem> TraderQuestTree => _traderQuestTree;
    public ObservableCollection<StorylineQuestViewModel> StorylineQuests => _storylineQuests;
    
    public int KappaCompleted 
    { 
        get => _kappaCompleted;
        set { _kappaCompleted = value; OnPropertyChanged(); }
    }
    
    public int KappaTotal 
    { 
        get => _kappaTotal;
        set { _kappaTotal = value; OnPropertyChanged(); }
    }
    
    public double KappaPercentage => KappaTotal > 0 ? (KappaCompleted * 100.0) / KappaTotal : 0;
    
    public ICommand ToggleQuestCompletionCommand { get; }
    public ICommand ToggleQuestPinCommand { get; }
}

public class TraderQuestTreeItem : ViewModelBase
{
    public string Content { get; set; }
    public ObservableCollection<TraderQuestTreeItem> Children { get; set; } = new();
    public TraderQuest? Quest { get; set; }
}

public class StorylineQuestViewModel : ViewModelBase
{
    private StorylineQuest _quest;
    private bool _isCompleted;
    private bool _isPinned;
    
    public StorylineQuest Quest => _quest;
    
    public bool IsCompleted 
    { 
        get => _isCompleted;
        set { _isCompleted = value; OnPropertyChanged(); }
    }
    
    public bool IsPinned 
    { 
        get => _isPinned;
        set { _isPinned = value; OnPropertyChanged(); }
    }
    
    public string LocalizedName => Quest.LocalizedName;
    public int Chapter => Quest.Chapter;
    public int Stage => Quest.Stage;
}
```

### QuestTrackerView.xaml Structure
```xaml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    
    <!-- Trader Quests Section -->
    <GroupBox Grid.Row="0" Header="Trader Quests">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="*" />
                <RowDefinition Height="Auto" />
            </Grid.RowDefinitions>
            
            <!-- TreeView for quest hierarchy -->
            <TreeView Grid.Row="0" ItemsSource="{Binding TraderQuestTree}">
                <TreeView.ItemTemplate>
                    <HierarchicalDataTemplate ItemsSource="{Binding Children}">
                        <StackPanel Orientation="Horizontal">
                            <CheckBox IsChecked="{Binding Quest.IsCompleted}" 
                                      Margin="0,0,5,0" />
                            <TextBlock Text="{Binding Content}" />
                            <Button Content="📌" Margin="10,0,0,0" 
                                    Command="{Binding TogglePinCommand}" />
                        </StackPanel>
                    </HierarchicalDataTemplate>
                </TreeView.ItemTemplate>
            </TreeView>
            
            <!-- Kappa Progress -->
            <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="5">
                <TextBlock Text="Kappa Progress: " />
                <TextBlock Text="{Binding KappaCompleted}" />
                <TextBlock Text="/" />
                <TextBlock Text="{Binding KappaTotal}" />
                <ProgressBar Width="200" Margin="10,0,0,0" 
                             Value="{Binding KappaPercentage}" 
                             Maximum="100" />
                <TextBlock Text="{Binding KappaPercentage, StringFormat='{0:P0}'}" Margin="10,0,0,0" />
            </StackPanel>
        </Grid>
    </GroupBox>
    
    <!-- Divider -->
    <GridSplitter Grid.Row="1" Height="5" HorizontalAlignment="Stretch" />
    
    <!-- Storyline Quests Section -->
    <GroupBox Grid.Row="2" Header="Storyline Quests">
        <ListBox ItemsSource="{Binding StorylineQuests}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <Grid Margin="5">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        
                        <!-- Completion Checkbox -->
                        <CheckBox Grid.Column="0" IsChecked="{Binding IsCompleted}" 
                                  VerticalAlignment="Center" />
                        
                        <!-- Chapter/Stage -->
                        <TextBlock Grid.Column="1" Text="{Binding Chapter, StringFormat='Ch. {0}.{1}'}" 
                                   Foreground="Gray" Margin="10,0,10,0" />
                        
                        <!-- Quest Name -->
                        <TextBlock Grid.Column="2" Text="{Binding LocalizedName}">
                            <TextBlock.Style>
                                <Style TargetType="TextBlock">
                                    <Style.Triggers>
                                        <DataTrigger Binding="{Binding IsCompleted}" Value="True">
                                            <Setter Property="TextDecorations" Value="Strikethrough" />
                                            <Setter Property="Foreground" Value="Gray" />
                                        </DataTrigger>
                                    </Style.Triggers>
                                </Style>
                            </TextBlock.Style>
                        </TextBlock>
                        
                        <!-- Pin Button -->
                        <Button Grid.Column="3" Content="📌" Margin="10,0,0,0"
                                Command="{Binding TogglePinCommand}"
                                Foreground="{Binding IsPinned, Converter={StaticResource BoolToColorConverter}}" />
                    </Grid>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </GroupBox>
</Grid>
```

### QuestService
```csharp
public class QuestService
{
    public List<TraderQuest> LoadTraderQuests(string questsJsonPath)
    {
        // Load trader quests from JSON file
    }
    
    public List<StorylineQuest> LoadStorylineQuests(string questsJsonPath)
    {
        // Load storyline quests from JSON file
    }
    
    public void SaveQuestProgress(QuestProgressState state, string userDataPath)
    {
        // Save quest completion state to user progress JSON
    }
    
    public QuestProgressState? LoadQuestProgress(string questId, string userDataPath)
    {
        // Load quest completion state from user progress JSON
    }
    
    public List<QuestProgressState> LoadAllQuestProgress(string userDataPath)
    {
        // Load all quest states from user progress
    }
    
    public int CalculateKappaProgress(List<TraderQuest> quests, List<QuestProgressState> progress)
    {
        // Calculate number of completed Kappa-required quests
    }
    
    public int GetKappaTotal(List<TraderQuest> quests)
    {
        // Get total number of Kappa-required quests
    }
}
```

## Data Files

### trader_quests.json Structure
```json
[
  {
    "id": "quest_001",
    "nameEn": "Debut",
    "nameRu": "Дебют",
    "trader": "Prapor",
    "requiredMaps": ["customs"],
    "requiredForKappa": true,
    "chainedQuestIds": ["quest_002"]
  },
  {
    "id": "quest_002",
    "nameEn": "Checking",
    "nameRu": "Проверка",
    "trader": "Prapor",
    "requiredMaps": ["customs", "woods"],
    "requiredForKappa": true,
    "chainedQuestIds": []
  }
]
```

### storyline_quests.json Structure
```json
[
  {
    "id": "story_001",
    "nameEn": "The Arrival",
    "nameRu": "Прибытие",
    "chapter": 1,
    "stage": 1,
    "requiredMaps": ["customs"],
    "dependsOnQuestIds": []
  },
  {
    "id": "story_002",
    "nameEn": "Escape",
    "nameRu": "Побег",
    "chapter": 1,
    "stage": 2,
    "requiredMaps": ["customs"],
    "dependsOnQuestIds": ["story_001"]
  }
]
```

### User Progress (quests_progress.json)
```json
{
  "quests": [
    {
      "questId": "quest_001",
      "isCompleted": true,
      "isPinned": false,
      "lastUpdated": "2024-01-11T16:01:07Z"
    },
    {
      "questId": "story_001",
      "isCompleted": false,
      "isPinned": true,
      "lastUpdated": "2024-01-11T15:00:00Z"
    }
  ]
}
```

## Tasks Checklist

### Data Model & ViewModel
- [ ] Create `TraderQuest` class with localization
- [ ] Create `StorylineQuest` class with localization
- [ ] Create `QuestProgressState` class
- [ ] Create `QuestTrackerViewModel` with collections
- [ ] Create `TraderQuestTreeItem` for tree structure
- [ ] Create `StorylineQuestViewModel` wrapper

### QuestService
- [ ] Load trader quests from JSON
- [ ] Load storyline quests from JSON
- [ ] Load quest progress from user progress JSON
- [ ] Save quest progress to JSON
- [ ] Calculate Kappa progress
- [ ] Handle file I/O and JSON serialization

### UI Implementation (QuestTrackerView.xaml)
- [ ] Split layout: Trader Quests / Storyline Quests
- [ ] TreeView for trader quest hierarchy
- [ ] ListBox for storyline quests
- [ ] Checkboxes for quest completion
- [ ] Pin buttons for each quest
- [ ] Kappa progress display (counter + progress bar)
- [ ] Visual styling for completed quests

### Commands
- [ ] `ToggleQuestCompletionCommand` - mark quest complete/incomplete
- [ ] `ToggleQuestPinCommand` - pin/unpin quest

### Integration
- [ ] Wire up QuestTrackerViewModel to MainWindowViewModel
- [ ] Load trader and storyline quests on application start
- [ ] Load user progress for quests
- [ ] Auto-save quest progress when completion changes
- [ ] Calculate Kappa on startup and after quest changes
- [ ] Update UI when language changes
- [ ] Filter pinned quests by selected map

---

## Implementation Guidelines

### TreeView Structure
Build a tree from flat quest data using chains:
```csharp
// Root node per trader
// Children: quest chains
// Leaf nodes: individual quests
var traderGroups = quests.GroupBy(q => q.Trader)
    .Select(g => new TraderQuestTreeItem { Content = g.Key, ... })
```

### Kappa Calculation
```csharp
public int CalculateKappaProgress(List<TraderQuest> quests, List<QuestProgressState> progress)
{
    return quests
        .Where(q => q.RequiredForKappa)
        .Count(q => progress.FirstOrDefault(p => p.QuestId == q.Id)?.IsCompleted ?? false);
}
```

### Dependency Handling
When marking a quest complete, check dependencies:
- For trader quests: show warning if prerequisite quest not completed
- For storyline quests: disable completion if dependency not met

### Pin Filtering by Map
When pinned quests are shown on map overlay:
- Filter pinned quests by current selected map
- Only show quests where quest.RequiredMaps contains selectedMap

### Localization
- Store English and Russian names in data model
- Use `AppSettings.CurrentLanguage` to determine display name
- Update TreeView/ListBox when language changes

### Visual Feedback
- Completed quests: strikethrough, grey color, or reduced opacity
- Pinned quests: highlight or icon change
- Kappa progress: update automatically on completion toggle

---

## Testing & Verification

- [ ] Trader quests load and display in TreeView
- [ ] Storyline quests load and display in ListBox
- [ ] Quest completion toggle works
- [ ] Completion state persists to JSON
- [ ] Kappa progress calculates correctly
- [ ] Kappa progress updates on completion toggle
- [ ] Pin/unpin functionality works
- [ ] Pinned quests filtered by current map
- [ ] Language change updates all quest names
- [ ] Completed quests styled correctly (strikethrough, grey)
- [ ] Pinned quests show visual indicator
- [ ] TreeView hierarchy displays correctly
- [ ] No console errors or warnings
- [ ] Quest dependencies enforced (optional)
- [ ] Overlay receives pinned quests for current map

