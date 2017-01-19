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

namespace Pokedex.View
{
    public sealed partial class PokemonDetail : Page
    {
        private Pokedex.ViewModel.PokemonVM PokemonVM { get; set; }
        public PokemonDetail()
        {
            this.InitializeComponent();
            PokemonVM = new ViewModel.PokemonVM();
        }
        public bool IsError { get; set; }
        private void MegaStone_Click(object sender, RoutedEventArgs e)
        {
            var button = (HyperlinkButton)sender;
            var text = ((StackPanel)button.Content).Children.OfType<TextBlock>().First(f => f.Name == "MegaStoneName").Text;
            //Frame.Navigate();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                string pokemonName = (string)e.Parameter;
                LoadInformation(pokemonName);
            }
            catch
            {
                IsError = true;
            }
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
            try
            {
                await PokemonVM.GetPokemon(pokemonName);
            }
            catch
            {
                IsError = true;
            }
            Bindings.Update();
        }
    }
}
