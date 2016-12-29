using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Pokedex.Model
{//Use Observable Collection instead of array
    public class Pokemon : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate
        {
        };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Pokemon()
        {

        }

        public Pokemon(string data)
        {
            string[] pokemonData = ExtractJsonData(data);
            Func<string, string> extractionFunction = (attributeName) =>
            {
                var pokemonDataSelection = pokemonData.Where(p => p.Contains($"{attributeName}="));
                if (pokemonDataSelection.Count() > 0)
                    return pokemonDataSelection.ElementAt(0).Replace($"{attributeName}=", "");
                return "";
            };
            NationalDex = int.Parse(extractionFunction("ndex"));
            Name = extractionFunction("name");
            JapaneseName = extractionFunction("jname");
            JapaneseTransliteration = extractionFunction("jtranslit");
            JapaneseRomanizedName = extractionFunction("tmname");
            int formsNumber;
            if (!int.TryParse(extractionFunction("forme"), out formsNumber))
                formsNumber = 1;
            FormPicturePairs = new ObservableCollection<KeyValuePair<string, string>>();
            ExtractFormsAndPictures(extractionFunction, formsNumber);
            MegaStone = extractionFunction("mega");
            RegionsAndNumbers = new ObservableCollection<KeyValuePair<Region, int>>();
            ExtractRegionsAndNumbers(extractionFunction);
            int typesNumber;
            if (!int.TryParse(extractionFunction("typebox"), out typesNumber))
                typesNumber = 2;
            Type = new ObservableCollection<PokemonType>();
            ExtractPokemonTypes(extractionFunction, formsNumber, typesNumber);
            Category = extractionFunction("category");
            Height = new ObservableCollection<double>();
            Weight = new ObservableCollection<double>();
            ExtractWeightAndHeight(extractionFunction, formsNumber);
            Abilities = new ObservableCollection<KeyValuePair<AbilitySlot, string>>();
            ExtractAbilities(extractionFunction);
            int eggGroupNumber = int.Parse(extractionFunction("egggroupn"));
            EggGroups = new ObservableCollection<string>();
            ExtractEggGroups(extractionFunction, eggGroupNumber);
            EggCycles = int.Parse(extractionFunction("eggcycles"));
            ExperiencieYield = int.Parse(extractionFunction("expyield"));
            GenderCode = int.Parse(extractionFunction("gendercode"));
            CatchRate = int.Parse(extractionFunction("catchrate"));
            IntroducedGeneration = (Generation)Enum.GetValues(typeof(Generation)).GetValue(int.Parse(extractionFunction("generation")) - 1);
            BaseFriendship = int.Parse(extractionFunction("friendship"));
            URL = UrlQueryBuilder.PokemonUrlQuery(Name);
        }

        private void ExtractAbilities(Func<string, string> extractionFunction)
        {
            foreach (AbilitySlot abilitySlot in Enum.GetValues(typeof(AbilitySlot)))
            {
                string temporalResult;
                switch (abilitySlot)
                {
                case AbilitySlot.First:
                    temporalResult = extractionFunction("ability1");
                    break;
                case AbilitySlot.Second:
                    temporalResult = extractionFunction("ability2");
                    break;
                case AbilitySlot.Hidden:
                    temporalResult = extractionFunction("abilityd");
                    break;
                case AbilitySlot.AlternFormFirst:
                    temporalResult = extractionFunction("ability2-1");
                    break;
                case AbilitySlot.AlternFormSecond:
                    temporalResult = extractionFunction("ability2-2");
                    break;
                case AbilitySlot.AlternFormHidden:
                    temporalResult = extractionFunction("abilityd2");
                    break;
                case AbilitySlot.Mega:
                    temporalResult = extractionFunction("abilitym");
                    break;
                default:
                    temporalResult = "";
                    break;
                }
                if (!string.IsNullOrEmpty(temporalResult))
                    Abilities.Add(new KeyValuePair<AbilitySlot, string>(abilitySlot, temporalResult));
            }
        }

        private void ExtractEggGroups(Func<string, string> extractionFunction, int eggGroupNumber)
        {
            for (int i = 1; i <= eggGroupNumber; i++)
            {
                string temporalGroupResult = extractionFunction($"egggroup{i}");
                if (string.IsNullOrEmpty(temporalGroupResult))
                    temporalGroupResult = Name;
                EggGroups.Add(temporalGroupResult);
            }
        }

        private void ExtractWeightAndHeight(Func<string, string> extractionFunction, int formsNumber)
        {
            double baseHeight = 0.0;
            double baseWeight = 0.0;
            for (int i = 1; i <= formsNumber; i++)
            {
                string temporalHeightResult = extractionFunction($"height-m{(i != 1 ? $"{i}" : "")}");
                string temporalWeightResult = extractionFunction($"weight-kg{(i != 1 ? $"{i}" : "")}");
                if (string.IsNullOrEmpty(temporalHeightResult))
                    temporalHeightResult = "0.0";
                if (string.IsNullOrEmpty(temporalWeightResult))
                    temporalWeightResult = "0.0";
                double heightResult = double.Parse(temporalHeightResult);
                double weightResult = double.Parse(temporalWeightResult);
                if (i == 1)
                {
                    baseHeight = heightResult;
                    baseWeight = weightResult;
                }
                if (heightResult == 0.0)
                    heightResult = baseHeight;
                if (weightResult == 0.0)
                    weightResult = baseWeight;
                Height.Add(heightResult);
                Weight.Add(weightResult);
            }
        }

        private void ExtractPokemonTypes(Func<string, string> extractionFunction, int formsNumber, int typesNumber)
        {
            PokemonType[] baseTypes = new PokemonType[typesNumber];
            for (int j = 1; j <= formsNumber; j++)
            {
                for (int i = 1; i <= typesNumber; i++)
                {
                    string temporalTypeResult = extractionFunction($"{(j != 1 ? $"form{j}" : "")}type{i}");
                    if (string.IsNullOrEmpty(temporalTypeResult))
                        temporalTypeResult = "None";
                    PokemonType result = (PokemonType)Enum.Parse(typeof(PokemonType), temporalTypeResult);
                    if (j == 1)
                        baseTypes[i - 1] = result;
                    if (temporalTypeResult == "None")
                        result = baseTypes[i - 1];
                    Type.Add(result);
                }
            }
        }

        private void ExtractRegionsAndNumbers(Func<string, string> extractionFunction)
        {
            foreach (Region region in Enum.GetValues(typeof(Region)))
            {
                string temporalResult;
                switch (region)
                {
                case Region.Kanto:
                    temporalResult = extractionFunction("ndex");
                    break;
                case Region.Johto:
                    temporalResult = extractionFunction("jdex");
                    break;
                case Region.Hoenn:
                    temporalResult = extractionFunction("hdex");
                    break;
                case Region.Sinnoh:
                    temporalResult = extractionFunction("sdex");
                    break;
                case Region.Unova:
                    temporalResult = extractionFunction("udex");
                    break;
                case Region.NewUnova:
                    temporalResult = extractionFunction("u2dex");
                    break;
                case Region.CentralKalos:
                case Region.CoastalKalos:
                case Region.MountainKalos:
                    {
                        string area = extractionFunction("karea");
                        if (area == region.ToString().Replace("Kalos", ""))
                            temporalResult = extractionFunction("kdex");
                        else
                            temporalResult = "";
                    }
                    break;
                case Region.Alola:
                    temporalResult = extractionFunction("adex");
                    break;
                default:
                    temporalResult = "";
                    break;
                }
                if (!string.IsNullOrEmpty(temporalResult))
                    RegionsAndNumbers.Add(new KeyValuePair<Region, int>(region, int.Parse(temporalResult)));
            }
        }

        private void ExtractFormsAndPictures(Func<string, string> extractionFunction, int formsNumber)
        {
            for (int i = 1; i <= formsNumber; i++)
            {
                string temporalFormResult = extractionFunction($"form{i}");
                string temporalPictureResult = extractionFunction($"image{i}");
                if (string.IsNullOrEmpty(temporalFormResult))
                    temporalFormResult = Name;
                if (string.IsNullOrEmpty(temporalPictureResult))
                    temporalPictureResult = $"{NationalDex}{Name}.png";
                FormPicturePairs.Add(new KeyValuePair<string, string>(temporalFormResult, $"File:{temporalPictureResult}"));
            }
        }

        private string[] ExtractJsonData(string data)
        {
            JObject jsonData = JObject.Parse(data);
            JToken result = jsonData["query"]["pages"].ElementAt(0).First["revisions"].Last.Last.Last;
            string semiparsedData = result.ToString(Formatting.None);
            string replacedString = semiparsedData.Replace("\"", "").Replace("\\n", "");
            string pokemonDataString = replacedString.Split(new string[] { "{{", "}}" }, System.StringSplitOptions.RemoveEmptyEntries).Where(p => p.Contains("Infobox")).ElementAt(0);
            string[] pokemonData = pokemonDataString.Split('|');
            return pokemonData;
        }

        private int _nationalDex;
        public int NationalDex
        {
            get { return _nationalDex; }
            set
            {
                if (value != _nationalDex)
                {
                    _nationalDex = value;
                    RaiseProperty();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseName;
        public string JapaneseName
        {
            get { return _japaneseName; }
            set
            {
                if (value != _japaneseName)
                {
                    _japaneseName = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseTransliteration;
        public string JapaneseTransliteration
        {
            get { return _japaneseTransliteration; }
            set
            {
                if (value != _japaneseTransliteration)
                {
                    _japaneseTransliteration = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseRomanizedName;
        public string JapaneseRomanizedName
        {
            get { return _japaneseRomanizedName; }
            set
            {
                if (value != _japaneseRomanizedName)
                {
                    _japaneseRomanizedName = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<KeyValuePair<string, string>> _formPicturePairs;
        public ObservableCollection<KeyValuePair<string, string>> FormPicturePairs
        {
            get { return _formPicturePairs; }
            set
            {
                if (value != _formPicturePairs)
                {
                    _formPicturePairs = value;
                    RaiseProperty();
                }
            }
        }

        private string _megaStone;
        public string MegaStone
        {
            get { return _megaStone; }
            set
            {
                if (value != _megaStone)
                {
                    _megaStone = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<KeyValuePair<Region, int>> _regionsAndNumbers;
        public ObservableCollection<KeyValuePair<Region, int>> RegionsAndNumbers
        {
            get { return _regionsAndNumbers; }
            set
            {
                if (value != _regionsAndNumbers)
                {
                    _regionsAndNumbers = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<PokemonType> _types;
        public ObservableCollection<PokemonType> Type
        {
            get { return _types; }
            set
            {
                if (value != _types)
                {
                    _types = value;
                    RaiseProperty();
                }
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<double> _height;
        public ObservableCollection<double> Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<double> _weight;
        public ObservableCollection<double> Weight
        {
            get { return _weight; }
            set
            {
                if (value != _weight)
                {
                    _weight = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<KeyValuePair<AbilitySlot, string>> _abilities;
        public ObservableCollection<KeyValuePair<AbilitySlot, string>> Abilities
        {
            get { return _abilities; }
            set
            {
                if (value != _abilities)
                {
                    _abilities = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<string> _eggGroups;
        public ObservableCollection<string> EggGroups
        {
            get { return _eggGroups; }
            set
            {
                if (value != _eggGroups)
                {
                    _eggGroups = value;
                    RaiseProperty();
                }
            }
        }

        private int _eggCycles;
        public int EggCycles
        {
            get { return _eggCycles; }
            set
            {
                if (value != _eggCycles)
                {
                    _eggCycles = value;
                    RaiseProperty();
                }
            }
        }

        private int _experienceYield;
        public int ExperiencieYield
        {
            get { return _experienceYield; }
            set
            {
                if (value != _experienceYield)
                {
                    _experienceYield = value;
                    RaiseProperty();
                }
            }
        }

        private int _genderCode;
        public int GenderCode
        {
            get { return _genderCode; }
            set
            {
                if (value != _genderCode)
                {
                    _genderCode = value;
                    RaiseProperty();
                }
            }
        }

        private int _catchRate;
        public int CatchRate
        {
            get { return _catchRate; }
            set
            {
                if (value != _catchRate)
                {
                    _catchRate = value;
                    RaiseProperty();
                }
            }
        }

        private Generation _introducedGeneration;
        public Generation IntroducedGeneration
        {
            get { return _introducedGeneration; }
            set
            {
                if (value != _introducedGeneration)
                {
                    _introducedGeneration = value;
                    RaiseProperty();
                }
            }
        }

        private int _baseFriendship;
        public int BaseFriendship
        {
            get { return _baseFriendship; }
            set
            {
                if (value != _baseFriendship)
                {
                    _baseFriendship = value;
                    RaiseProperty();
                }
            }
        }

        private string url;
        public string URL
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    RaiseProperty();
                }
            }
        }

    }
}
