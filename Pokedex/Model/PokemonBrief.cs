using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Model
{
    class PokemonBrief : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _nationalDex;
        public int NationalDex
        {
            get { return _nationalDex; }
            set
            {
                if (value != _nationalDex)
                {
                    _nationalDex = value;
                    RaiseProperty();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaiseProperty();
                }
            }
        }

        private PokemonType[] _types;
        public PokemonType[] Type
        {
            get { return _types; }
            set
            {
                if (value != _types)
                {
                    _types = value;
                    RaiseProperty();
                }
            }
        }

        private string url;
        public string URL
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    RaiseProperty();
                }
            }
        }

    }
}
