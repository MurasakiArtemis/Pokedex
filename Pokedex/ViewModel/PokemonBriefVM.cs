using Pokedex.Communication;
using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
        public IEnumerable<string> DataList { get { return PokemonCompleteListData.Select(p => p.Split('|').ElementAt(3)); } }

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
        {
            GetPokemonList();
            PokemonBriefList = new IncrementalLoadingCollection(LastPokemonDex, PokemonCompleteListData);
        }
        private IEnumerable<string> ExtractFullJsonData(string data)
        {
            var replacedString = JsonDataExtractor.ExtractContent(data);
            var semiParsedString = replacedString.Split(new string[] { "{{", "}}" }, StringSplitOptions.RemoveEmptyEntries);
            var pokemonDataEnumerable = semiParsedString.Where(p => p.StartsWith("rdex|"));
            return pokemonDataEnumerable;
        }
        private void GetPokemonList()
        {
            HttpCommunication source = new HttpCommunication();
            string urlQuery = UrlQueryBuilder.PokemonListQuery();
            string pokemonListBaseData = source.GetResponse(urlQuery).Result;
            PokemonCompleteListData = ExtractFullJsonData(pokemonListBaseData);
            var numberOfPokemon = PokemonCompleteListData.Count();
            var lastPokemonData = PokemonCompleteListData.ElementAt(numberOfPokemon - 1);
            var splitedString = lastPokemonData.Split('|');
            LastPokemonDex = int.Parse(Regex.Replace(splitedString.ElementAt(2), "[A-Za-z ]", ""));
        }
    }
}
