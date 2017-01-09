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
    public class Form : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _formName;
        public string Name
        {
            get { return _formName; }
            set
            {
                if (value != _formName)
                {
                    _formName = value;
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

        private ObservableCollection<AbilityName> _abilities;
        public ObservableCollection<AbilityName> Abilities
        {
            get { return _abilities; }
            set
            {
                if (value != _abilities)
                {
                    _abilities = value;
                    RaiseProperty();
                }
            }
        }

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
        public PokemonType PrimaryType { get { return Types.Where(p => p.Slot == TypeSlot.Primary).Single().Type; } }

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    RaiseProperty();
                }
            }
        }

        private double _weight;
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value != _weight)
                {
                    _weight = value;
                    RaiseProperty();
                }
            }
        }
    }
}
