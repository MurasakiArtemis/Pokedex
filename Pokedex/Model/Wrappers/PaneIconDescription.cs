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
