using Abstractions.Interfaces.Controllers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Microsoft.Practices.Unity;
using Windows.UI.Xaml.Navigation;
using Abstractions.Models.AppModels;
using NativeVyatkaCore.Utilities;
using FFImageLoading;
using Windows.Devices.Geolocation;
using Abstractions.Interfaces.Plugins;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NativeVyatka.UWP.Pages
{
    public sealed partial class BurialEditPage : Page
    {
        public BurialEditPage()
        {
            this.InitializeComponent();
            mController = App.Container.Resolve<IBurialEditController>();
            mController.BurialUpdated += OnBurialUpdated;
        }       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                var burial = (e.Content as Dictionary<string, string>)[FormBundleConstants.BurialModel];
                mController.Burial = JsonConvert.DeserializeObject<BurialModel>(burial);
                OnDisplayBurial(mController.Burial);
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                mController.ForceGoBack();
            }
        }       

        private void NameTextChanged(object sender, TextChangedEventArgs e)
        {
            mController.Burial.Name = tbName.Text;
            BurialNeedToBeUpdated(tbName);
        }

        private void SurnameTextChanged(object sender, TextChangedEventArgs e)
        {
            mController.Burial.Surname = tbSurname.Text;
            BurialNeedToBeUpdated(tbSurname);
        }

        private void PatronymicTextChanged(object sender, TextChangedEventArgs e)
        {
            mController.Burial.Patronymic = tbSurname.Text;
            BurialNeedToBeUpdated(tbPatronymic);
        }

        private void DescriptionTextChanged(object sender, TextChangedEventArgs e)
        {
            mController.Burial.Desctiption = tbSurname.Text;
            BurialNeedToBeUpdated(tbDescription);
        }

        private void BirthTimeDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            mController.Burial.BirthDay = (args.NewDate != null) ? new DateTime(args.NewDate.Value.Ticks) : (DateTime?)null;
            mController.Updated = true;
        }

        private void DeathTimeDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            mController.Burial.DeathDay = (args.NewDate != null) ? new DateTime(args.NewDate.Value.Ticks) : (DateTime?)null;
            mController.Updated = true;
        }

        private void OnDisplayBurial(BurialModel burial)
        {
            ImageService.Instance.LoadFile(burial.PicturePath).Into(imgPhoto);
            tbName.Text = burial.Name;
            tbSurname.Text = burial.Surname;
            tbPatronymic.Text = burial.Patronymic;
            tbDescription.Text = burial.Desctiption;
            cdpPhotoTime.Date = burial.RecordTime;
            cdpBirthTime.Date = burial.BirthDay;
            cdpDeathTime.Date = burial.DeathDay;
            mSaveIcon.Visibility = (mController.Updated) ? Visibility.Visible : Visibility.Collapsed;
            mDeleteIcon.Visibility = (!mController.Creating) ? Visibility.Visible : Visibility.Collapsed;
            OnMapBurialLoaded(null, null);
        }

        private void BurialNeedToBeUpdated(Control view)
        {
            if (view.Tag == null)
            {
                view.Tag = true;
            }
            else
            {
                mController.Updated = true;
            }
        }

        private void OnBurialUpdated(object sender, bool e)
        {
            mSaveIcon.Visibility = e ? Visibility.Visible : Visibility.Collapsed;
        }       

        private void OnMapBurialLoaded(object sender, RoutedEventArgs e)
        {
            if (mapBurial != null && mController.Burial != BurialModel.Null)
            {
                mapBurial.ZoomLevel = 12;
                mapBurial.Style = MapStyle.AerialWithRoads;
                mapBurial.Center = new Geopoint(new BasicGeoposition() { Latitude = mController.Burial.Location.Latitude, Longitude = mController.Burial.Location.Longitude });

                mapBurial.MapElements.Clear();
                var burialMarker = new MapIcon();
                burialMarker.Location = mapBurial.Center;
                burialMarker.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
                burialMarker.Title = mController.Burial.Name;
                burialMarker.ZIndex = 0;
                mapBurial.MapElements.Add(burialMarker);
            }       
        }        

        private async void OnRetakePhoto(object sender, RoutedEventArgs e)
        {
            var path = await mController.RetakePhotoAsync();
            ImageService.Instance.LoadFile(path).Into(imgPhoto);
        }

        private async void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            await mController.SaveAndUploadBurialAndGoBackAsync();
        }

        private async void OnDeleteIconClick(object sender, RoutedEventArgs e)
        {
          await mController.DeleteRecordAsync();
        }

        private async void SaveIconClick(object sender, RoutedEventArgs e)
        {
           await mController.SaveAndUploadBurialAsync();
        }

        private readonly IBurialEditController mController;
    }
}
