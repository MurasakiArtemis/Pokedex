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
            return semiparsedData.Replace("\"", "").Replace("\\n", "");
        }
        public static string ExtractPictureUrl(string data)
        {
            JObject jsonData = JObject.Parse(data);
            JToken result = jsonData["query"]["pages"].ElementAt(0).First["imageinfo"].First["url"];
            string semiparsedData = result.ToString(Formatting.None).Replace("\"", "");
            return semiparsedData;
        }
    }
}
