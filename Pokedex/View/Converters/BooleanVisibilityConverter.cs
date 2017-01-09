using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pokedex.View.Converters
{
    class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
                throw new ArgumentException("Value is not of type string");
            var option = "";
            var visibility = (bool)value;
            if (string.IsNullOrEmpty(parameter as string))
                option = "Forward";
            else
                option = parameter as string;
            switch(option)
            {
            case "Reverse":
                visibility = !visibility;
                break;
            case "Forward":
            default:
                break;
            }
            if (visibility)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
