using Abstractions.Interfaces.Controllers;
using Abstractions.Models.AppModels;
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

            mBurialCollection = mController.GetBurials();
            foreach (var item in mBurialCollection)
            {
                var burialIcon = new MapIcon();
                burialIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = item.Location.Latitude, Longitude = item.Location.Longitude });
                burialIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                burialIcon.Title = item.Name;
                myMap.MapElements.Add(burialIcon);
            }           
        }

        private void myMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon myClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            //херня какая-то
            var burial = mBurialCollection.FirstOrDefault(x => x.Name == myClickedIcon.Title);
            if (burial != null)
                mController.DisplayBurial(burial);
        }

        private List<BurialModel> mBurialCollection = new List<BurialModel>();
        private IMainRecordsController mController
        {
            get
            {
                return MainPage.Controller;
            }            
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
