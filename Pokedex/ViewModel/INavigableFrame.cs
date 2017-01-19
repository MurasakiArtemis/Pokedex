using Windows.UI.Xaml.Controls;

namespace Pokedex.ViewModel
{
    interface INavigableFrame
    {
        Frame NavigableFrame { get; }
        void NavigateDefault(object parameter);
    }
}
