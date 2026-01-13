using System.Runtime.InteropServices;
using TARKIT.Models;

namespace TARKIT.Services;

public class HotKeyManager
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_ALT = 1;
    private const uint MOD_CONTROL = 2;
    private const uint MOD_SHIFT = 4;
    private const uint MOD_WIN = 8;

    private Dictionary<string, (int id, HotKeyBinding binding)> _registeredHotKeys = new();
    private int _nextHotKeyId = 1;
    private IntPtr _windowHandle;

    public event Action<string>? HotKeyPressed;

    public HotKeyManager(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;
    }

    public bool RegisterHotKey(string actionName, HotKeyBinding binding)
    {
        if (string.IsNullOrEmpty(actionName) || binding.VirtualKey == 0)
            return false;

        if (_registeredHotKeys.ContainsKey(actionName))
        {
            UnregisterHotKey(actionName);
        }

        int id = _nextHotKeyId++;
        bool success = RegisterHotKey(_windowHandle, id, binding.Modifiers, binding.VirtualKey);

        if (success)
        {
            _registeredHotKeys[actionName] = (id, binding);
        }
        else
        {
            _nextHotKeyId--;
        }

        return success;
    }

    public bool UnregisterHotKey(string actionName)
    {
        if (!_registeredHotKeys.TryGetValue(actionName, out var entry))
            return false;

        bool success = UnregisterHotKey(_windowHandle, entry.id);

        if (success)
        {
            _registeredHotKeys.Remove(actionName);
        }

        return success;
    }

    public void UnregisterAllHotKeys()
    {
        var keys = _registeredHotKeys.Keys.ToList();
        foreach (var key in keys)
        {
            UnregisterHotKey(key);
        }
    }

    public void HandleWmHotKey(int hotKeyId)
    {
        var entry = _registeredHotKeys.FirstOrDefault(x => x.Value.id == hotKeyId);
        if (!string.IsNullOrEmpty(entry.Key))
        {
            HotKeyPressed?.Invoke(entry.Key);
        }
    }

    public Dictionary<string, HotKeyBinding> GetRegisteredHotKeys()
    {
        return _registeredHotKeys.ToDictionary(x => x.Key, x => x.Value.binding);
    }

    public bool IsHotKeyRegistered(string actionName)
    {
        return _registeredHotKeys.ContainsKey(actionName);
    }
}
