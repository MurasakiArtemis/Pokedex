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
using System.Threading.Tasks;

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
            private set
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
        public PokemonVM()
        { }
        public async Task GetPokemon(string pokemonName)
        {
            HttpCommunication source = new HttpCommunication();
            IsBusy = true;
            int section = 0;
            if (pokemonName.StartsWith("Alolan"))
                pokemonName.Remove(0, "Alolan".Length);
            if(pokemonName.StartsWith("Mega"))
                pokemonName.Remove(0, "Mega".Length);
            string pokemonBaseData = await source.GetResponse(Model.UrlQueryBuilder.PokemonContentQuery(pokemonName, section));
            Pokemon pokemon = await GetPokemonFromJson(pokemonBaseData);
            CurrentPokemon = pokemon;
            IsBusy = false;
        }
        private async Task<Pokemon> GetPokemonFromJson(string data)
        {
            string[] pokemonData = ExtractJsonData(data);
            Func<string, string> extractionFunction = (attributeName) =>
            {
                var pokemonDataSelection = pokemonData.Where(p => p.StartsWith($"{attributeName}=")).Select(p => p.Replace($"{attributeName}=", ""));
                if (pokemonDataSelection.Count() > 0)
                {
                    return pokemonDataSelection.First();
                }
                return null;
            };
            var NationalDex = int.Parse(extractionFunction("ndex"));
            var Name = extractionFunction("name");
            int formsNumber;
            if (!int.TryParse(extractionFunction("forme"), out formsNumber))
                formsNumber = 1;
            int typesNumber;
            if (!int.TryParse(extractionFunction("typebox"), out typesNumber))
                typesNumber = 2;
            var taskForms = ExtractForms(extractionFunction, typesNumber, formsNumber, Name, NationalDex);
            var JapaneseName = extractionFunction("jname");
            var JapaneseTransliteration = extractionFunction("jtranslit");
            var JapaneseRomanizedName = extractionFunction("tmname");
            var RegionsAndNumbers = ExtractRegionsAndNumbers(extractionFunction);
            var Category = extractionFunction("category");
            bool hasMega;
            var MegaStones = ExtractMegaStones(extractionFunction, formsNumber, out hasMega);
            int eggGroupNumber = int.Parse(extractionFunction("egggroupn"));
            var EggGroups = ExtractEggGroups(extractionFunction, eggGroupNumber);
            var HatchTime = int.Parse(extractionFunction("eggcycles")) * 257;
            var ExperienceYield = int.Parse(extractionFunction("expyield"));
            var temporalGenderCode = double.Parse(extractionFunction("gendercode"));
            var GenderCode = temporalGenderCode == 255? -1 : temporalGenderCode / 254;
            var CatchRate = int.Parse(extractionFunction("catchrate"));
            var IntroducedGeneration = (Generation)Enum.GetValues(typeof(Generation)).GetValue(int.Parse(extractionFunction("generation")) - 1);
            var BaseFriendship = int.Parse(extractionFunction("friendship"));
            var URL = UrlQueryBuilder.PokemonUrlQuery(Name);
            var Forms = await taskForms;
            return new Pokemon() { NationalDex = NationalDex, Name = Name, JapaneseName = JapaneseName, JapaneseTransliteration = JapaneseTransliteration,
                JapaneseRomanizedName = JapaneseRomanizedName, RegionsAndNumbers = RegionsAndNumbers, Category = Category, Forms = Forms, HasMega = hasMega,
                MegaStones = MegaStones, EggGroups = EggGroups, HatchTime = HatchTime, ExperienceYield = ExperienceYield, GenderCode = GenderCode,
                CatchRate = CatchRate, IntroducedGeneration = IntroducedGeneration, BaseFriendship = BaseFriendship, URL = URL};
        }
        private async Task<ObservableCollection<Form>> ExtractForms(Func<string, string> extractionFunction, int typesAmount, int formsAmount, string name, int nationalDex)
        {
            var forms = new ObservableCollection<Form>();
            for (int i = 1; i <= formsAmount; i++)
            {
                var temporalNameResult = ExtractFormName(extractionFunction, i, name);
                var temporalImageRelativeLinkResult = ExtractFormPicture(extractionFunction, i, name, nationalDex);
                var client = new HttpCommunication();
                var linkData = await client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(temporalImageRelativeLinkResult));
                var dataString = JsonDataExtractor.ExtractPictureUrl(linkData);
                var imageUri = new Uri(dataString);
                var temporalImageResult = new BitmapImage(imageUri);
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
                temporalPictureResult = $"{nationalDex.ToString("D3")}{name}.png";
            return $"File:{temporalPictureResult}";
        }
        private ObservableCollection<SlotType> ExtractFormTypes(Func<string, string> extractionFunction, int typesAmount, int formNumber, ref ObservableCollection<Form> Forms)
        {
            var types = new ObservableCollection<SlotType>();
            bool requiresBase = true;
            for (int j = 1; j <= typesAmount; j++)
            {
                string temporalTypeResult = extractionFunction($"{(formNumber != 1 ? $"form{formNumber}" : "")}type{j}");
                if (string.IsNullOrEmpty(temporalTypeResult))
                    temporalTypeResult = "None";
                PokemonType result = (PokemonType)Enum.Parse(typeof(PokemonType), temporalTypeResult);
                if (j == 1 && result != PokemonType.None)
                    requiresBase = false;
                if (formNumber != 1 && result == PokemonType.None && requiresBase)
                {
                    if(j <= Forms.First().Types.Count)
                        result = Forms.First().Types.ElementAt(j - 1).Type;
                }
                if (result != PokemonType.None)
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
                    {
                        var baseAbility = Forms.First().Abilities.Where(p => p.AbilitySlot == abilitySlot);
                        if(baseAbility.Count() != 0)
                            temporalAbilityResult = baseAbility.Single().Name;
                    }
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
            if(eggGroupNumber == 0)
                eggGroups.Add("Undiscovered");
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
            var replacedString = JsonDataExtractor.ExtractContent(data);
            replacedString = replacedString.Replace("{{tt|", "");           
            string pokemonDataString = replacedString.Split(new string[] { "}}{{", "}}|}", "}}'" }, System.StringSplitOptions.RemoveEmptyEntries).Where(p => p.Contains("Infobox")).Single();
            string[] pokemonData = pokemonDataString.Split('|', '<', '>');
            return pokemonData;
        }

    }
}
