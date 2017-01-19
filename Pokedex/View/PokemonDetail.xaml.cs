using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Pokedex.View
{
    public sealed partial class PokemonDetail : Page
    {
        public PokemonDetail()
        {
            this.InitializeComponent();
        }
        private void MegaStone_Click(object sender, RoutedEventArgs e)
        {
            var button = (HyperlinkButton)sender;
            var text = ((StackPanel)button.Content).Children.OfType<TextBlock>().First(f => f.Name == "MegaStoneName").Text;
            //Frame.Navigate();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string pokemonName = (string)e.Parameter;
            LoadInformation(pokemonName);
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack? AppViewBackButtonVisibility.Visible: AppViewBackButtonVisibility.Collapsed;
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
        private async void LoadInformation(string pokemonName)
        {
            await PokemonVM.GetPokemon(pokemonName);
            Bindings.Update();
        }
    }
}
