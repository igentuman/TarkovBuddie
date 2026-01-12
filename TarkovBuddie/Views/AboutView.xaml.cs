using System.Diagnostics;

namespace TarkovBuddie.Views;

public partial class AboutView
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void GitHub_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        OpenUrl("https://github.com/igentuman/TarkovBuddie");
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
