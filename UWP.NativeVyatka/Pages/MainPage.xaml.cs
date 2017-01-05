using NativeVyatka.UWP.Pages.Frames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NativeVyatka.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

                }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if  (menuRecords.IsSelected)
            {
                fContentFrame.Navigate(typeof(RecordsFrame));
            }
            else if (menuMap.IsSelected)
            {
                fContentFrame.Navigate(typeof(MapFrame));
            }
            svNavigationMenu.IsPaneOpen = false;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            svNavigationMenu.IsPaneOpen = !svNavigationMenu.IsPaneOpen;
        }
    }
}
