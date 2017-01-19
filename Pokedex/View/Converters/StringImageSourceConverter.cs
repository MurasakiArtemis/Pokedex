using Pokedex.Communication;
using Pokedex.Model;
using Pokedex.ViewModel;
using System;
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
            var data = client.GetResponse(UrlQueryBuilder.BasePictureLocationQuery(fileLink)).Result;
            var dataString = JsonDataExtractor.ExtractPictureUrl(data);
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
