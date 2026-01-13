using System.Windows.Input;
using TARKIT.ViewModels;

namespace TARKIT.Views;

public partial class SettingsView
{
    private SettingsViewModel? _viewModel;

    public SettingsView()
    {
        InitializeComponent();
        _viewModel = new SettingsViewModel();
        DataContext = _viewModel;
    }

    private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (_viewModel?.IsListeningForHotKey == true)
        {
            _viewModel.HandleKeyPress(e);
        }
    }
}
