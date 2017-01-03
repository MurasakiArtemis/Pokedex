using Pokedex.Communication;
using Pokedex.Model;
using Pokedex.Model.Wrappers;
using Pokedex.View.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
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

        private ObservableCollection<PokemonBrief> _pokemonBriefList;
        public ObservableCollection<PokemonBrief> PokemonBriefList
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

        private int _start;
        public int FirstIndex
        {
            get { return _start; }
            set
            {
                if (value != _start)
                {
                    _start = value;
                    RaiseProperty();
                }
            }
        }

        private int _elementsPerView;
        public int ElementsPerView
        {
            get { return _elementsPerView; }
            set
            {
                if (value != _elementsPerView)
                {
                    _elementsPerView = value;
                    RaiseProperty();
                }
            }
        }
        public PokemonBriefVM()
        {
            PokemonBriefList = new ObservableCollection<PokemonBrief>();
            FirstIndex = 1;
            ElementsPerView = 5;
        }

        private IEnumerable<string> pokemonCompleteListData;
        private void GetPokemonListFromJson()
        {
            PokemonBriefList.Clear();
            var pokemonListData = ExtractJsonData(FirstIndex, FirstIndex + ElementsPerView);
            foreach(var pokemonElement in pokemonListData)
            {
                PokemonBriefList.Add(pokemonElement);
            }
        }
        private IEnumerable<string> ExtractFullJsonData(string data)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data);
            var semiParsedString = replacedString.Split(new string[] { "{{", "}}" }, StringSplitOptions.RemoveEmptyEntries);
            var pokemonDataEnumerable = semiParsedString.Where(p => p.StartsWith("rdex|"));
            return pokemonDataEnumerable;
        }
        private IEnumerable<PokemonBrief> ExtractJsonData(int start, int end)
        {
            Func<string, PokemonBrief> selectionFunction = p =>
            {
                var splitedString = p.Split('|');
                int nationalDex;
                if (!int.TryParse(splitedString.ElementAt(2), out nationalDex))
                    nationalDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                var converter = new StringImageSourceConverter();
                var image = (BitmapImage)converter.Convert($"File:{splitedString.ElementAt(2)}MS.png", typeof(BitmapImage), null, null);
                var name = splitedString.ElementAt(3);
                var types = new ObservableCollection<SlotType>();
                var numberOfTypes = int.Parse(splitedString.ElementAt(4));
                for(int i = 0; i < numberOfTypes; i++)
                {
                    var temporalTypeResult = splitedString.ElementAt(5+i);
                    PokemonType result = (PokemonType)Enum.Parse(typeof(PokemonType), temporalTypeResult);
                    types.Add(new SlotType() { Slot = i == 0 ? TypeSlot.Primary : TypeSlot.Secondary, Type = result });
                }
                return new PokemonBrief() { Name = name, NationalDex = nationalDex, Image = image, Types = types, URL = UrlQueryBuilder.PokemonUrlQuery(name) };
            };
            Func<string, bool> whereFunction = p =>
            {
                var splitedString = p.Split('|');
                int nationalDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
                bool result = nationalDex >= start;
                result &= nationalDex < end;
                return result;
            };
            var pokemonDataEnumerable = pokemonCompleteListData.Where(whereFunction).Select(selectionFunction);
            return pokemonDataEnumerable;
        }
        public void GetPokemonList()
        {
            IsBusy = true;
            HttpCommunication source = new HttpCommunication();
            string urlQuery = UrlQueryBuilder.PokemonListQuery();
            string pokemonListBaseData = source.GetResponse(urlQuery).Result;
            pokemonCompleteListData = ExtractFullJsonData(pokemonListBaseData);
            var numberOfPokemon = pokemonCompleteListData.Count();
            var lastPokemonData = pokemonCompleteListData.ElementAt(numberOfPokemon - 1);
            var splitedString = lastPokemonData.Split('|');
            LastPokemonDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
            GetPokemonListFromJson();
            IsBusy = false;
        }
        public void FilterList()
        {
            IsBusy = true;
            GetPokemonListFromJson();
            IsBusy = false;
        }
    }
}
