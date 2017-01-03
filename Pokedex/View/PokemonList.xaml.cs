using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PokemonList : Page
    {
        private int start, end;
        public ViewModel.PokemonBriefVM PokemonBriefVM { get; set; }
        public IEnumerable<string> ContentList { get { return PokemonBriefVM.DataList; } }
        public Frame NavigableFrame { get { return VisualStates.CurrentState == SmallLayout ? Frame : DetailsFrame; } }
        public PokemonList()
        {
            this.InitializeComponent();
            PokemonBriefVM = new ViewModel.PokemonBriefVM();
            this.DataContext = PokemonBriefVM;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (int[])e.Parameter;
            start = parameter.ElementAt(0);
            end = parameter.ElementAt(1);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            PokemonBriefVM.IsBusy = true;
            if (Frame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                Frame.GoBack();
            }
            PokemonBriefVM.IsBusy = false;
        }
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pokemon = e.ClickedItem as PokemonBrief;
            if(VisualStates.CurrentState == SmallLayout)
                Frame.Navigate(typeof(PokemonDetail), pokemon.Name);
            else
                DetailsFrame.Navigate(typeof(PokemonDetail), pokemon.Name);
        }
        private void PreviousSection_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = PokemonBriefVM.FirstIndex - PokemonBriefVM.ElementsPerView;
            if (newIndex > 0)
            {
                PokemonBriefVM.FirstIndex = newIndex;
                PokemonBriefVM.FilterList();
            }
        }
        private void NextSection_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = PokemonBriefVM.FirstIndex + PokemonBriefVM.ElementsPerView;
            if(newIndex < PokemonBriefVM.LastPokemonDex)
            {
                PokemonBriefVM.FirstIndex = newIndex;
                PokemonBriefVM.FilterList();
            }
        }
        private void ElementsPerView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PokemonBriefVM == null)
                return;
            if (PokemonBriefVM.PokemonBriefList.Count == 0)
                PokemonBriefVM.GetPokemonList();
            else
                PokemonBriefVM.FilterList();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (PokemonBriefVM.PokemonBriefList.Count == 0)
                PokemonBriefVM.GetPokemonList();
        }
    }
}
