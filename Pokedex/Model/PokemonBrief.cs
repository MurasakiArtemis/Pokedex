using Pokedex.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.Model
{
    public class PokemonBrief : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
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

        private string _imageRelativeLink;
        public string ImageRelativeLink
        {
            get { return _imageRelativeLink; }
            set
            {
                if (value != _imageRelativeLink)
                {
                    _imageRelativeLink = value;
                    RaiseProperty();
                }
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    RaiseProperty();
                }
            }
        }
        public PokemonType PrimaryType { get { return Types.First(p => p.Slot == TypeSlot.Primary).Type; } }

        private ObservableCollection<SlotType> _types;
        public ObservableCollection<SlotType> Types
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
