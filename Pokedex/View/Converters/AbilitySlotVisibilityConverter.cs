using Pokedex.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pokedex.View.Converters
{
    class AbilitySlotVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is AbilitySlot))
                throw new ArgumentException("Value is not of type AbilitySlot");
            AbilitySlot slot = (AbilitySlot)value;
            switch (slot)
            {
            case AbilitySlot.First:
            case AbilitySlot.Second:
            case AbilitySlot.Mega:
                return Visibility.Collapsed;
            case AbilitySlot.Hidden:
                return Visibility.Visible;
            default:
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
