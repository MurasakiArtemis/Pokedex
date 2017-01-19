using Pokedex.Communication;
using Pokedex.Model;
using Pokedex.Model.Wrappers;
using Pokedex.View.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.ViewModel
{
    public class IncrementalLoadingCollection : ObservableCollection<PokemonBrief>, ISupportIncrementalLoading
    {
        private IEnumerable<string> DataList { get; set; }
        private int MaxItems { get; set; }
        public IncrementalLoadingCollection(int maxItems, IEnumerable<string> dataList)
        {
            MaxItems = maxItems;
            DataList = dataList;
        }
        public bool HasMoreItems { get { return Count < MaxItems; } }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }
        private IEnumerable<PokemonBrief> GetElementsList(long start, long end)
        {
            Func<string, PokemonBrief> selectionFunction = p =>
            {
                var splitedString = p.Split('|');
                int nationalDex;
                if (!int.TryParse(splitedString.ElementAt(2), out nationalDex))
                    nationalDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                string imageRelativeLink = $"File:{splitedString.ElementAt(2)}MS.png";
                var name = splitedString.ElementAt(3);
                var types = new ObservableCollection<SlotType>();
                var numberOfTypes = int.Parse(splitedString.ElementAt(4));
                for (int i = 0; i < numberOfTypes; i++)
                {
                    var temporalTypeResult = splitedString.ElementAt(5 + i);
                    PokemonType result = (PokemonType)Enum.Parse(typeof(PokemonType), temporalTypeResult);
                    types.Add(new SlotType() { Slot = i == 0 ? TypeSlot.Primary : TypeSlot.Secondary, Type = result });
                }
                return new PokemonBrief() { Name = name, NationalDex = nationalDex, ImageRelativeLink = imageRelativeLink, Types = types, URL = UrlQueryBuilder.PokemonUrlQuery(name) };
            };
            Func<string, bool> whereFunction = p =>
            {
                var splitedString = p.Split('|');
                int nationalDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                bool result = nationalDex > start;
                result &= nationalDex <= end;
                return result;
            };
            var pokemonDataEnumerable = DataList.Where(whereFunction).Select(selectionFunction);
            return pokemonDataEnumerable;
        }
        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            var pokemonListData = GetElementsList(Count, Count + expectedCount);
            int startingCount = Count;
            //List<Task> tasks = new List<Task>();
            try
            {
                if (pokemonListData != null && pokemonListData.Any())
                    foreach (var pokemonElement in pokemonListData)
                        //tasks.Add(LoadIndividualItem(pokemonElement));
                        await LoadIndividualItem(pokemonElement);
                else
                    throw new Exception();
                //await Task.WhenAll(tasks);
            }
            catch
            {
                MaxItems = Count;
            }
            return new LoadMoreItemsResult { Count = (uint)(Count - startingCount) };
        }
        private async Task LoadIndividualItem(PokemonBrief pokemonElement)
        {
            HttpCommunication client = new HttpCommunication();
            var linkData = await client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(pokemonElement.ImageRelativeLink));
            var dataString = JsonDataExtractor.ExtractPictureUrl(linkData);
            Uri imageUri = new Uri(dataString);
            BitmapImage image = new BitmapImage(imageUri);
            pokemonElement.Image = image;
            Add(pokemonElement);
        }
    }
}
