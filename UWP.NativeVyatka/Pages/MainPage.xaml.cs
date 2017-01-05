using Abstractions.Interfaces.Controllers;
using NativeVyatka.UWP.Pages.Frames;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Unity;
using Windows.UI.Xaml.Media.Imaging;

namespace NativeVyatka.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Controller = mController = App.Container.Resolve<IMainController>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetProfile();
            SetFrame();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            mController.Dispose();
        }

        private void SetFrame()
        {
            menuRecords.IsSelected = true;
            OnMenuSelectionChanged(null, null);
        }

        private void SetProfile()
        {
            tbProfileName.Text = mController.Profile.Name;
            tbProfileEmail.Text = mController.Profile.Email;
            elProfilePhoto.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(mController.Profile.PictureUrl)) };
        }

        private void OnMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if  (menuRecords.IsSelected)
            {
                fContentFrame.Navigate(typeof(RecordsFrame));
            }
            else if (menuMap.IsSelected)
            {
                fContentFrame.Navigate(typeof(MapFrame));
            }
            mSyncIcon.Visibility = (menuRecords.IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            svNavigationMenu.IsPaneOpen = false;
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            svNavigationMenu.IsPaneOpen = !svNavigationMenu.IsPaneOpen;
        }       

        private async void OnTakeNewPhoto(object sender, RoutedEventArgs e)
        {
            await mController.CreateNewBurial();
        }

        private async void OnSyncClick(object sender, RoutedEventArgs e)
        {
            await (fContentFrame.Content as RecordsFrame)?.ObtainData();
        }
        public readonly IMainController mController; 
        public static IMainController Controller { get; private set; }
    }
}
