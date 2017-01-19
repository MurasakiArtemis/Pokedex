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
