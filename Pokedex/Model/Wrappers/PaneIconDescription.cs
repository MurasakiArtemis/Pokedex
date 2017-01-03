using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.Model.Wrappers
{
    class PaneIconDescription
    {
        public ResourceType Type { get; set; }
        public BitmapImage Icon { get; set; }
        public string Description { get; set; }
    }
}
