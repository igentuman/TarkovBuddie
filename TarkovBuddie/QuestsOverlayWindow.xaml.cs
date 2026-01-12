using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TarkovBuddie.ViewModels;

namespace TarkovBuddie;

public partial class QuestsOverlayWindow : Window
{
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int GWL_EXSTYLE = -20;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    private readonly QuestTrackerViewModel _viewModel;
    private readonly MapViewModel? _mapViewModel;
    private ItemsOverlayWindow? _itemsOverlayWindow;
    private ObservableCollection<QuestViewModel> _pinnedQuests = new();
    private bool _isDragging;
    private System.Windows.Point _lastMousePosition;

    public ObservableCollection<QuestViewModel> PinnedQuests => _pinnedQuests;

    public QuestsOverlayWindow(QuestTrackerViewModel viewModel, ItemsOverlayWindow? itemsOverlayWindow = null, MapViewModel? mapViewModel = null)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _mapViewModel = mapViewModel;
        _itemsOverlayWindow = itemsOverlayWindow;
        DataContext = this;
        
        if (_itemsOverlayWindow != null)
        {
            _itemsOverlayWindow.VisibilityChanged += ItemsOverlayWindow_VisibilityChanged;
        }
        
        this.KeyDown += QuestsOverlayWindow_KeyDown;
        this.Closed += QuestsOverlayWindow_Closed;
        this.Loaded += QuestsOverlayWindow_Loaded;
        this.MouseLeftButtonDown += QuestsOverlayWindow_MouseLeftButtonDown;
        this.MouseMove += QuestsOverlayWindow_MouseMove;
        this.MouseLeftButtonUp += QuestsOverlayWindow_MouseLeftButtonUp;
    }

    private void QuestsOverlayWindow_Loaded(object sender, RoutedEventArgs e)
    {
        EnableClickThrough();
        AdjustPositionBasedOnItemsOverlay();
        RefreshPinnedQuests();
        if (_viewModel.FilteredQuests is INotifyCollectionChanged notifyCollection)
        {
            notifyCollection.CollectionChanged += FilteredQuests_CollectionChanged;
        }
        
        foreach (var quest in _viewModel.FilteredQuests)
        {
            if (quest is INotifyPropertyChanged notifyProperty)
            {
                notifyProperty.PropertyChanged += Quest_PropertyChanged;
            }
        }
        
        if (_mapViewModel is INotifyPropertyChanged mapNotifyProperty)
        {
            mapNotifyProperty.PropertyChanged += MapViewModel_PropertyChanged;
        }
    }

    private void AdjustPositionBasedOnItemsOverlay()
    {
        if (_itemsOverlayWindow != null && _itemsOverlayWindow.IsVisible)
        {
            this.Left = 110;
        }
        else
        {
            this.Left = 10;
        }
    }

    private void ItemsOverlayWindow_VisibilityChanged()
    {
        AdjustPositionBasedOnItemsOverlay();
    }

    public void RefreshPositionForItemsOverlay(ItemsOverlayWindow? itemsOverlayWindow)
    {
        _itemsOverlayWindow = itemsOverlayWindow;
        
        if (_itemsOverlayWindow != null)
        {
            _itemsOverlayWindow.VisibilityChanged -= ItemsOverlayWindow_VisibilityChanged;
            _itemsOverlayWindow.VisibilityChanged += ItemsOverlayWindow_VisibilityChanged;
        }
        
        AdjustPositionBasedOnItemsOverlay();
    }

    private void EnableClickThrough()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd != IntPtr.Zero)
        {
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT);
        }
    }

    private void QuestsOverlayWindow_Closed(object? sender, System.EventArgs e)
    {
        if (_itemsOverlayWindow != null)
        {
            _itemsOverlayWindow.VisibilityChanged -= ItemsOverlayWindow_VisibilityChanged;
        }
        
        if (_viewModel.FilteredQuests is INotifyCollectionChanged notifyCollection)
        {
            notifyCollection.CollectionChanged -= FilteredQuests_CollectionChanged;
        }
        
        foreach (var quest in _viewModel.FilteredQuests)
        {
            if (quest is INotifyPropertyChanged notifyProperty)
            {
                notifyProperty.PropertyChanged -= Quest_PropertyChanged;
            }
        }
        
        if (_mapViewModel is INotifyPropertyChanged mapNotifyProperty)
        {
            mapNotifyProperty.PropertyChanged -= MapViewModel_PropertyChanged;
        }
    }

    private void FilteredQuests_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var quest in e.NewItems.OfType<QuestViewModel>())
            {
                if (quest is INotifyPropertyChanged notifyProperty)
                {
                    notifyProperty.PropertyChanged += Quest_PropertyChanged;
                }
            }
        }

        if (e.OldItems != null)
        {
            foreach (var quest in e.OldItems.OfType<QuestViewModel>())
            {
                if (quest is INotifyPropertyChanged notifyProperty)
                {
                    notifyProperty.PropertyChanged -= Quest_PropertyChanged;
                }
            }
        }

        RefreshPinnedQuests();
    }

    private void Quest_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QuestViewModel.IsPinned))
        {
            RefreshPinnedQuests();
        }
    }

    private void MapViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapViewModel.SelectedMap))
        {
            RefreshPinnedQuests();
        }
    }

    private void RefreshPinnedQuests()
    {
        _pinnedQuests.Clear();
        var currentMap = _mapViewModel?.SelectedMap ?? string.Empty;
        
        foreach (var quest in _viewModel.FilteredQuests)
        {
            if (quest.IsPinned)
            {
                if (string.IsNullOrEmpty(quest.Map) || quest.Map == currentMap)
                {
                    _pinnedQuests.Add(quest);
                }
            }
        }
    }

    private void QuestsOverlayWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.Close();
            e.Handled = true;
        }
    }

    private void QuestsOverlayWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _lastMousePosition = e.GetPosition(null);
    }

    private void QuestsOverlayWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        var currentPosition = e.GetPosition(null);
        var delta = new System.Windows.Point(
            currentPosition.X - _lastMousePosition.X,
            currentPosition.Y - _lastMousePosition.Y
        );

        this.Left += delta.X;
        this.Top += delta.Y;
        _lastMousePosition = currentPosition;
    }

    private void QuestsOverlayWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
    }
}
