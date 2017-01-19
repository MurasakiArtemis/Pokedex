using System;
using Windows.UI.Xaml.Data;

namespace Pokedex.View.Converters
{
    class FrameStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is PokemonList))
                throw new ArgumentException("Value is not of type PokemonList");
            string text = "";
            if(value is PokemonList)
                text = "Search a Pokémon";
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
