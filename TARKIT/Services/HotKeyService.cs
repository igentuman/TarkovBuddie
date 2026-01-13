using System.Runtime.InteropServices;
using TARKIT.Models;

namespace TARKIT.Services;

public class HotKeyService
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_ALT = 1;
    private const uint MOD_CONTROL = 2;
    private const uint MOD_SHIFT = 4;
    private const uint MOD_WIN = 8;

    private const int WM_HOTKEY = 0x0312;

    private Dictionary<string, HotKeyBinding> _registeredHotKeys = new();
    private int _hotKeyId = 1;

    public HotKeyService()
    {
    }

    public bool RegisterHotKey(IntPtr windowHandle, string actionName, uint virtualKey, uint modifiers)
    {
        int id = _hotKeyId++;
        bool success = RegisterHotKey(windowHandle, id, modifiers, virtualKey);
        
        if (success)
        {
            _registeredHotKeys[actionName] = new HotKeyBinding
            {
                Action = actionName,
                VirtualKey = virtualKey,
                Modifiers = modifiers
            };
        }
        
        return success;
    }

    public bool UnregisterHotKey(IntPtr windowHandle, string actionName)
    {
        if (!_registeredHotKeys.ContainsKey(actionName))
            return false;

        var binding = _registeredHotKeys[actionName];
        int id = _registeredHotKeys.Keys.ToList().IndexOf(actionName) + 1;
        
        bool success = UnregisterHotKey(windowHandle, id);
        
        if (success)
        {
            _registeredHotKeys.Remove(actionName);
        }
        
        return success;
    }

    public Dictionary<string, HotKeyBinding> GetRegisteredHotKeys()
    {
        return new Dictionary<string, HotKeyBinding>(_registeredHotKeys);
    }

    public void ClearAllHotKeys(IntPtr windowHandle)
    {
        var keys = _registeredHotKeys.Keys.ToList();
        foreach (var key in keys)
        {
            UnregisterHotKey(windowHandle, key);
        }
    }
}
