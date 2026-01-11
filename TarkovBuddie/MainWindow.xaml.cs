using System.Windows;
using TarkovBuddie.ViewModels;

namespace TarkovBuddie;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}