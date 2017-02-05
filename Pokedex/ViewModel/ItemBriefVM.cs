using Pokedex.Communication;
using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.ViewModel
{
    class ItemBriefVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<string> ItemCompleteListData;

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

        private IncrementalLoadingCollection<ItemBrief> _itemBriefList;
        public IncrementalLoadingCollection<ItemBrief> ItemBriefList
        {
            get { return _itemBriefList; }
            set
            {
                if (value != _itemBriefList)
                {
                    _itemBriefList = value;
                    RaiseProperty();
                }
            }
        }
        public IEnumerable<string> DataList { get { return ItemCompleteListData != null ? ItemCompleteListData.Select(p => Regex.Replace(p.Split('|').ElementAt(4), @"[\d=]+", "")) : null; } }

        private int _lastItemIndex;
        public int LastItemIndex
        {
            get { return _lastItemIndex; }
            set
            {
                if (value != _lastItemIndex)
                {
                    _lastItemIndex = value;
                    RaiseProperty();
                }
            }
        }
        public ItemBriefVM()
        { }
        private IEnumerable<string> ExtractFullJsonData(string data)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data).Replace("\"", "");
            var semiParsedString = replacedString.Split(new string[] { "}}\\n{{" }, StringSplitOptions.RemoveEmptyEntries);
            var pokemonDataEnumerable = semiParsedString.Where(p => 
                p.StartsWith("hexlist|") && 
                !p.Contains("???") && 
                (p.Contains("Evolutionary stone") 
                    || p.Contains("Evolution-inducing held item")
                    || p.Contains("Incense")
                    || p.Contains("Mega Stone")));
            return pokemonDataEnumerable;
        }
        public async Task GetItemList()
        {
            HttpCommunication source = new HttpCommunication();
            IsBusy = true;
            try
            {
                string urlQuery = UrlQueryBuilder.ItemListQuery(Generation.Seventh);
                string itemListBaseData = await source.GetResponse(urlQuery);
                ItemCompleteListData = ExtractFullJsonData(itemListBaseData);
                var numberOfItems = ItemCompleteListData.Count();
                var lastItemData = ItemCompleteListData.ElementAt(numberOfItems - 1);
                var splitedString = lastItemData.Split('|');
                LastItemIndex = int.Parse(splitedString.ElementAt(3));
                ItemBriefList = new IncrementalLoadingCollection<ItemBrief>(LastItemIndex, GetElementsList, LoadIndividualItem);
            }
            catch (System.Net.Http.HttpRequestException)
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
        private IEnumerable<ItemBrief> GetElementsList(long start, long end)
        {
            Func<string, ItemBrief> selectionFunction = p =>
            {
                var splitedString = p.Split('|');
                var categoryElement = splitedString.ElementAt(1);
                string category;
                if (categoryElement.Contains("#"))
                    category = categoryElement.Remove(categoryElement.IndexOf('#'));
                else category = categoryElement;
                int index = int.Parse(splitedString.ElementAt(3));
                string name;
                if (splitedString.Length == 6)
                    name = Regex.Replace(splitedString.ElementAt(4), @"[\d=]+", "");
                else
                    name = category;
                var imageRelativeLink = $"File:Bag {name} Sprite.png";
                Pocket pocket = Pocket.Items;
                switch (splitedString.Last())
                {
                case "7=Items":
                    pocket = Pocket.Items;
                    break;
                case "7=Medicine":
                    pocket = Pocket.Medicine;
                    break;
                case "7=Berries":
                    pocket = Pocket.Berries;
                    break;
                case "7=Key items":
                    pocket = Pocket.Key;
                    break;
                case "7=TMs":
                    pocket = Pocket.TM;
                    break;
                case "7=Z-Crystals":
                    pocket = Pocket.ZCrystal;
                    break;
                }

                return new ItemBrief() { Category = category, ImageRelativeLink = imageRelativeLink, Index = index, Name = name, Pocket = pocket };
            };
            Func<string, int, bool> whereFunction = (p, b) =>
            {
                bool result = b >= start;
                result &= b < end;
                return result;
            };
            var pokemonDataEnumerable = ItemCompleteListData.Where(whereFunction).Select(selectionFunction);
            return pokemonDataEnumerable;
        }
        private async Task<ItemBrief> LoadIndividualItem(ItemBrief itemElement)
        {
            HttpCommunication client = new HttpCommunication();
            var linkData = await client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(itemElement.ImageRelativeLink));
            var dataString = JsonDataExtractor.ExtractPictureUrl(linkData);
            Uri imageUri = new Uri(dataString);
            BitmapImage image = new BitmapImage(imageUri);
            itemElement.Image = image;
            return itemElement;
        }
    }
}
