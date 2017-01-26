using Pokedex.Model;
using Pokedex.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ItemList : Page, INavigableFrame, IContentList
    {
        public IEnumerable<string> ContentList { get { return ItemBriefVM.DataList; } }
        public Frame NavigableFrame { get { return VisualStates.CurrentState == StackedLayout ? Frame : DetailsFrame; } }
        public ItemList()
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
            await ItemBriefVM.GetItemList();
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
            var item = e.ClickedItem as ItemBrief;
            //if (VisualStates.CurrentState == StackedLayout)
            //    Frame.Navigate(typeof(ItemDetail), item.Name);
            //else
            //    DetailsFrame.Navigate(typeof(ItemDetail), item.Name);
        }
        public void NavigateDefault(object parameter)
        {
            //NavigableFrame.Navigate(typeof(ItemDetail), parameter);
        }
    }
}
