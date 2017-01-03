using Pokedex.View.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pokedex.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PokemonDetail : Page
    {
        private Pokedex.ViewModel.PokemonVM PokemonVM { get; set; }
        public PokemonDetail()
        {
            this.InitializeComponent();
            PokemonVM = new ViewModel.PokemonVM();
        }

        private static double add(double value1)
        {
            return value1 + 0.01;
        }

        private void MegaStone_Click(object sender, RoutedEventArgs e)
        {
            var button = (HyperlinkButton)sender;
            var text = ((StackPanel)button.Content).Children.OfType<TextBlock>().Single(f => f.Name == "MegaStoneName").Text;
            //Frame.Navigate();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PokemonVM.IsBusy = true;
            string pokemonName = (string)e.Parameter;
            PokemonVM.GetPokemon(pokemonName);
            PokemonVM.IsBusy = false;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack? AppViewBackButtonVisibility.Visible: AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            PokemonVM.IsBusy = true;
            if (Frame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                Frame.GoBack();
            }
            PokemonVM.IsBusy = false;
        }
    }
}
