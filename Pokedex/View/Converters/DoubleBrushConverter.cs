using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Pokedex.View.Converters
{
    class DoubleBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is double))
                throw new ArgumentException("Value is not of type double");
            if ((double)value == -1)
                return new SolidColorBrush(Colors.Black);
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(1, 0.5);
            brush.EndPoint = new Point(0, 0.5);
            brush.GradientStops.Add(new GradientStop() { Color = Colors.Pink, Offset = 0 });
            brush.GradientStops.Add(new GradientStop() { Color = Colors.Pink, Offset = (double)value - 0.001 });
            brush.GradientStops.Add(new GradientStop() { Color = Colors.Blue, Offset = (double)value + 0.001 });
            brush.GradientStops.Add(new GradientStop() { Color = Colors.Blue, Offset = 1 });
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
