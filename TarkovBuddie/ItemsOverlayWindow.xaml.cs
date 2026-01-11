using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TarkovBuddie.ViewModels;

namespace TarkovBuddie;

public partial class ItemsOverlayWindow : Window
{
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int GWL_EXSTYLE = -20;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    private readonly ItemsTrackerViewModel _viewModel;
    private ObservableCollection<ItemViewModel> _pinnedItems = new();
    private bool _isDragging;
    private System.Windows.Point _lastMousePosition;

    public ObservableCollection<ItemViewModel> PinnedItems => _pinnedItems;

    public ItemsOverlayWindow(ItemsTrackerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = this;
        
        this.KeyDown += ItemsOverlayWindow_KeyDown;
        this.Closed += ItemsOverlayWindow_Closed;
        this.Loaded += ItemsOverlayWindow_Loaded;
        this.MouseLeftButtonDown += ItemsOverlayWindow_MouseLeftButtonDown;
        this.MouseMove += ItemsOverlayWindow_MouseMove;
        this.MouseLeftButtonUp += ItemsOverlayWindow_MouseLeftButtonUp;
    }

    private void ItemsOverlayWindow_Loaded(object sender, RoutedEventArgs e)
    {
        EnableClickThrough();
        RefreshPinnedItems();
        if (_viewModel.FilteredItems is INotifyCollectionChanged notifyCollection)
        {
            notifyCollection.CollectionChanged += FilteredItems_CollectionChanged;
        }
        
        foreach (var item in _viewModel.FilteredItems)
        {
            if (item is INotifyPropertyChanged notifyProperty)
            {
                notifyProperty.PropertyChanged += Item_PropertyChanged;
            }
        }
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

    private void ItemsOverlayWindow_Closed(object? sender, System.EventArgs e)
    {
        if (_viewModel.FilteredItems is INotifyCollectionChanged notifyCollection)
        {
            notifyCollection.CollectionChanged -= FilteredItems_CollectionChanged;
        }
        
        foreach (var item in _viewModel.FilteredItems)
        {
            if (item is INotifyPropertyChanged notifyProperty)
            {
                notifyProperty.PropertyChanged -= Item_PropertyChanged;
            }
        }
    }

    private void FilteredItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems.OfType<ItemViewModel>())
            {
                if (item is INotifyPropertyChanged notifyProperty)
                {
                    notifyProperty.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems.OfType<ItemViewModel>())
            {
                if (item is INotifyPropertyChanged notifyProperty)
                {
                    notifyProperty.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        RefreshPinnedItems();
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemViewModel.IsPinned))
        {
            RefreshPinnedItems();
        }
    }

    private void RefreshPinnedItems()
    {
        _pinnedItems.Clear();
        
        foreach (var item in _viewModel.FilteredItems)
        {
            if (item.IsPinned)
            {
                _pinnedItems.Add(item);
            }
        }
    }

    private void ItemsOverlayWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.Close();
            e.Handled = true;
        }
    }

    private void ItemsOverlayWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _lastMousePosition = e.GetPosition(null);
    }

    private void ItemsOverlayWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

    private void ItemsOverlayWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
    }
}
