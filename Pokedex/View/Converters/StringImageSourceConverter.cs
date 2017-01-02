using Pokedex.Communication;
using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Pokedex.View.Converters
{
    class StringImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string))
                throw new ArgumentException("Value is not of type string");
            var fileLink = (string)value;
            HttpCommunication client = new HttpCommunication();
            var data = client.GetResponse(UrlQueryBuilder.PictureLocationQuery(fileLink)).Result;
            var dataString = UrlQueryBuilder.PictureLocation(data);
            Uri imageUri = new Uri(dataString);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            return imageBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
