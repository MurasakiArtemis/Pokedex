using Pokedex.Model;
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
            if (!((value is double) || (value is int) || (value is string) || (value is Region)))
                throw new ArgumentException("Value is not of accepted type");
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
                    if (number == -1)
                        text = $"Genderless";
                    else if (number == 1)
                        text = $"100% Female";
                    else if(number == 0)
                        text = $"100% Male";
                    else
                    {
                        for (double i = 0.125; i < 1; i += 0.125)
                        {
                            if (number <= i)
                            {
                                number = i;
                                break;
                            }
                        }
                        text = $"{(1 - number) * 100}% Male, {number * 100}% Female";
                    }
                }
                break;
            case "PokemonNumber":
                text = $"# {(int)value}";
                break;
            case "PokemonListNumber":
                text = ((int)value).ToString("D3");
                break;
            case "MegaStoneTooltip":
                text = "Check information from " + (string)value;
                break;
            case "Region":
                switch ((Region)value)
                {
                case Region.Kanto:
                case Region.Johto:
                case Region.Hoenn:
                case Region.Sinnoh:
                case Region.Alola:
                    text = ((Region)value).ToString();
                    break;
                case Region.Unova:
                case Region.NewUnova:
                    text = "Unova";
                    break;
                case Region.CentralKalos:
                case Region.CoastalKalos:
                case Region.MountainKalos:
                    text = "Kalos";
                    break;
                default:
                    text = "";
                    break;
                }
                break;
            case "RegionTooltip":
                switch ((Region)value)
                {
                case Region.NewUnova:
                    text = "Black 2 and White 2";
                    break;
                case Region.CentralKalos:
                    text = "Central";
                    break;
                case Region.CoastalKalos:
                    text = "Coastal";
                    break;
                case Region.MountainKalos:
                    text = "Mountain";
                    break;
                case Region.Kanto:
                case Region.Johto:
                case Region.Hoenn:
                case Region.Sinnoh:
                case Region.Unova:
                case Region.Alola:
                default:
                    text = null;
                    break;
                }
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
