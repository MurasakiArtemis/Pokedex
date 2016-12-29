using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Model
{
    public static class UrlQueryBuilder
    {
        public static string PokemonUrlQuery(string pokemonName)
        {
            return $"{Windows.UI.Xaml.Application.Current.Resources["BulbapediaBaseUrl"] as string}{pokemonName} (Pokémon)";
        }
        public static string PokemonContentQuery(string pokemonName, int section)
        {
            return $"{Windows.UI.Xaml.Application.Current.Resources["BulbapediaApiBaseUrl"] as string}api.php?action=query&prop=revisions&titles={pokemonName} (Pokémon)&rvprop=content&rvsection={section}&format=json";
        }
        public static string PokemonSectionsQuery(string pokemonName)
        {
            return $"{Windows.UI.Xaml.Application.Current.Resources["BulbapediaApiBaseUrl"] as string}api.php?action=parse&page={pokemonName} (Pokémon)&prop=sections&format=json";
        }
        public static string PictureLocationQuery(string pictureLink)
        {
            return $"{Windows.UI.Xaml.Application.Current.Resources["BulbapediaApiBaseUrl"] as string}api.php?action=query&titles={pictureLink}&prop=imageinfo&iiprop=url&format=json";
        }
        public static string PictureLocation(string data)
        {
            JObject jsonData = JObject.Parse(data);
            JToken result = jsonData["query"]["pages"].ElementAt(0).First["imageinfo"].First["url"];
            string semiparsedData = result.ToString(Formatting.None).Replace("\"", "");
            return semiparsedData;
        }
    }
}
