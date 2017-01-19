using Pokedex.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pokedex.Communication;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using Pokedex.View.Converters;
using System;
using Pokedex.Model.Wrappers;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.ViewModel
{
    public class PokemonVM : INotifyPropertyChanged
    {
        private Pokemon _currentPokemon;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
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

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                if (value != _isError)
                {
                    _isError = value;
                    RaiseProperty();
                }
            }
        }
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
            try
            {
                if (pokemonName.StartsWith("Alolan"))
                    pokemonName.Remove(0, "Alolan".Length);
                if (pokemonName.StartsWith("Mega"))
                    pokemonName.Remove(0, "Mega".Length);
                string pokemonBaseData = await source.GetResponse(UrlQueryBuilder.PokemonContentQuery(pokemonName, 0));
                CurrentPokemon = await GetPokemonFromJson(pokemonBaseData);
            }
            catch
            {
                IsError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task<Pokemon> GetPokemonFromJson(string pokemonBaseData)
        {
            string[] pokemonData = ExtractInfoboxData(pokemonBaseData);
            var nationalDex = int.Parse(ExtractionFunction(pokemonData, "ndex"));
            var name = ExtractionFunction(pokemonData, "name");
            int formsNumber;
            if (!int.TryParse(ExtractionFunction(pokemonData, "forme"), out formsNumber))
                formsNumber = 1;
            int typesNumber;
            if (!int.TryParse(ExtractionFunction(pokemonData, "typebox"), out typesNumber))
                typesNumber = 2;
            var taskForms = ExtractForms(pokemonData, typesNumber, formsNumber, name, nationalDex);
            var japaneseName = ExtractionFunction(pokemonData, "jname");
            var japaneseTransliteration = ExtractionFunction(pokemonData, "jtranslit");
            var japaneseRomanizedName = ExtractionFunction(pokemonData, "tmname");
            var regionsAndNumbers = ExtractRegionsAndNumbers(pokemonData);
            var category = ExtractionFunction(pokemonData, "category");
            bool hasMega;
            var megaStones = ExtractMegaStones(pokemonData, formsNumber, out hasMega);
            int eggGroupNumber = int.Parse(ExtractionFunction(pokemonData, "egggroupn"));
            var eggGroups = ExtractEggGroups(pokemonData, eggGroupNumber);
            var hatchTime = int.Parse(ExtractionFunction(pokemonData, "eggcycles")) * 257;
            var experienceYield = int.Parse(ExtractionFunction(pokemonData, "expyield"));
            var temporalGenderCode = double.Parse(ExtractionFunction(pokemonData, "gendercode"));
            var genderCode = temporalGenderCode == 255? -1 : temporalGenderCode / 254;
            var catchRate = int.Parse(ExtractionFunction(pokemonData, "catchrate"));
            var introducedGeneration = (Generation)Enum.GetValues(typeof(Generation)).GetValue(int.Parse(ExtractionFunction(pokemonData, "generation")) - 1);
            var baseFriendship = int.Parse(ExtractionFunction(pokemonData, "friendship"));
            var URL = UrlQueryBuilder.PokemonUrlQuery(name);
            var forms = await taskForms;
            return new Pokemon() { NationalDex = nationalDex, Name = name, JapaneseName = japaneseName, JapaneseTransliteration = japaneseTransliteration,
                JapaneseRomanizedName = japaneseRomanizedName, RegionsAndNumbers = regionsAndNumbers, Category = category, Forms = forms, HasMega = hasMega,
                MegaStones = megaStones, EggGroups = eggGroups, HatchTime = hatchTime, ExperienceYield = experienceYield, GenderCode = genderCode,
                CatchRate = catchRate, IntroducedGeneration = introducedGeneration, BaseFriendship = baseFriendship, URL = URL};
        }
        private async Task<ObservableCollection<Form>> ExtractForms(string[] pokemonData, int typesAmount, int formsAmount, string name, int nationalDex)
        {
            var forms = new ObservableCollection<Form>();
            for (int i = 1; i <= formsAmount; i++)
            {
                var temporalNameResult = ExtractFormName(pokemonData, i, name);
                var temporalImageRelativeLinkResult = ExtractFormPicture(pokemonData, i, name, nationalDex);
                var taskImage = ExtractPictureAsync(temporalImageRelativeLinkResult);
                var heightResult = ExtractFormHeight(pokemonData, i, forms);
                var weightResult = ExtractFormWeight(pokemonData, i, forms);
                var Types = ExtractFormTypes(pokemonData, typesAmount, i, forms);
                var Abilities = ExtractFormAbilities(pokemonData, i, temporalNameResult, forms);
                var temporalImageResult = await taskImage;
                var formResult = new Form() { Name = temporalNameResult, ImageRelativeLink = temporalImageRelativeLinkResult, Image = temporalImageResult,
                    Types = Types, Height = heightResult, Weight = weightResult, Abilities = Abilities };
                forms.Add(formResult);
            }
            return forms;
        }
        private string ExtractFormName(string[] pokemonData, int formNumber, string name)
        {
            string temporalFormResult = ExtractionFunction(pokemonData, $"form{formNumber}");
            if (string.IsNullOrEmpty(temporalFormResult))
                temporalFormResult = name;
            return temporalFormResult;
        }
        private string ExtractFormPicture(string[] pokemonData, int formNumber, string name, int nationalDex)
        {
            string temporalPictureResult = ExtractionFunction(pokemonData, $"image{formNumber}");
            if (string.IsNullOrEmpty(temporalPictureResult))
                temporalPictureResult = $"{nationalDex.ToString("D3")}{name}.png";
            return $"File:{temporalPictureResult}";
        }
        private ObservableCollection<SlotType> ExtractFormTypes(string[] pokemonData, int typesAmount, int formNumber, ObservableCollection<Form> Forms)
        {
            var types = new ObservableCollection<SlotType>();
            bool requiresBase = true;
            for (int j = 1; j <= typesAmount; j++)
            {
                string temporalTypeResult = ExtractionFunction(pokemonData, $"{(formNumber != 1 ? $"form{formNumber}" : "")}type{j}");
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
        private double ExtractFormWeight(string[] pokemonData, int formNumber, ObservableCollection<Form> Forms)
        {
            double weightResult;
            string temporalWeightResult = ExtractionFunction(pokemonData, $"weight-kg{(formNumber != 1 ? $"{formNumber}" : "")}");
            if (string.IsNullOrEmpty(temporalWeightResult))
                temporalWeightResult = "0.0";
            weightResult = double.Parse(temporalWeightResult);
            if (formNumber != 1 && weightResult == 0.0)
                weightResult = Forms.First().Weight;
            return weightResult;
        }
        private double ExtractFormHeight(string[] pokemonData, int formNumber, ObservableCollection<Form> Forms)
        {
            double heightResult;
            string temporalHeightResult = ExtractionFunction(pokemonData, $"height-m{(formNumber != 1 ? $"{formNumber}" : "")}");
            if (string.IsNullOrEmpty(temporalHeightResult))
                temporalHeightResult = "0.0";
            heightResult = double.Parse(temporalHeightResult);
            if (formNumber != 1 && heightResult == 0.0)
                heightResult = Forms.First().Height;
            return heightResult;
        }
        private ObservableCollection<AbilityName> ExtractFormAbilities(string[] pokemonData, int formNumber, string temporalFormResult, ObservableCollection<Form> Forms)
        {
            string temporalAbilityResult;
            var Abilities = new ObservableCollection<AbilityName>();
            if (temporalFormResult.Contains("Mega"))
            {
                temporalAbilityResult = ExtractionFunction(pokemonData, "abilitym");
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
                        temporalAbilityResult = ExtractionFunction(pokemonData, $"ability{(formNumber != 1 ? $"{formNumber}-" : "")}1");
                        break;
                    case AbilitySlot.Second:
                        temporalAbilityResult = ExtractionFunction(pokemonData, $"ability{(formNumber != 1 ? $"{formNumber}-" : "")}2");
                        break;
                    case AbilitySlot.Hidden:
                        temporalAbilityResult = ExtractionFunction(pokemonData, $"abilityd{(formNumber != 1 ? $"{formNumber}" : "")}");
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
        private ObservableCollection<MegaStonePicture> ExtractMegaStones(string[] pokemonData, int formsNumber, out bool hasMega)
        {
            var megaStones = new ObservableCollection<MegaStonePicture>();
            hasMega = false;
            for (int i = 1; i <= formsNumber; i++)
            {
                string megaStoneName = ExtractionFunction(pokemonData, $"mega{(i != 1 ? $"{i}" : "")}");
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
        private ObservableCollection<string> ExtractEggGroups(string[] pokemonData, int eggGroupNumber)
        {
            var eggGroups = new ObservableCollection<string>();
            for (int i = 1; i <= eggGroupNumber; i++)
                eggGroups.Add(ExtractionFunction(pokemonData, $"egggroup{i}"));
            if(eggGroupNumber == 0)
                eggGroups.Add("Undiscovered");
            return eggGroups;
        }
        private ObservableCollection<RegionNumber> ExtractRegionsAndNumbers(string[] pokemonData)
        {
            var regionsAndNumbers = new ObservableCollection<RegionNumber>();
            foreach (Region region in Enum.GetValues(typeof(Region)))
            {
                string temporalResult;
                switch (region)
                {
                case Region.Kanto:
                    temporalResult = ExtractionFunction(pokemonData, "ndex");
                    break;
                case Region.Johto:
                    temporalResult = ExtractionFunction(pokemonData, "jdex");
                    break;
                case Region.Hoenn:
                    temporalResult = ExtractionFunction(pokemonData, "hdex");
                    break;
                case Region.Sinnoh:
                    temporalResult = ExtractionFunction(pokemonData, "sdex");
                    break;
                case Region.Unova:
                    temporalResult = ExtractionFunction(pokemonData, "udex");
                    break;
                case Region.NewUnova:
                    temporalResult = ExtractionFunction(pokemonData, "u2dex");
                    break;
                case Region.CentralKalos:
                case Region.CoastalKalos:
                case Region.MountainKalos:
                    {
                        string area = ExtractionFunction(pokemonData, "karea");
                        if (area == region.ToString().Replace("Kalos", ""))
                            temporalResult = ExtractionFunction(pokemonData, "kdex");
                        else
                            temporalResult = "";
                    }
                    break;
                case Region.Alola:
                    temporalResult = ExtractionFunction(pokemonData, "adex");
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
        private string ExtractionFunction(string[] data, string attributeName)
        {
            var pokemonDataSelection = data.FirstOrDefault(p => p.StartsWith($"{attributeName}="));
            if (!string.IsNullOrEmpty(pokemonDataSelection))
                return pokemonDataSelection.Replace($"{attributeName}=", "");
            return null;
        }
        private string[] ExtractInfoboxData(string data)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data).Replace("\"", "").Replace("\\n", "");
            replacedString = replacedString.Replace("{{tt|", "");
            string pokemonDataString = replacedString.Split(new string[] { "}}{{", "}}|}", "}}'" }, System.StringSplitOptions.RemoveEmptyEntries).First(p => p.Contains("Infobox"));
            string[] pokemonData = pokemonDataString.Split('|', '<', '>');
            return pokemonData;
        }
        private async Task<BitmapImage> ExtractPictureAsync(string relativeLink)
        {
            var client = new HttpCommunication();
            var linkData = await client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(relativeLink));
            var imageLink = JsonDataExtractor.ExtractPictureUrl(linkData);
            var imageUri = new Uri(imageLink);
            return new BitmapImage(imageUri);
        }
    }
}
