using Pokedex.Model;
using Pokedex.ViewModel;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Pokedex.View
{
    public sealed partial class PokemonList : Page, INavigableFrame, IContentList
    {
        public IEnumerable<string> ContentList { get { return PokemonBriefVM.DataList; } }
        public Frame NavigableFrame { get { return VisualStates.CurrentState == StackedLayout ? Frame : DetailsFrame; } }
        public PokemonList()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadInformation();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }
        private async void LoadInformation()
        {
            await PokemonBriefVM.GetPokemonList();
            Bindings.Update();
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pokemon = e.ClickedItem as PokemonBrief;
            if(VisualStates.CurrentState == StackedLayout)
                Frame.Navigate(typeof(PokemonDetail), pokemon.Name);
            else
                DetailsFrame.Navigate(typeof(PokemonDetail), pokemon.Name);
        }
        public void NavigateDefault(object parameter)
        {
            NavigableFrame.Navigate(typeof(PokemonDetail), parameter);
        }
    }
}
