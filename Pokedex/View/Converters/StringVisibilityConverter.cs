using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pokedex.View.Converters
{
    class StringVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is string))
                throw new ArgumentException("Value is not of type string");
            if (string.IsNullOrEmpty(value as string))
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
