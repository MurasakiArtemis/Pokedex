using Pokedex.Communication;
using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        private IncrementalLoadingCollection _pokemonBriefList;
        public IncrementalLoadingCollection PokemonBriefList
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
                PokemonBriefList = new IncrementalLoadingCollection(LastPokemonDex, PokemonCompleteListData);
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
    }
}
