using Pokedex.Communication;
using Pokedex.Model;
using Pokedex.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.ViewModel
{
    public class PokemonBriefVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<string> PokemonCompleteListData;

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

        private IncrementalLoadingCollection<PokemonBrief> _pokemonBriefList;
        public IncrementalLoadingCollection<PokemonBrief> PokemonBriefList
        {
            get { return _pokemonBriefList; }
            set
            {
                if (value != _pokemonBriefList)
                {
                    _pokemonBriefList = value;
                    RaiseProperty();
                }
            }
        }
        public IEnumerable<string> DataList { get { return PokemonCompleteListData != null? PokemonCompleteListData.Select(p => p.Split('|').ElementAt(3)) : null; } }

        private int _lastPokemonDex;
        public int LastPokemonDex
        {
            get { return _lastPokemonDex; }
            set
            {
                if (value != _lastPokemonDex)
                {
                    _lastPokemonDex = value;
                    RaiseProperty();
                }
            }
        }
        public PokemonBriefVM()
        { }
        private IEnumerable<string> ExtractFullJsonData(string data)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data).Replace("\"", "").Replace("\\n", "");
            var semiParsedString = replacedString.Split(new string[] { "{{", "}}" }, StringSplitOptions.RemoveEmptyEntries);
            var pokemonDataEnumerable = semiParsedString.Where(p => p.StartsWith("rdex|"));
            return pokemonDataEnumerable;
        }
        public async Task GetPokemonList()
        {
            HttpCommunication source = new HttpCommunication();
            IsBusy = true;
            try
            {
                string urlQuery = UrlQueryBuilder.PokemonListQuery();
                string pokemonListBaseData = await source.GetResponse(urlQuery);
                PokemonCompleteListData = ExtractFullJsonData(pokemonListBaseData);
                var numberOfPokemon = PokemonCompleteListData.Count();
                var lastPokemonData = PokemonCompleteListData.ElementAt(numberOfPokemon - 1);
                var splitedString = lastPokemonData.Split('|');
                LastPokemonDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                PokemonBriefList = new IncrementalLoadingCollection<PokemonBrief>(numberOfPokemon, GetElementsList, LoadIndividualItem);
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
            Func<string, int, bool> whereFunction = (p, i) =>
            {
                var splitedString = p.Split('|');
                int nationalDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                bool result = i >= start;
                result &= i < end;
                return result;
            };
            var pokemonDataEnumerable = PokemonCompleteListData.Where(whereFunction).Select(selectionFunction);
            return pokemonDataEnumerable;
        }
        private async Task<PokemonBrief> LoadIndividualItem(PokemonBrief pokemonElement)
        {
            HttpCommunication client = new HttpCommunication();
            var linkData = await client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(pokemonElement.ImageRelativeLink));
            var dataString = JsonDataExtractor.ExtractPictureUrl(linkData);
            Uri imageUri = new Uri(dataString);
            BitmapImage image = new BitmapImage(imageUri);
            pokemonElement.Image = image;
            return pokemonElement;
        }
    }
}
