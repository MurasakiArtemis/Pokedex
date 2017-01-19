﻿using Pokedex.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
    }
}
