---
description: Stage 4 - Items Tracker Implementation
alwaysApply: false
---

# Stage 4: Items Tracker Implementation

**Objective**: Create an item tracking interface for quest and hideout items.

## Key Features

### Item Grid View
- Display items in DataGrid format
- Columns:
  - Item name (localized based on language setting)
  - Required quantity
  - Current quantity
  - Category (Trader Quest / Storyline Quest / Hideout)
  - Status (visual indicator: completed or pending)

### Quantity Controls
- **+/- Buttons**: Increment/decrement current quantity
- **Manual Input**: TextBox to set quantity directly
- **Visual Feedback**: Highlight items when completed (current ≥ required)
- **Validation**: Prevent negative quantities

### Pin/Unpin Functionality
- Pin items to overlay for raid visibility
- Pinned items displayed separately (toggle view)
- Pin status persisted to local storage
- Visual indicator (star, checkmark, etc.) for pinned items

### Filtering (Optional)
- Filter by category (Trader Quest / Storyline Quest / Hideout)
- Filter by completion status (completed / incomplete)
- Search by item name

## Implementation Details

### Item Data Model
```csharp
public class Item
{
    public string Id { get; set; }
    public string NameRu { get; set; }
    public string NameEn { get; set; }
    public int RequiredQuantity { get; set; }
    public ItemCategory Category { get; set; }
    public string RelatedQuestId { get; set; }
    
    public string LocalizedName => AppSettings.CurrentLanguage == "Russian" ? NameRu : NameEn;
}

public enum ItemCategory
{
    TraderQuest,
    StorylineQuest,
    Hideout
}
```

### Item Tracker State
```csharp
public class ItemTrackerState
{
    public string ItemId { get; set; }
    public int CurrentQuantity { get; set; } = 0;
    public bool IsPinned { get; set; } = false;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
```

### ItemsTrackerViewModel
```csharp
public class ItemsTrackerViewModel : ViewModelBase
{
    private ObservableCollection<ItemViewModel> _items = new();
    private ObservableCollection<ItemViewModel> _pinnedItems = new();
    private ItemCategory? _selectedCategory;
    private string _searchText = "";
    private bool _showOnlyIncomplete = false;
    
    public ObservableCollection<ItemViewModel> Items => _items;
    public ObservableCollection<ItemViewModel> PinnedItems => _pinnedItems;
    
    public string SearchText 
    { 
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); RefreshItems(); }
    }
    
    public ItemCategory? SelectedCategory 
    { 
        get => _selectedCategory;
        set { _selectedCategory = value; OnPropertyChanged(); RefreshItems(); }
    }
    
    public bool ShowOnlyIncomplete 
    { 
        get => _showOnlyIncomplete;
        set { _showOnlyIncomplete = value; OnPropertyChanged(); RefreshItems(); }
    }
    
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand SetQuantityCommand { get; }
    public ICommand TogglePinCommand { get; }
}

public class ItemViewModel : ViewModelBase
{
    private Item _item;
    private int _currentQuantity;
    private bool _isPinned;
    
    public Item Item => _item;
    
    public int CurrentQuantity 
    { 
        get => _currentQuantity;
        set { _currentQuantity = Math.Max(0, value); OnPropertyChanged(); }
    }
    
    public bool IsPinned 
    { 
        get => _isPinned;
        set { _isPinned = value; OnPropertyChanged(); }
    }
    
    public bool IsCompleted => CurrentQuantity >= Item.RequiredQuantity;
    public string LocalizedName => Item.LocalizedName;
    public int RequiredQuantity => Item.RequiredQuantity;
    public ItemCategory Category => Item.Category;
}
```

### ItemsTrackerView.xaml Structure
```xaml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    
    <!-- Filters -->
    <StackPanel Grid.Row="0" Orientation="Horizontal">
        <TextBlock Text="Search:" />
        <TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" Width="200" />
        
        <TextBlock Text="Category:" Margin="20,0,0,0" />
        <ComboBox ItemsSource="{Binding Categories}" 
                  SelectedItem="{Binding SelectedCategory}" Width="150" />
        
        <CheckBox IsChecked="{Binding ShowOnlyIncomplete}" 
                  Content="Show Only Incomplete" Margin="20,0,0,0" />
    </StackPanel>
    
    <!-- Items DataGrid -->
    <DataGrid Grid.Row="1" ItemsSource="{Binding Items}" AutoGenerateColumns="False">
        <DataGrid.Columns>
            <DataGridCheckBoxColumn Header="Pin" Binding="{Binding IsPinned}" Width="40" />
            <DataGridTextColumn Header="Item" Binding="{Binding LocalizedName}" Width="*" />
            <DataGridTextColumn Header="Required" Binding="{Binding RequiredQuantity}" Width="80" IsReadOnly="True" />
            <DataGridTemplateColumn Header="Current" Width="150">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <StackPanel Orientation="Horizontal">
                            <Button Content="-" Command="{Binding DecreaseQuantityCommand}" Width="30" />
                            <TextBox Text="{Binding CurrentQuantity}" Width="50" />
                            <Button Content="+" Command="{Binding IncreaseQuantityCommand}" Width="30" />
                        </StackPanel>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="Category" Binding="{Binding Category}" Width="120" IsReadOnly="True" />
            <DataGridTextColumn Header="Status" Binding="{Binding Status}" Width="100" IsReadOnly="True" />
        </DataGrid.Columns>
    </DataGrid>
    
    <!-- Pinned Items Section -->
    <Expander Grid.Row="2" Header="Pinned Items" IsExpanded="True">
        <ItemsControl ItemsSource="{Binding PinnedItems}">
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <Grid Margin="5">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="100" />
                            <ColumnDefinition Width="30" />
                        </Grid.ColumnDefinitions>
                        
                        <TextBlock Grid.Column="0" 
                                   Text="{Binding LocalizedName}" />
                        <TextBlock Grid.Column="1" 
                                   Text="{Binding CurrentQuantity, StringFormat='{0}/{1}', StringFormatParameter={Binding RequiredQuantity}}" />
                        <Button Grid.Column="2" Content="×" 
                                Command="{Binding TogglePinCommand}" />
                    </Grid>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </Expander>
</Grid>
```

### ItemService
```csharp
public class ItemService
{
    public List<Item> LoadItems(string itemsJsonPath)
    {
        // Load items from JSON file
    }
    
    public void SaveItemTracker(ItemTrackerState state, string userDataPath)
    {
        // Save item quantity to user progress JSON
    }
    
    public ItemTrackerState? LoadItemTracker(string itemId, string userDataPath)
    {
        // Load item quantity from user progress JSON
    }
    
    public List<ItemTrackerState> LoadAllItemTrackers(string userDataPath)
    {
        // Load all item states from user progress
    }
    
    public void SaveAllItemTrackers(List<ItemTrackerState> states, string userDataPath)
    {
        // Save all item states to user progress
    }
}
```

## Data Files

### items.json Structure
```json
[
  {
    "id": "item_001",
    "nameEn": "Salewa",
    "nameRu": "Салева",
    "requiredQuantity": 2,
    "category": "TraderQuest",
    "relatedQuestId": "quest_001"
  },
  {
    "id": "item_002",
    "nameEn": "Matches",
    "nameRu": "Спички",
    "requiredQuantity": 5,
    "category": "Hideout",
    "relatedQuestId": ""
  }
]
```

### User Progress (items_progress.json)
```json
{
  "items": [
    {
      "itemId": "item_001",
      "currentQuantity": 1,
      "isPinned": true,
      "lastUpdated": "2024-01-11T16:01:07Z"
    }
  ]
}
```

## Tasks Checklist

### Data Model & ViewModel
- [ ] Create `Item` class with localization support
- [ ] Create `ItemCategory` enum
- [ ] Create `ItemTrackerState` class
- [ ] Create `ItemsTrackerViewModel` with filtering
- [ ] Create `ItemViewModel` wrapper for binding

### ItemService
- [ ] Load items from JSON file
- [ ] Load item tracker state from user progress
- [ ] Save item tracker state to user progress
- [ ] Handle file I/O and JSON serialization

### UI Implementation (ItemsTrackerView.xaml)
- [ ] DataGrid with columns (pin, name, required, current, category, status)
- [ ] +/- buttons for quantity control
- [ ] TextBox for manual quantity input
- [ ] Pin checkbox column
- [ ] Filters: search, category, completion status
- [ ] Pinned items section (Expander)

### Commands
- [ ] `IncreaseQuantityCommand` - increment quantity
- [ ] `DecreaseQuantityCommand` - decrement quantity
- [ ] `SetQuantityCommand` - set quantity from textbox
- [ ] `TogglePinCommand` - pin/unpin item

### Integration
- [ ] Wire up ItemsTrackerViewModel to MainWindowViewModel
- [ ] Load items on application start
- [ ] Load user progress for items
- [ ] Auto-save item progress when quantity changes
- [ ] Update UI when language changes

---

## Implementation Guidelines

### Localization
- Store English and Russian names in data model
- Use `AppSettings.CurrentLanguage` to determine display name
- Update DataGrid when language changes

### Quantity Validation
- Prevent negative quantities (use Math.Max(0, value))
- Allow quantities up to practical limit (e.g., 1000)
- Visual feedback: green for completed, red for pending

### Filtering Logic
- Apply multiple filters (search + category + completion)
- Use LINQ to filter items
- Debounce search input (avoid filtering on every keystroke)

### Pin/Unpin
- Maintain separate PinnedItems collection
- Update both collections when pin status changes
- Persist pin status to user progress file
- Show count of pinned items somewhere

### Performance
- Use virtual scrolling in DataGrid if there are many items (100+)
- Debounce quantity changes before saving
- Load items once on startup, cache in memory

---

## Testing & Verification

- [ ] All items load and display correctly
- [ ] Search filters items by name
- [ ] Category filter works correctly
- [ ] Completion status filter works
- [ ] +/- buttons increment/decrement quantity
- [ ] Manual quantity input works
- [ ] Quantity changes persist to JSON
- [ ] Pin/unpin functionality works
- [ ] Pinned items section shows only pinned items
- [ ] Language change updates all item names
- [ ] No console errors or warnings
- [ ] DataGrid sorts by columns correctly
- [ ] Items display correct category

