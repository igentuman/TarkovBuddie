namespace TARKIT.Models;

public class HotKeyBinding
{
    public string Action { get; set; } = "";
    public uint VirtualKey { get; set; }
    public uint Modifiers { get; set; }
}
