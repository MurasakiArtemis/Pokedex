using Pokedex.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using Pokedex.Communication;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using Pokedex.View.Converters;
using System;
using Pokedex.Model.Wrappers;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Pokedex.ViewModel
{
    public class PokemonVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate
        {
        };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    RaiseProperty();
                }
            }
        }

        private Pokemon _currentPokemon;
        public Pokemon CurrentPokemon
        {
            get { return _currentPokemon; }
            set
            {
                if (value != _currentPokemon)
                {
                    _currentPokemon = value;
                    RaiseProperty();
                }
            }
        }

        public PokemonVM(string pokemonName)
        {
            IsBusy = true;
            HttpCommunication source = new HttpCommunication();
            int section = 0;
            string pokemonBaseData = source.GetResponse(Model.UrlQueryBuilder.PokemonContentQuery(pokemonName, section)).Result;
            CurrentPokemon = GetPokemonFromJson(pokemonBaseData);
            IsBusy = false;
        }

        private Pokemon GetPokemonFromJson(string data)
        {
            string[] pokemonData = ExtractJsonData(data);
            Func<string, string> extractionFunction = (attributeName) =>
            {
                var pokemonDataSelection = pokemonData.Where(p => p.Contains($"{attributeName}="));
                if (pokemonDataSelection.Count() > 0)
                    return pokemonDataSelection.ElementAt(0).Replace($"{attributeName}=", "");
                return "";
            };
            var NationalDex = int.Parse(extractionFunction("ndex"));
            var Name = extractionFunction("name");
            var JapaneseName = extractionFunction("jname");
            var JapaneseTransliteration = extractionFunction("jtranslit");
            var JapaneseRomanizedName = extractionFunction("tmname");
            var RegionsAndNumbers = ExtractRegionsAndNumbers(extractionFunction);
            var Category = extractionFunction("category");
            int typesNumber;
            int formsNumber;
            if (!int.TryParse(extractionFunction("forme"), out formsNumber))
                formsNumber = 1;
            if (!int.TryParse(extractionFunction("typebox"), out typesNumber))
                typesNumber = 2;
            var Forms = ExtractForms(extractionFunction, typesNumber, formsNumber, Name, NationalDex);
            bool hasMega;
            var MegaStones = ExtractMegaStones(extractionFunction, formsNumber, out hasMega);
            int eggGroupNumber = int.Parse(extractionFunction("egggroupn"));
            var EggGroups = ExtractEggGroups(extractionFunction, eggGroupNumber);
            var HatchTime = int.Parse(extractionFunction("eggcycles")) * 257;
            var ExperienceYield = int.Parse(extractionFunction("expyield"));
            var GenderCode = double.Parse(extractionFunction("gendercode")) / 254;
            var CatchRate = int.Parse(extractionFunction("catchrate"));
            var IntroducedGeneration = (Generation)Enum.GetValues(typeof(Generation)).GetValue(int.Parse(extractionFunction("generation")) - 1);
            var BaseFriendship = int.Parse(extractionFunction("friendship"));
            var URL = UrlQueryBuilder.PokemonUrlQuery(Name);
            return new Pokemon() { NationalDex = NationalDex, Name = Name, JapaneseName = JapaneseName, JapaneseTransliteration = JapaneseTransliteration,
                JapaneseRomanizedName = JapaneseRomanizedName, RegionsAndNumbers = RegionsAndNumbers, Category = Category, Forms = Forms, HasMega = hasMega,
                MegaStones = MegaStones, EggGroups = EggGroups, HatchTime = HatchTime, ExperienceYield = ExperienceYield, GenderCode = GenderCode,
                CatchRate = CatchRate, IntroducedGeneration = IntroducedGeneration, BaseFriendship = BaseFriendship, URL = URL};
        }
        private ObservableCollection<Form> ExtractForms(Func<string, string> extractionFunction, int typesAmount, int formsAmount, string name, int nationalDex)
        {
            var forms = new ObservableCollection<Form>();
            for (int i = 1; i <= formsAmount; i++)
            {
                var temporalNameResult = ExtractFormName(extractionFunction, i, name);
                var temporalImageRelativeLinkResult = ExtractFormPicture(extractionFunction, i, name, nationalDex);
                var converter = new StringImageSourceConverter();
                var temporalImageResult = (BitmapImage)converter.Convert(temporalImageRelativeLinkResult, typeof(BitmapImage), null, null);
                var heightResult = ExtractFormHeight(extractionFunction, i, ref forms);
                var weightResult = ExtractFormWeight(extractionFunction, i, ref forms);
                var Types = ExtractFormTypes(extractionFunction, typesAmount, i, ref forms);
                var Abilities = ExtractFormAbilities(extractionFunction, i, temporalNameResult, ref forms);
                var formResult = new Form() { Name = temporalNameResult, ImageRelativeLink = temporalImageRelativeLinkResult, Image = temporalImageResult,
                    Types = Types, Height = heightResult, Weight = weightResult, Abilities = Abilities };
                forms.Add(formResult);
            }
            return forms;
        }
        private string ExtractFormName(Func<string, string> extractionFunction, int formNumber, string name)
        {
            string temporalFormResult = extractionFunction($"form{formNumber}");
            if (string.IsNullOrEmpty(temporalFormResult))
                temporalFormResult = name;
            return temporalFormResult;
        }
        private string ExtractFormPicture(Func<string, string> extractionFunction, int formNumber, string name, int nationalDex)
        {
            string temporalPictureResult = extractionFunction($"image{formNumber}");
            if (string.IsNullOrEmpty(temporalPictureResult))
                temporalPictureResult = $"{nationalDex}{name}.png";
            return $"File:{temporalPictureResult}";
        }
        private ObservableCollection<SlotType> ExtractFormTypes(Func<string, string> extractionFunction, int typesAmount, int formNumber, ref ObservableCollection<Form> Forms)
        {
            var types = new ObservableCollection<SlotType>();
            for (int j = 1; j <= typesAmount; j++)
            {
                string temporalTypeResult = extractionFunction($"{(formNumber != 1 ? $"form{formNumber}" : "")}type{j}");
                if (string.IsNullOrEmpty(temporalTypeResult))
                    temporalTypeResult = "None";
                PokemonType result = (PokemonType)Enum.Parse(typeof(PokemonType), temporalTypeResult);
                if (formNumber != 1 && result == PokemonType.None)
                    result = Forms.First().Types.ElementAt(j - 1).Type;
                types.Add(new SlotType() { Slot = j == 1 ? TypeSlot.Primary : TypeSlot.Secondary, Type = result });
            }
            return types;
        }
        private double ExtractFormWeight(Func<string, string> extractionFunction, int formNumber, ref ObservableCollection<Form> Forms)
        {
            double weightResult;
            string temporalWeightResult = extractionFunction($"weight-kg{(formNumber != 1 ? $"{formNumber}" : "")}");
            if (string.IsNullOrEmpty(temporalWeightResult))
                temporalWeightResult = "0.0";
            weightResult = double.Parse(temporalWeightResult);
            if (formNumber != 1 && weightResult == 0.0)
                weightResult = Forms.First().Weight;
            return weightResult;
        }
        private double ExtractFormHeight(Func<string, string> extractionFunction, int formNumber, ref ObservableCollection<Form> Forms)
        {
            double heightResult;
            string temporalHeightResult = extractionFunction($"height-m{(formNumber != 1 ? $"{formNumber}" : "")}");
            if (string.IsNullOrEmpty(temporalHeightResult))
                temporalHeightResult = "0.0";
            heightResult = double.Parse(temporalHeightResult);
            if (formNumber != 1 && heightResult == 0.0)
                heightResult = Forms.First().Height;
            return heightResult;
        }
        private ObservableCollection<AbilityName> ExtractFormAbilities(Func<string, string> extractionFunction, int formNumber, string temporalFormResult, ref ObservableCollection<Form> Forms)
        {
            string temporalAbilityResult;
            var Abilities = new ObservableCollection<AbilityName>();
            if (temporalFormResult.Contains("Mega"))
            {
                temporalAbilityResult = extractionFunction("abilitym");
                if (!string.IsNullOrEmpty(temporalAbilityResult))
                    Abilities.Add(new AbilityName() { AbilitySlot = AbilitySlot.Mega, Name = temporalAbilityResult });
            }
            else
            {
                foreach (AbilitySlot abilitySlot in Enum.GetValues(typeof(AbilitySlot)))
                {
                    switch (abilitySlot)
                    {
                    case AbilitySlot.First:
                        temporalAbilityResult = extractionFunction($"ability{(formNumber != 1 ? $"{formNumber}-" : "")}1");
                        break;
                    case AbilitySlot.Second:
                        temporalAbilityResult = extractionFunction($"ability{(formNumber != 1 ? $"{formNumber}-" : "")}2");
                        break;
                    case AbilitySlot.Hidden:
                        temporalAbilityResult = extractionFunction($"abilityd{(formNumber != 1 ? $"{formNumber}" : "")}");
                        break;
                    default:
                        temporalAbilityResult = "";
                        break;
                    }
                    if (formNumber != 1 && string.IsNullOrEmpty(temporalAbilityResult))
                        temporalAbilityResult = Forms.First().Abilities.Where(p => p.AbilitySlot == abilitySlot).First().Name;
                    if (!string.IsNullOrEmpty(temporalAbilityResult))
                        Abilities.Add(new AbilityName() { AbilitySlot = abilitySlot, Name = temporalAbilityResult });
                }
            }
            return Abilities;
        }
        private ObservableCollection<MegaStonePicture> ExtractMegaStones(Func<string, string> extractionFunction, int formsNumber, out bool hasMega)
        {
            var megaStones = new ObservableCollection<MegaStonePicture>();
            hasMega = false;
            for (int i = 1; i <= formsNumber; i++)
            {
                string megaStoneName = extractionFunction($"mega{(i != 1 ? $"{i}" : "")}");
                if (!string.IsNullOrEmpty(megaStoneName))
                {
                    hasMega = true;
                    var converter = new StringImageSourceConverter();
                    var temporalImageRelativeLinkResult = $"File:Bag {megaStoneName} Sprite.png";
                    var temporalImageResult = (BitmapImage)converter.Convert(temporalImageRelativeLinkResult, typeof(BitmapImage), null, null);
                    MegaStonePicture megaStoneResult = new MegaStonePicture() { Name = megaStoneName,
                        ImageRelativeLink = temporalImageRelativeLinkResult, Image = temporalImageResult};
                    megaStones.Add(megaStoneResult);
                }
                else break;
            }
            return megaStones;
        }
        private ObservableCollection<string> ExtractEggGroups(Func<string, string> extractionFunction, int eggGroupNumber)
        {
            var eggGroups = new ObservableCollection<string>();
            for (int i = 1; i <= eggGroupNumber; i++)
                eggGroups.Add(extractionFunction($"egggroup{i}"));
            return eggGroups;
        }
        private ObservableCollection<RegionNumber> ExtractRegionsAndNumbers(Func<string, string> extractionFunction)
        {
            var regionsAndNumbers = new ObservableCollection<RegionNumber>();
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
                    regionsAndNumbers.Add(new RegionNumber() { Region = region, RegionalDex = int.Parse(temporalResult) });
            }
            return regionsAndNumbers;
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

    }
}
