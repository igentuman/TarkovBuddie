using System.Diagnostics;

namespace TARKIT.Views;

public partial class AboutView
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void GitHub_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        OpenUrl("https://github.com/igentuman/TARKIT");
    }

    private void Patreon_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        OpenUrl("https://www.patreon.com/c/igentuman");
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }
}
