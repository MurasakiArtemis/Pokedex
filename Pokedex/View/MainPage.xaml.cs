using Pokedex.Model;
using Pokedex.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pokedex.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<PaneIconDescription> PaneObjectsList;
        private IEnumerable<string> dataList;
        public MainPage()
        {
            this.InitializeComponent();
            PaneObjectsList = new List<PaneIconDescription>();
            PaneObjectsList.Add(new PaneIconDescription() { Type = ResourceType.Pokemon, Description = "Pokémon", Icon = new BitmapImage(new Uri("ms-appx:///Assets/Square71x71Logo.scale-100.png")) });
            //PaneObjectsList.Add(new PaneIconDescription() { Type = ResourceType.Ability, Description = "Abilities", Icon = new BitmapImage(new Uri("ms-appx:///Assets/Icon-2.ico")) });
            //PaneObjectsList.Add(new PaneIconDescription() { Type = ResourceType.Item, Description = "Items", Icon = new BitmapImage(new Uri("ms-appx:///Assets/Icon-2.ico")) });
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavMenu.IsPaneOpen = !NavMenu.IsPaneOpen;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContentFrame.Navigate(typeof(PokemonList));
            Frame rootFrame = Window.Current.Content as Frame;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentFrame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                ContentFrame.GoBack();
            }
        }

        private void NavListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PaneIconDescription clickedItem = (PaneIconDescription)e.ClickedItem;
            switch (clickedItem.Type)
            {
            case ResourceType.Pokemon:
                ContentFrame.Navigate(typeof(PokemonList));
                break;
            case ResourceType.Miscellaneous:
            case ResourceType.Ability:
            case ResourceType.Item:
            default:
                break;
            }
        }
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (!string.IsNullOrEmpty(sender.Text))
                {
                    var filteredItems = dataList.Where(p => p.ToLowerInvariant().StartsWith(sender.Text.ToLowerInvariant())).GroupBy(p => p).Select(p => p.First());
                    if (filteredItems.Count() != 0)
                        sender.ItemsSource = filteredItems;
                    else
                        sender.ItemsSource = new string[] { "No results" };
                }
            }
        }
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(!string.IsNullOrEmpty(args.QueryText))
            {
                var query = args.QueryText.ToLowerInvariant();
                query = char.ToUpper(query[0]) + query.Substring(1);
                if (ContentFrame.Content is PokemonList)
                {
                    var frame = ContentFrame.Content as PokemonList;
                    if (dataList.Contains(query))
                        frame.NavigableFrame.Navigate(typeof(PokemonDetail), query);
                }
                else if (ContentFrame.Content is PokemonDetail)
                {
                    if (dataList.Contains(query))
                        ContentFrame.Navigate(typeof(PokemonDetail), query);
                }
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.Content is PokemonList)
            {
                var frame = ContentFrame.Content as PokemonList;
                dataList = frame.ContentList;
            }
        }
    }
}
