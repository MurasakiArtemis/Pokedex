using Pokedex.Model.Wrappers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.Model
{
    class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

        private string _japaneseName;
        public string JapaneseName
        {
            get { return _japaneseName; }
            set
            {
                if (value != _japaneseName)
                {
                    _japaneseName = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseTransliteration;
        public string JapaneseTransliteration
        {
            get { return _japaneseTransliteration; }
            set
            {
                if (value != _japaneseTransliteration)
                {
                    _japaneseTransliteration = value;
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

        private Generation _generation;
        public Generation Generation
        {
            get { return _generation; }
            set
            {
                if (value != _generation)
                {
                    _generation = value;
                    RaiseProperty();
                }
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    RaiseProperty();
                }
            }
        }

        private Pocket _pocket;
        public Pocket Pocket
        {
            get { return _pocket; }
            set
            {
                if (value != _pocket)
                {
                    _pocket = value;
                    RaiseProperty();
                }
            }
        }

        private bool _isBuyable;
        public bool IsBuyable
        {
            get { return _isBuyable; }
            set
            {
                if (value != _isBuyable)
                {
                    _isBuyable = value;
                    RaiseProperty();
                }
            }
        }

        private int _buyPrice;
        public int BuyPrice
        {
            get { return _buyPrice; }
            set
            {
                if (value != _buyPrice)
                {
                    _buyPrice = value;
                    RaiseProperty();
                }
            }
        }

        private int _sellPrice;
        public int SellPrice
        {
            get { return _sellPrice; }
            set
            {
                if (value != _sellPrice)
                {
                    _sellPrice = value;
                    RaiseProperty();
                }
            }
        }

        private string _effect;
        public string Effect
        {
            get { return _effect; }
            set
            {
                if (value != _effect)
                {
                    _effect = value;
                    RaiseProperty();
                }
            }
        }

        private int _catchRate;
        public int CatchRate
        {
            get { return _catchRate; }
            set
            {
                if (value != _catchRate)
                {
                    _catchRate = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<GameDescription> _descriptions;
        public ObservableCollection<GameDescription> Descriptions
        {
            get { return _descriptions; }
            set
            {
                if (value != _descriptions)
                {
                    _descriptions = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<GameLocation> _locations;
        public ObservableCollection<GameLocation> Locations
        {
            get { return _locations; }
            set
            {
                if (value != _locations)
                {
                    _locations = value;
                    RaiseProperty();
                }
            }
        }

        private string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    RaiseProperty();
                }
            }
        }

    }
}
