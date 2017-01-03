using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Pokedex.View.Converters
{
    class StringFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!((value is double) || (value is int)))
                throw new ArgumentException("Value is not of type double");
            string text;
            switch(parameter as string)
            {
            case "Metres":
                text = $"{(double)value} m";
                break;
            case "Kilograms":
                text = $"{(double)value} Kg";
                break;
            case "GenderPercent":
                {
                    var number = (double)value;
                    for(double i = 0.125; i < 1; i += 0.125)
                    {
                        if(number < i)
                        {
                            number = i;
                            break;
                        }
                    }
                    text = $"{(1 - number) * 100}% Male, {number * 100}% Female";
                }
                break;
            case "PokemonNumber":
                text = $"# {(int)value}";
                break;
            case "PokemonListNumber":
                text = ((int)value).ToString("D3");
                break;
            default:
                text = $"{(double)value}";
                break;
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
