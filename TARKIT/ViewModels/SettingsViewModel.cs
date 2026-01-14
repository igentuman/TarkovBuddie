using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TARKIT.Models;
using TARKIT.Services;

namespace TARKIT.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly ExitTrackerService _exitTrackerService;
    private string _selectedLanguage = "English";
    private int _overlayTransparency = 100;
    private string _questDataPath = "";
    private string _itemDataPath = "";
    private string _userDataPath = "";
    private string? _currentBindingAction;
    private bool _isListeningForHotKey;
    private bool _enableExitTracker = false;

    public ObservableCollection<string> AvailableLanguages { get; }
    public ObservableCollection<HotKeyDisplay> HotKeyDisplays { get; }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value))
            {
                _settingsService.SetLanguage(value);
                LocalizationService.Instance.CurrentLanguage = value;
            }
        }
    }

    public int OverlayTransparency
    {
        get => _overlayTransparency;
        set => SetProperty(ref _overlayTransparency, value);
    }

    public string QuestDataPath
    {
        get => _questDataPath;
        set => SetProperty(ref _questDataPath, value);
    }

    public string ItemDataPath
    {
        get => _itemDataPath;
        set => SetProperty(ref _itemDataPath, value);
    }

    public string UserDataPath
    {
        get => _userDataPath;
        set => SetProperty(ref _userDataPath, value);
    }

    public bool IsListeningForHotKey
    {
        get => _isListeningForHotKey;
        set => SetProperty(ref _isListeningForHotKey, value);
    }

    public bool EnableExitTracker
    {
        get => _enableExitTracker;
        set
        {
            if (SetProperty(ref _enableExitTracker, value))
            {
                HandleExitTrackerToggle(value);
            }
        }
    }

    public ICommand BindHotKeyCommand { get; }
    public ICommand ClearHotKeyCommand { get; }
    public ICommand BrowseQuestDataCommand { get; }
    public ICommand BrowseItemDataCommand { get; }
    public ICommand BrowseUserDataCommand { get; }
    public ICommand SaveSettingsCommand { get; }

    public SettingsViewModel()
    {
        _settingsService = SettingsService.Instance;
        _exitTrackerService = ExitTrackerService.Instance;
        
        AvailableLanguages = new ObservableCollection<string> { "English", "Russian" };
        HotKeyDisplays = new ObservableCollection<HotKeyDisplay>
        {
            new() { Action = "Toggle Map Overlay", KeyString = "" },
            new() { Action = "Toggle Pinned Quests", KeyString = "" },
            new() { Action = "Toggle Pinned Items", KeyString = "" }
        };

        BindHotKeyCommand = new RelayCommand(BindHotKey);
        ClearHotKeyCommand = new RelayCommand(ClearHotKey);
        BrowseQuestDataCommand = new RelayCommand(_ => BrowseFolder("Quest Data"));
        BrowseItemDataCommand = new RelayCommand(_ => BrowseFolder("Item Data"));
        BrowseUserDataCommand = new RelayCommand(_ => BrowseFolder("User Data"));
        SaveSettingsCommand = new RelayCommand(_ => SaveSettings());

        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = _settingsService.LoadSettings();
        SelectedLanguage = settings.Language;
        OverlayTransparency = settings.OverlayTransparency;
        QuestDataPath = settings.QuestDataPath;
        ItemDataPath = settings.ItemDataPath;
        UserDataPath = settings.UserDataPath;
        EnableExitTracker = settings.EnableExitTracker;

        UpdateHotKeyDisplays(settings);
        
        if (EnableExitTracker)
        {
            _exitTrackerService.Start();
        }
    }

    private void UpdateHotKeyDisplays(ApplicationSettings settings)
    {
        foreach (var display in HotKeyDisplays)
        {
            if (settings.HotKeys.TryGetValue(display.Action, out var binding))
            {
                display.KeyString = ConvertKeyToString(binding.VirtualKey, binding.Modifiers);
            }
            else
            {
                display.KeyString = "[Not Bound]";
            }
        }
    }

    private void BindHotKey(object? parameter)
    {
        if (parameter is HotKeyDisplay display)
        {
            _currentBindingAction = display.Action;
            IsListeningForHotKey = true;
            System.Diagnostics.Debug.WriteLine($"Now listening for hotkey: {_currentBindingAction}");
        }
    }

    private void ClearHotKey(object? parameter)
    {
        if (parameter is HotKeyDisplay display)
        {
            display.KeyString = "[Not Bound]";
            var settings = _settingsService.GetCurrentSettings();
            settings.HotKeys.Remove(display.Action);
            _settingsService.SaveSettings(settings);
        }
    }

    private void BrowseFolder(string pathType)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = $"Select {pathType} Directory"
        };
        
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            switch (pathType)
            {
                case "Quest Data":
                    QuestDataPath = dialog.SelectedPath;
                    break;
                case "Item Data":
                    ItemDataPath = dialog.SelectedPath;
                    break;
                case "User Data":
                    UserDataPath = dialog.SelectedPath;
                    break;
            }
        }
    }

    private void SaveSettings()
    {
        if (!ValidateAllPaths())
            return;

        var currentSettings = _settingsService.GetCurrentSettings();
        var settings = new ApplicationSettings
        {
            Language = SelectedLanguage,
            OverlayTransparency = OverlayTransparency,
            QuestDataPath = QuestDataPath,
            ItemDataPath = ItemDataPath,
            UserDataPath = UserDataPath,
            HotKeys = currentSettings.HotKeys,
            EnableExitTracker = EnableExitTracker
        };

        _settingsService.SaveSettings(settings);
    }

    private void HandleExitTrackerToggle(bool enabled)
    {
        if (enabled)
        {
            _exitTrackerService.ExitsDetected += OnExitsDetected;
            _exitTrackerService.ErrorOccurred += OnTrackerError;
            _exitTrackerService.Start();
        }
        else
        {
            _exitTrackerService.Stop();
            _exitTrackerService.ExitsDetected -= OnExitsDetected;
            _exitTrackerService.ErrorOccurred -= OnTrackerError;
        }

        var currentSettings = _settingsService.GetCurrentSettings();
        currentSettings.EnableExitTracker = enabled;
        _settingsService.SaveSettings(currentSettings);
    }

    private void OnExitsDetected(List<string> exits)
    {
        System.Diagnostics.Debug.WriteLine($"Exits detected: {string.Join(", ", exits)}");
    }

    private void OnTrackerError(Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Exit tracker error: {ex.Message}");
    }

    private bool ValidateAllPaths()
    {
        if (!string.IsNullOrEmpty(QuestDataPath) && !_settingsService.ValidatePath(QuestDataPath))
            return false;

        if (!string.IsNullOrEmpty(ItemDataPath) && !_settingsService.ValidatePath(ItemDataPath))
            return false;

        if (!string.IsNullOrEmpty(UserDataPath) && !_settingsService.ValidatePath(UserDataPath))
            return false;

        return true;
    }

    public void HandleKeyPress(System.Windows.Input.KeyEventArgs e)
    {
        if (!IsListeningForHotKey || _currentBindingAction == null)
            return;

        var key = e.Key;
        
        if (IsIgnoredKey(key))
        {
            System.Diagnostics.Debug.WriteLine($"Ignored key pressed: {key}");
            return;
        }

        uint virtualKey = (uint)System.Windows.Input.KeyInterop.VirtualKeyFromKey(key);
        uint modifiers = GetModifiers(e);

        System.Diagnostics.Debug.WriteLine($"Key pressed: {key}, VirtualKey: {virtualKey}, Modifiers: {modifiers}");

        if (!IsValidHotKeyBinding(virtualKey, modifiers))
        {
            System.Diagnostics.Debug.WriteLine($"Invalid hotkey binding: VirtualKey={virtualKey}, Modifiers={modifiers}");
            return;
        }

        IsListeningForHotKey = false;

        var settings = _settingsService.GetCurrentSettings();
        settings.HotKeys[_currentBindingAction] = new HotKeyBinding
        {
            Action = _currentBindingAction,
            VirtualKey = virtualKey,
            Modifiers = modifiers
        };

        var display = HotKeyDisplays.FirstOrDefault(x => x.Action == _currentBindingAction);
        if (display != null)
        {
            display.KeyString = ConvertKeyToString(virtualKey, modifiers);
            System.Diagnostics.Debug.WriteLine($"Hotkey bound: {_currentBindingAction} = {display.KeyString}");
            _settingsService.SaveSettings(settings);
        }

        _currentBindingAction = null;
        e.Handled = true;
    }

    private bool IsIgnoredKey(Key key)
    {
        return key == Key.LeftCtrl || key == Key.RightCtrl ||
               key == Key.LeftAlt || key == Key.RightAlt ||
               key == Key.LeftShift || key == Key.RightShift ||
               key == Key.LWin || key == Key.RWin ||
               key == Key.System || key == Key.Escape;
    }

    private uint GetModifiers(System.Windows.Input.KeyEventArgs e)
    {
        uint modifiers = 0;

        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) > 0)
            modifiers |= 2;
        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) > 0)
            modifiers |= 1;
        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) > 0)
            modifiers |= 4;
        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Windows) > 0)
            modifiers |= 8;

        return modifiers;
    }

    private bool IsValidHotKeyBinding(uint virtualKey, uint modifiers)
    {
        if (virtualKey == 0)
            return false;

        if (modifiers == 0)
        {
            return false;
        }

        return true;
    }

    private string ConvertKeyToString(uint virtualKey, uint modifiers)
    {
        var key = System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)virtualKey);
        string keyString = key.ToString();

        string modString = "";
        if ((modifiers & 2) > 0) modString += "Ctrl+";
        if ((modifiers & 1) > 0) modString += "Alt+";
        if ((modifiers & 4) > 0) modString += "Shift+";
        if ((modifiers & 8) > 0) modString += "Win+";

        return modString + keyString;
    }
}

public class HotKeyDisplay : INotifyPropertyChanged
{
    private string _action = "";
    private string _keyString = "[Not Bound]";

    public string Action
    {
        get => _action;
        set
        {
            if (_action != value)
            {
                _action = value;
                OnPropertyChanged();
            }
        }
    }

    public string KeyString
    {
        get => _keyString;
        set
        {
            if (_keyString != value)
            {
                _keyString = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
