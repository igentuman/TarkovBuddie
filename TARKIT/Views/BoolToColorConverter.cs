using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TARKIT.Views;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            if (boolValue)
            {
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFE7F1");
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush(Colors.White);
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
