using Pokedex.Model;
using Pokedex.ViewModel;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
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
            NavigateDefault(new string[] { item.Name, item.Category });
        }
        public void NavigateDefault(object parameter)
        {
            NavigableFrame.Navigate(typeof(ItemDetail), parameter);
        }
    }
}
