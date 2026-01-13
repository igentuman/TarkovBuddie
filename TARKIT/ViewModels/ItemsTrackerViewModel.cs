using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using TARKIT.Models;
using TARKIT.Services;

namespace TARKIT.ViewModels;

public class ItemsTrackerViewModel : ViewModelBase
{
    private readonly ItemService _itemService;
    private readonly SettingsService _settingsService;
    private ObservableCollection<ItemViewModel> _items = new();
    private ObservableCollection<ItemViewModel> _filteredItems = new();
    private string _searchText = "";
    private bool _showOnlyIncomplete = false;
    private ItemViewModel? _selectedItem;

    public ObservableCollection<ItemViewModel> FilteredItems => _filteredItems;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                RefreshFilteredItems();
            }
        }
    }

    public bool ShowOnlyIncomplete
    {
        get => _showOnlyIncomplete;
        set
        {
            if (SetProperty(ref _showOnlyIncomplete, value))
            {
                RefreshFilteredItems();
            }
        }
    }

    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand TogglePinCommand { get; }

    public ItemsTrackerViewModel()
    {
        _itemService = new ItemService();
        _settingsService = SettingsService.Instance;

        IncreaseQuantityCommand = new RelayCommand(ExecuteIncreaseQuantity);
        DecreaseQuantityCommand = new RelayCommand(ExecuteDecreaseQuantity);
        TogglePinCommand = new RelayCommand(ExecuteTogglePin);

        LoadItems();
        _settingsService.SettingsChanged += OnSettingsChanged;
    }

    private void LoadItems()
    {
        _items.Clear();
        _filteredItems.Clear();

        var itemsData = _itemService.LoadItems();
        System.Diagnostics.Debug.WriteLine($"[ItemsTracker] Loaded {itemsData.Count} items from ItemService");
        var tracker = _itemService.LoadItemProgress();

        foreach (var item in itemsData)
        {
            if (tracker.ItemQuantities.TryGetValue(item.Id, out var quantity))
            {
                item.CurrentQuantity = quantity;
            }

            if (tracker.PinnedItemIds.Contains(item.Id))
            {
                item.IsPinned = true;
            }

            var viewModel = new ItemViewModel(item);
            viewModel.QuantityChanged += OnItemQuantityChanged;
            viewModel.PinStatusChanged += OnItemPinStatusChanged;
            _items.Add(viewModel);
        }

        RefreshFilteredItems();
    }

    private void RefreshFilteredItems()
    {
        _filteredItems.Clear();

        var filtered = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            filtered = filtered.Where(i =>
                i.DisplayName.ToLower().Contains(search) ||
                i.ShortName.ToLower().Contains(search)
            );
        }

        if (ShowOnlyIncomplete)
        {
            filtered = filtered.Where(i => !i.IsCompleted);
        }

        foreach (var item in filtered.OrderBy(i => i.DisplayName))
        {
            _filteredItems.Add(item);
        }
        
        System.Diagnostics.Debug.WriteLine($"[ItemsTracker] RefreshFilteredItems: {_filteredItems.Count} items after filtering");
    }

    private void ExecuteIncreaseQuantity(object? parameter)
    {
        if (parameter is ItemViewModel item)
        {
            item.CurrentQuantity++;
        }
    }

    private void ExecuteDecreaseQuantity(object? parameter)
    {
        if (parameter is ItemViewModel item && item.CurrentQuantity > 0)
        {
            item.CurrentQuantity--;
        }
    }

    private void ExecuteTogglePin(object? parameter)
    {
        if (parameter is ItemViewModel item)
        {
            item.IsPinned = !item.IsPinned;
        }
    }

    private void OnItemQuantityChanged(ItemViewModel item)
    {
        SaveItemProgress();
    }

    private void OnItemPinStatusChanged(ItemViewModel item)
    {
        SaveItemProgress();
        RefreshFilteredItems();
    }

    private void SaveItemProgress()
    {
        var tracker = new ItemTracker();

        foreach (var item in _items)
        {
            tracker.ItemQuantities[item.Item.Id] = item.CurrentQuantity;
            if (item.IsPinned)
            {
                tracker.PinnedItemIds.Add(item.Item.Id);
            }
        }

        _itemService.SaveItemProgress(tracker);
    }

    private void OnSettingsChanged(ApplicationSettings settings)
    {
        RefreshFilteredItems();
    }
}

public class ItemViewModel : ViewModelBase
{
    private Item _item;
    private int _currentQuantity;
    private bool _isPinned;

    public event Action<ItemViewModel>? QuantityChanged;
    public event Action<ItemViewModel>? PinStatusChanged;

    public Item Item => _item;

    public int CurrentQuantity
    {
        get => _currentQuantity;
        set
        {
            if (SetProperty(ref _currentQuantity, Math.Max(0, value)))
            {
                _item.CurrentQuantity = value;
                OnPropertyChanged(nameof(IsCompleted));
                QuantityChanged?.Invoke(this);
            }
        }
    }

    public bool IsPinned
    {
        get => _isPinned;
        set
        {
            if (SetProperty(ref _isPinned, value))
            {
                _item.IsPinned = value;
                PinStatusChanged?.Invoke(this);
            }
        }
    }

    public bool IsCompleted => CurrentQuantity >= _item.RequiredQuantity;
    public string DisplayName => _item.NameEn;
    public string ShortName => _item.NameShort;
    public int RequiredQuantity => _item.RequiredQuantity;
    public string IconId => _item.IconId;
    
    public string IconPath
    {
        get
        {
            if (string.IsNullOrEmpty(_item.IconId))
                return string.Empty;
            
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appDirectory = Path.GetDirectoryName(assemblyLocation) ?? "";
            var iconPath = Path.Combine(appDirectory, "icons", "items", $"{_item.IconId}.png");
            
            if (File.Exists(iconPath))
            {
                return "file:///" + iconPath.Replace("\\", "/");
            }
            
            return string.Empty;
        }
    }

    public ItemViewModel(Item item)
    {
        _item = item;
        _currentQuantity = item.CurrentQuantity;
        _isPinned = item.IsPinned;
    }
}
