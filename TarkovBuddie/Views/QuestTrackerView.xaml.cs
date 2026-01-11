using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TarkovBuddie.Views;

public partial class QuestTrackerView
{
    public QuestTrackerView()
    {
        InitializeComponent();
    }

    private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!e.Handled)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                Source = sender
            };
            var parent = (UIElement)((DataGrid)sender).Parent;
            parent.RaiseEvent(eventArg);
        }
    }
}
