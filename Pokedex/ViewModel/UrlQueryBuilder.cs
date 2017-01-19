using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Pokedex.Model
{
    public static class UrlQueryBuilder
    {
        private static string GetResource(string key)
        {
            string resource = "";
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //    () => resource = Windows.UI.Xaml.Application.Current.Resources[key] as string);
            resource = Windows.UI.Xaml.Application.Current.Resources[key] as string;
            return resource;
        }
        private static string GetResourceString(ResourceType resourceType)
        {
            string resourceTypeString = "";
            switch (resourceType)
            {
            case ResourceType.Pokemon:
                resourceTypeString = "_(Pokémon)";
                break;
            case ResourceType.Ability:
                resourceTypeString = "_(Ability)";
                break;
            case ResourceType.Miscellaneous:
            default:
                break;
            }
            return resourceTypeString;
        }
        private static string BaseContentQuery(string resource, int section, ResourceType resourceType)
        {
            string resourceTypeString = GetResourceString(resourceType);
            return $"{GetResource("BulbapediaApiBaseUrl")}api.php?action=query&prop=revisions&titles={resource}{resourceTypeString}&rvprop=content&rvsection={section}&format=json";
        }
        private static string BaseParseQuery(string resource, int section, ResourceType resourceType)
        {
            string resourceTypeString = GetResourceString(resourceType);
            return $"{GetResource("BulbapediaApiBaseUrl")}api.php?action=parse&page={resource}{resourceTypeString}&prop=text&section={section}&format=json";
        }
        private static string BaseSectionsQuery(string resource, ResourceType resourceType)
        {
            string resourceTypeString = GetResourceString(resourceType);
            return $"{GetResource("BulbapediaApiBaseUrl")}api.php?action=parse&page={resource}{resourceTypeString}&prop=sections&format=json";
        }
        private static string BaseUrlQuery(string resource, ResourceType resourceType)
        {
            string resourceTypeString = GetResourceString(resourceType);
            return $"{GetResource("BulbapediaBaseUrl")}{resource}{resourceTypeString}";
        }
        public static string BasePictureLocationQuery(string pictureLink)
        {
            return $"{GetResource("BulbapediaApiBaseUrl")}api.php?action=query&titles={pictureLink}&prop=imageinfo&iiprop=url&format=json";
        }
        public static string PokemonUrlQuery(string pokemonName)
        {
            return BaseUrlQuery(pokemonName, ResourceType.Pokemon);
        }
        public static string PokemonContentQuery(string pokemonName, int section)
        {
            return BaseContentQuery(pokemonName, section, ResourceType.Pokemon);
        }
        public static string PokemonParseQuery(string pokemonName, int section)
        {
            return BaseParseQuery(pokemonName, section, ResourceType.Pokemon);
        }
        public static string PokemonListQuery(Generation? generation = null)
        {
            int section;
            switch (generation)
            {
            case Generation.First:
                section = 2;
                break;
            case Generation.Second:
                section = 3;
                break;
            case Generation.Third:
                section = 4;
                break;
            case Generation.Fourth:
                section = 5;
                break;
            case Generation.Fifth:
                section = 6;
                break;
            case Generation.Sixth:
                section = 7;
                break;
            case Generation.Seventh:
                section = 8;
                break;
            default:
                section = 1;
                break;
            }
            return BaseContentQuery("List_of_Pokémon_by_National_Pokédex_number", section, ResourceType.Miscellaneous);
        }
        public static string PokemonSectionsQuery(string pokemonName)
        {
            return BaseSectionsQuery(pokemonName, ResourceType.Pokemon);
        }
    }
}
