using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Pokedex.View.Converters
{
    public class ColourModifier
    {
        public static SolidColorBrush ChangeColorBrightness(SolidColorBrush color, double correctionFactor)
        {
            double red = color.Color.R;
            double green = color.Color.G;
            double blue = color.Color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return new SolidColorBrush(Color.FromArgb(color.Color.A, (byte)red, (byte)green, (byte)blue));
        }
    }
}
