using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.ViewModel
{
    public static class JsonDataExtractor
    {
        public static string ExtractContent(string data)
        {
            JObject jsonData = JObject.Parse(data);
            JToken result = jsonData["query"]["pages"].ElementAt(0).First["revisions"].Last.Last.Last;
            string semiparsedData = result.ToString(Formatting.None);
            return semiparsedData;
        }
        public static string ExtractPictureUrl(string data)
        {
            JObject jsonData = JObject.Parse(data);
            JToken result = jsonData["query"]["pages"].ElementAt(0).First["imageinfo"].First["url"];
            string semiparsedData = result.ToString(Formatting.None).Replace("\"", "");
            return semiparsedData;
        }
        public static List<Section> ExtractSectionList(string data)
        {
            JObject jsonData = JObject.Parse(data);
            var result = jsonData["parse"]["sections"];
            return JsonConvert.DeserializeObject<List<Section>>(result.ToString(Formatting.None));
        }
        public static int ExtractSection(List<Section> sectionsList, Model.PokemonSection sectionToFind)
        {
            int section;
            switch (sectionToFind)
            {
            case Model.PokemonSection.Base:
                section = 0;
                break;
            case Model.PokemonSection.Evolution:
                section = int.Parse(sectionsList.First(p => p.anchor == "Evolution").index);
                break;
            default:
                section = -1;
                break;
            }
            return section;
        }
    }
    public class Section
    {
        public int toclevel { get; set; }
        public string level { get; set; }
        public string line { get; set; }
        public string number { get; set; }
        public string index { get; set; }
        public string fromtitle { get; set; }
        public int byteoffset { get; set; }
        public string anchor { get; set; }
    }
}
