using Pokedex.Communication;
using Pokedex.Model;
using Pokedex.Model.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.ViewModel
{
    class ItemVM : INotifyPropertyChanged
    {
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

        private string _errorDescription;
        public string ErrorDescription
        {
            get { return _errorDescription; }
            set
            {
                if (value != _errorDescription)
                {
                    _errorDescription = value;
                    RaiseProperty();
                }
            }
        }

        private Item _currentItem;
        public Item CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (value != _currentItem)
                {
                    _currentItem = value;
                    RaiseProperty();
                }
            }
        }
        public ItemVM()
        { }
        public async Task GetItem(string itemName, string category)
        {
            HttpCommunication source = new HttpCommunication();
            IsBusy = true;
            try
            {
                if (itemName == "Master Ball")
                    category = "Poké Ball";
                string itemSections = await source.GetResponse(UrlQueryBuilder.ItemSectionsQuery(category));
                var sectionsList = JsonDataExtractor.ExtractSectionList(itemSections);
                int section;
                try
                {
                    section = JsonDataExtractor.ExtractSection(sectionsList, PokemonSection.Item);
                }
                catch
                {
                    section = 0;
                }
                string itemBaseData = await source.GetResponse(UrlQueryBuilder.ItemContentQuery(category, section));
                CurrentItem = await GetItemFromJson(itemBaseData, itemName, category);
            }
            catch(System.Net.Http.HttpRequestException)
            {
                IsError = true;
                ErrorDescription = (string)App.Current.Resources["InternetErrorMessage"];
            }
            catch
            {
                IsError = true;
                ErrorDescription = (string)App.Current.Resources["ErrorMessage"]; ;
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task<Item> GetItemFromJson(string itemBaseData, string itemName, string category)
        {
            string[] itemData = ExtractItemData(itemBaseData, itemName);
            var name = ExtractionFunction(itemData, "name");
            var imageRelativeLink = $"File:Bag {name} Sprite.png";
            HttpCommunication client = new HttpCommunication();
            var taskLinkData = client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(imageRelativeLink));
            var japaneseName = ExtractionFunction(itemData, "jp");
            var japaneseTransliteration = ExtractionFunction(itemData, "jpt");
            var generationString = ExtractionFunction(itemData, "gen");
            Generation generation = ExtractGeneration(generationString);
            var pocketString = ExtractionFunction(itemData, "bag");
            Pocket pocket = (Pocket)Enum.Parse(typeof(Pocket), pocketString);
            var buyable = ExtractionFunction(itemData, "buyable") == "yes" ? true : false;
            var buyPriceString = ExtractionFunction(itemData, "buy");
            int buyPrice = 0;
            if(!string.IsNullOrEmpty(buyPriceString))
                if (!int.TryParse(buyPriceString, out buyPrice))
                    buyPrice = int.Parse(Regex.Match(buyPriceString, @"[\d]+").Value);
            var sellPriceString = ExtractionFunction(itemData, "sell");
            int sellPrice = 0;
            if (!string.IsNullOrEmpty(sellPriceString))
                if (!int.TryParse(buyPriceString, out sellPrice))
                    sellPrice = int.Parse(Regex.Match(sellPriceString, @"[\d]+").Value);
            var effect = ExtractionFunction(itemData, "effect");
            var catchRateString = ExtractionFunction(itemData, "catchrate");
            int catchRate = 0;
            if (!string.IsNullOrEmpty(catchRateString))
                if (!int.TryParse(catchRateString, out catchRate))
                    catchRate = int.Parse(Regex.Match(catchRateString, @"[\d]+").Value);
            var descriptions = ExtractDescriptions(itemData);
            ObservableCollection<GameLocation> locations = ExtractLocations(itemData);
            var dataString = JsonDataExtractor.ExtractPictureUrl(await taskLinkData);
            Uri imageUri = new Uri(dataString);
            BitmapImage image = new BitmapImage(imageUri);
            var URL = UrlQueryBuilder.ItemUrlQuery(name, category);
            return new Item
            {
                Name = name,
                JapaneseName = japaneseName,
                JapaneseTransliteration = japaneseTransliteration,
                ImageRelativeLink = imageRelativeLink,
                Image = image,
                Generation = generation,
                Category = category,
                Pocket = pocket,
                IsBuyable = buyable,
                BuyPrice = buyPrice,
                SellPrice = sellPrice,
                Effect = effect,
                CatchRate = catchRate,
                Descriptions = descriptions,
                Locations = locations,
                URL = URL
            };
        }
        private static Generation ExtractGeneration(string generationString)
        {
            Generation generation;
            switch (generationString)
            {
            case "I":
                generation = Generation.First;
                break;
            case "II":
                generation = Generation.Second;
                break;
            case "III":
                generation = Generation.Third;
                break;
            case "IV":
                generation = Generation.Fourth;
                break;
            case "V":
                generation = Generation.Fifth;
                break;
            case "VI":
                generation = Generation.Sixth;
                break;
            case "VII":
                generation = Generation.Seventh;
                break;
            default:
                generation = Generation.First;
                break;
            }

            return generation;
        }
        private ObservableCollection<GameLocation> ExtractLocations(string[] itemData)
        {
            var locations = new ObservableCollection<GameLocation>();
            foreach (Game game in Enum.GetValues(typeof(Game)))
            {
                string location;
                switch (game)
                {
                case Game.Red:
                case Game.Green:
                case Game.Blue:
                case Game.Yellow:
                    location = ExtractionFunction(itemData, "locrby");
                    break;
                case Game.Gold:
                case Game.Silver:
                case Game.Crystal:
                    location = ExtractionFunction(itemData, "locgsc");
                    break;
                case Game.Ruby:
                case Game.Sapphire:
                case Game.Emerald:
                    location = ExtractionFunction(itemData, "locrse");
                    break;
                case Game.FireRed:
                case Game.LeafGreen:
                    location = ExtractionFunction(itemData, "locfrlg");
                    break;
                case Game.Diamond:
                case Game.Pearl:
                case Game.Platinum:
                    location = ExtractionFunction(itemData, "locdppt");
                    break;
                case Game.HeartGold:
                case Game.SoulSilver:
                    location = ExtractionFunction(itemData, "lochgss");
                    break;
                case Game.Black:
                case Game.White:
                    location = ExtractionFunction(itemData, "locbw");
                    break;
                case Game.Black2:
                case Game.White2:
                    location = ExtractionFunction(itemData, "locb2w2");
                    break;
                case Game.X:
                case Game.Y:
                    location = ExtractionFunction(itemData, "locxy");
                    break;
                case Game.OmegaRuby:
                case Game.AlphaSapphire:
                    location = ExtractionFunction(itemData, "locoras");
                    break;
                case Game.Sun:
                case Game.Moon:
                    location = ExtractionFunction(itemData, "locsm");
                    break;
                default:
                    location = "";
                    break;
                }
                if (!string.IsNullOrEmpty(location))
                    locations.Add(new GameLocation() { Game = game, Location = location });
            }
            return locations;
        }
        private ObservableCollection<GameDescription> ExtractDescriptions(string[] itemData)
        {
            var descriptions = new ObservableCollection<GameDescription>();
            foreach (Game game in Enum.GetValues(typeof(Game)))
            {
                string description;
                switch (game)
                {
                case Game.Red:
                case Game.Green:
                case Game.Blue:
                case Game.Yellow:
                    description = ExtractionFunction(itemData, "descrby");
                    break;
                case Game.Gold:
                case Game.Silver:
                case Game.Crystal:
                    description = ExtractionFunction(itemData, "descgsc");
                    break;
                case Game.Ruby:
                case Game.Sapphire:
                case Game.Emerald:
                    description = ExtractionFunction(itemData, "descrse");
                    break;
                case Game.FireRed:
                case Game.LeafGreen:
                    description = ExtractionFunction(itemData, "descfrlg");
                    break;
                case Game.Diamond:
                case Game.Pearl:
                case Game.Platinum:
                    description = ExtractionFunction(itemData, "descdppthgss");
                    break;
                case Game.HeartGold:
                case Game.SoulSilver:
                    description = ExtractionFunction(itemData, "descdppthgss");
                    break;
                case Game.Black:
                case Game.White:
                    description = ExtractionFunction(itemData, "descbwb2w2");
                    break;
                case Game.Black2:
                case Game.White2:
                    description = ExtractionFunction(itemData, "descbwb2w2");
                    break;
                case Game.X:
                case Game.Y:
                    description = ExtractionFunction(itemData, "descxyoras");
                    break;
                case Game.OmegaRuby:
                case Game.AlphaSapphire:
                    description = ExtractionFunction(itemData, "descxyoras");
                    break;
                case Game.Sun:
                case Game.Moon:
                    description = ExtractionFunction(itemData, "descsm");
                    break;
                default:
                    description = "";
                    break;
                }
                if(!string.IsNullOrEmpty(description))
                    descriptions.Add(new GameDescription() { Game = game, Description = description });
            }

            return descriptions;
        }
        private string ExtractionFunction(string[] data, string attributeName)
        {
            var itemDataSelection = data.FirstOrDefault(p => p.StartsWith($"{attributeName}="));
            if (!string.IsNullOrEmpty(itemDataSelection))
                return itemDataSelection.Replace($"{attributeName}=", "");
            return null;
        }
        private string[] ExtractItemData(string data, string itemName)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data).Replace("\"", "");
            var aux = replacedString.Split(new string[] { @"\n\n{{", @"\n}}\n{{" }, System.StringSplitOptions.RemoveEmptyEntries);
            string itemDataString = aux.First(p => p.Contains($"name={itemName}"));
            string[] itemData = itemDataString.Split(new string[] { @"\n|" }, System.StringSplitOptions.RemoveEmptyEntries);
            return itemData;
        }
    }
}
