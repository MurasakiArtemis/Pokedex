using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Pokedex.View.Converters
{
    //[ValueConversion(typeof(string), typeof(Color))]
    class TypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is ObservableCollection<PokemonType>))
                throw new ArgumentException("Value is not of type ObservableCollection<PokemonType>");
            var pokemonType = ((ObservableCollection<PokemonType>)value).ElementAt(0);
            Brush colour;
            switch (pokemonType)
            {
            case PokemonType.Normal:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xA8, 0xA8, 0x78));
                break;
            case PokemonType.Fire:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xF0, 0x80, 0x30));
                break;
            case PokemonType.Fighting:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xC0, 0x30, 0x28));
                break;
            case PokemonType.Water:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x68, 0x90, 0xF0));
                break;
            case PokemonType.Flying:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xA8, 0x90, 0xF0));
                break;
            case PokemonType.Grass:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x78, 0xC8, 0x50));
                break;
            case PokemonType.Poison:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xA0, 0x40, 0xA0));
                break;
            case PokemonType.Electric:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xF8, 0xD0, 0x30));
                break;
            case PokemonType.Ground:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xE0, 0xC0, 0x68));
                break;
            case PokemonType.Psychic:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xF8, 0x58, 0x88));
                break;
            case PokemonType.Rock:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xB8, 0xA0, 0x38));
                break;
            case PokemonType.Ice:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x98, 0xD8, 0xD8));
                break;
            case PokemonType.Bug:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xA8, 0xB8, 0x20));
                break;
            case PokemonType.Dragon:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x20, 0x28, 0xF8));
                break;
            case PokemonType.Ghost:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x70, 0x58, 0x98));
                break;
            case PokemonType.Dark:
                colour = new SolidColorBrush(Color.FromArgb(255, 0x70, 0x58, 0x48));
                break;
            case PokemonType.Steel:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xB8, 0xB8, 0xD0));
                break;
            case PokemonType.Fairy:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xEE, 0x99, 0xAC));
                break;
            default:
                colour = new SolidColorBrush(Color.FromArgb(255, 0xFF, 0xFF, 0xFF));
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
