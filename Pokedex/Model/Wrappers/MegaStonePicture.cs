using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.Model.Wrappers
{
    public class MegaStonePicture
    {
        public string Name { get; set; }
        public string ImageRelativeLink { get; set; }
        public BitmapImage Image { get; set; }
    }
}
