using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.Model
{
    class ItemBrief : INotifyPropertyChanged
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

        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                if (value != _index)
                {
                    _index = value;
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
    }
}
