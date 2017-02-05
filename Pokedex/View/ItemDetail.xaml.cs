using Pokedex.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Pokedex.View
{
    public sealed partial class ItemDetail : Page
    {
        public ItemDetail()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string[] parameters = (string[])e.Parameter;
            string itemName = parameters[0];
            string category = parameters[1];
            LoadInformation(itemName, category);
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
        private async void LoadInformation(string itemName, string category)
        {
            await ItemVM.GetItem(itemName, category);
            Bindings.Update();
        }
    }
}
