using System.Configuration;
using System.Data;
using TARKIT.Services;

namespace TARKIT;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        var locService = LocalizationService.Instance;
        base.OnStartup(e);
    }

    protected override void OnExit(System.Windows.ExitEventArgs e)
    {
        ExitTrackerService.Instance?.Dispose();
        base.OnExit(e);
    }
}