using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Pokedex.View.Converters
{
    class PocketColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Pocket))
                throw new ArgumentException("Value is not of type PokemonType");
            var objectPocket = (Pocket)value;
            SolidColorBrush colour;
            switch (objectPocket)
            {
            case Pocket.Items:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xC8, 0x68, 0x90));
                break;
            case Pocket.Medicine:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xE0, 0x70, 0x38));
                break;
            case Pocket.Berries:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x20, 0xA0, 0x48));
                break;
            case Pocket.Key:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x90, 0x48, 0xD8));
                break;
            case Pocket.TM:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x80, 0xA8, 0x20));
                break;
            case Pocket.ZCrystal:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xB4, 0x8A, 0x4B));
                break;
            default:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xFF, 0xFF, 0xFF));
                break;
            }
            switch (parameter as string)
            {
            case "Lighter":
                colour = ColourModifier.ChangeColorBrightness(colour, .5);
                break;
            case "Darker":
                colour = ColourModifier.ChangeColorBrightness(colour, -.5);
                break;
            default:
                break;
            }
            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
