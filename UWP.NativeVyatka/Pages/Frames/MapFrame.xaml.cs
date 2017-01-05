using NativeVyatkaCore.Utilities;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NativeVyatka.UWP.Pages.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapFrame : Page
    {
        public MapFrame()
        {
            this.InitializeComponent();
        }

        private async void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            //https://msdn.microsoft.com/ru-ru/windows/uwp/maps-and-location/display-maps для ключа
            myMap.ZoomLevel = 12;
            myMap.Style = MapStyle.AerialWithRoads;
            myMap.LandmarksVisible = false;
            var posotion = await CrossGeolocator.Current.GetPositionAsync();
            myMap.Center = new Geopoint(new BasicGeoposition() { Latitude = posotion.Latitude, Longitude = posotion.Longitude });

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = myMap.Center;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "Space Needle";
            mapIcon1.ZIndex = 0;

            myMap.MapElements.Add(mapIcon1);

            /*MapItems.ItemsSource = new List<PointOfInterest>()
            {
                new PointOfInterest()
                {
                    DisplayName = "Place One",
                    ImageSourceUri = new Uri("ms-appx:///Assets/Images/nophoto.png"),
                    NormalizedAnchorPoint = new Point(0.5, 1),
                    Location = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = posotion.Latitude + 0.001,
                        Longitude = posotion.Longitude - 0.001
                    })
                }
            };*/
        }

        private void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            var tappedGeoPosition = args.Location.Position;
            iConsole.WriteLine(tappedGeoPosition.ToString());
        }

        private void mapItemButton_Click(object sender, RoutedEventArgs e)
        {
            var buttonSender = sender as Button;
            PointOfInterest poi = buttonSender.DataContext as PointOfInterest;
            iConsole.WriteLine(poi.ToString());
        }

        private void myMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {

        }
    }

    public class PointOfInterest
    {
        public string DisplayName { get; set; }
        public Geopoint Location { get; set; }
        public Uri ImageSourceUri { get; set; }
        public Point NormalizedAnchorPoint { get; set; }
    }
}
