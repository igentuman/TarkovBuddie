using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using TARKIT.Services;
using TARKIT.ViewModels;

namespace TARKIT;

public partial class MainWindow : Window
{
    private const int WM_HOTKEY = 0x0312;
    private HotKeyManager? _hotKeyManager;
    private MainWindowViewModel? _viewModel;
    private MapOverlayWindow? _mapOverlayWindow;
    private ItemsOverlayWindow? _itemsOverlayWindow;
    private QuestsOverlayWindow? _questsOverlayWindow;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var helper = new WindowInteropHelper(this);
        _hotKeyManager = new HotKeyManager(helper.Handle);

        var source = HwndSource.FromHwnd(helper.Handle);
        source?.AddHook(HwndHook);

        var settingsService = SettingsService.Instance;
        settingsService.SettingsChanged += OnSettingsChanged;

        RegisterHotKeysFromSettings();
    }

    private void OnSettingsChanged(TARKIT.Models.ApplicationSettings settings)
    {
        if (_hotKeyManager != null)
        {
            _hotKeyManager.UnregisterHotKey("Toggle Map Overlay");
            _hotKeyManager.UnregisterHotKey("Toggle Pinned Items");
            _hotKeyManager.UnregisterHotKey("Toggle Pinned Quests");
            if (settings.HotKeys.TryGetValue("Toggle Map Overlay", out var binding))
            {
                _hotKeyManager.RegisterHotKey("Toggle Map Overlay", binding);
            }
            if (settings.HotKeys.TryGetValue("Toggle Pinned Items", out var itemsBinding))
            {
                _hotKeyManager.RegisterHotKey("Toggle Pinned Items", itemsBinding);
            }
            if (settings.HotKeys.TryGetValue("Toggle Pinned Quests", out var questsBinding))
            {
                _hotKeyManager.RegisterHotKey("Toggle Pinned Quests", questsBinding);
            }
        }
        UpdateMapOverlayHotKeyDisplay(settings);
    }

    private void RegisterHotKeysFromSettings()
    {
        if (_hotKeyManager == null)
            return;

        var settingsService = SettingsService.Instance;
        var settings = settingsService.LoadSettings();

        foreach (var hotKey in settings.HotKeys)
        {
            _hotKeyManager.RegisterHotKey(hotKey.Key, hotKey.Value);
        }

        _hotKeyManager.HotKeyPressed += OnHotKeyPressed;

        UpdateMapOverlayHotKeyDisplay(settings);
    }

    private void UpdateMapOverlayHotKeyDisplay(TARKIT.Models.ApplicationSettings settings)
    {
        if (settings.HotKeys.TryGetValue("Toggle Map Overlay", out var binding))
        {
            var key = System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)binding.VirtualKey);
            string modString = "";
            if ((binding.Modifiers & 2) > 0) modString += "Ctrl+";
            if ((binding.Modifiers & 1) > 0) modString += "Alt+";
            if ((binding.Modifiers & 4) > 0) modString += "Shift+";
            if ((binding.Modifiers & 8) > 0) modString += "Win+";

            _viewModel?.SetMapOverlayHotKey(modString + key.ToString());
        }
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            _hotKeyManager?.HandleWmHotKey((int)wParam);
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void OnHotKeyPressed(string actionName)
    {
        if (actionName == "Toggle Map Overlay")
        {
            ToggleMapOverlay();
        }
        else if (actionName == "Toggle Pinned Items")
        {
            ToggleItemsOverlay();
        }
        else if (actionName == "Toggle Pinned Quests")
        {
            ToggleQuestsOverlay();
        }
    }

    private void ToggleMapOverlay()
    {
        if (_mapOverlayWindow == null || !_mapOverlayWindow.IsVisible)
        {
            if (_mapOverlayWindow != null)
            {
                _mapOverlayWindow.Close();
            }

            _mapOverlayWindow = new MapOverlayWindow();
            
            if (_viewModel?.MapViewModel != null)
            {
                _mapOverlayWindow.DataContext = _viewModel.MapViewModel;
            }

            _mapOverlayWindow.Show();
        }
        else
        {
            _mapOverlayWindow.Close();
        }
    }

    private void ToggleItemsOverlay()
    {
        if (_itemsOverlayWindow == null || !_itemsOverlayWindow.IsVisible)
        {
            if (_itemsOverlayWindow != null)
            {
                _itemsOverlayWindow.Close();
            }

            if (_viewModel?.ItemsTrackerViewModel != null)
            {
                _itemsOverlayWindow = new ItemsOverlayWindow(_viewModel.ItemsTrackerViewModel);
                _itemsOverlayWindow.Show();
            }
        }
        else
        {
            _itemsOverlayWindow.Close();
        }

        if (_questsOverlayWindow != null && _questsOverlayWindow.IsVisible)
        {
            _questsOverlayWindow.RefreshPositionForItemsOverlay(_itemsOverlayWindow);
        }
    }

    private void ToggleQuestsOverlay()
    {
        if (_questsOverlayWindow == null || !_questsOverlayWindow.IsVisible)
        {
            if (_questsOverlayWindow != null)
            {
                _questsOverlayWindow.Close();
            }

            if (_viewModel?.QuestTrackerViewModel != null)
            {
                _questsOverlayWindow = new QuestsOverlayWindow(_viewModel.QuestTrackerViewModel, _itemsOverlayWindow, _viewModel.MapViewModel);
                _questsOverlayWindow.Show();
                _questsOverlayWindow.RefreshPositionForItemsOverlay(_itemsOverlayWindow);
            }
        }
        else
        {
            _questsOverlayWindow.Close();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _mapOverlayWindow?.Close();
        _itemsOverlayWindow?.Close();
        _questsOverlayWindow?.Close();
        _hotKeyManager?.UnregisterAllHotKeys();
        SettingsService.Instance.SettingsChanged -= OnSettingsChanged;
        base.OnClosed(e);
    }
}