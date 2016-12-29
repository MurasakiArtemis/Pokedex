using Pokedex.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using Pokedex.Communication;

namespace Pokedex.ViewModel
{
    class PokemonVM : INotifyPropertyChanged
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
            HttpCommunication source = new HttpCommunication();
            int section = 0;
            string pokemonBaseData = source.GetResponse(Model.UrlQueryBuilder.PokemonContentQuery(pokemonName, section)).Result;
            IsBusy = true;
            CurrentPokemon = new Pokemon(pokemonBaseData);
            IsBusy = false;
        }

    }
}
