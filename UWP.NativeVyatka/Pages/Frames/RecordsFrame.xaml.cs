using Abstractions.Interfaces.Controllers;
using Abstractions.Models.AppModels;
using NativeVyatka.UWP.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NativeVyatka.UWP.Pages.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecordsFrame : Page
    {
        public RecordsFrame()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayRecords();
        }

        private void DisplayRecords()
        {
            // Show the status bar progress indicator, if available.
            Windows.UI.ViewManagement.StatusBar statusBar = null;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            }
            if (statusBar != null)
            {
                var task = statusBar.ProgressIndicator.ShowAsync();
            }
            Items.Clear();
            mBurialCollection = mController.GetBurials();
            for (int i = 0; i < mBurialCollection.Count; ++i)
            {
                var burial = mBurialCollection[i];
                var name = $"{burial.Surname} {burial.Name} {burial.Patronymic}";
                Items.Insert(0, new BurialListItem()
                {
                    CloudId = burial.CloudId,
                    PicturePath = burial.PicturePath,
                    Name = string.IsNullOrWhiteSpace(name) ? "Неизвестное захоронение" : name,
                    Description = string.IsNullOrEmpty(burial.Description) ? "Без описания" : burial.Description,
                    Updated = burial.Updated ? Visibility.Collapsed : Visibility.Visible
                });
            }
            if (statusBar != null)
            {
                var task = statusBar.ProgressIndicator.HideAsync();
            }
        }

        public async Task ObtainData()
        {
            await mController.ForceSyncBurials();
            DisplayRecords();
        }

        private void BurialRecordItemClick(object sender, ItemClickEventArgs e)
        {
            var burial = mBurialCollection.FirstOrDefault(x => x.CloudId == (e.ClickedItem as BurialListItem).CloudId);
            if(burial != null)
                mController.DisplayBurial(burial);
        }

        private async void ListViewRefreshRequested(object sender, RefreshRequestedEventArgs e)
        {
            using (Deferral deferral = listView.AutoRefresh ? e.GetDeferral() : null)
            {
                await ObtainData();
                if (SpinnerStoryboard.GetCurrentState() != Windows.UI.Xaml.Media.Animation.ClockState.Stopped)
                {
                    SpinnerStoryboard.Stop();
                }
            }
        }

        private void ListViewPullProgressChanged(object sender, RefreshProgressEventArgs e)
        {
            if (e.IsRefreshable)
            {
                if (e.PullProgress == 1)
                {
                    // Progress = 1.0 means that the refresh has been triggered.
                    if (SpinnerStoryboard.GetCurrentState() == Windows.UI.Xaml.Media.Animation.ClockState.Stopped)
                    {
                        SpinnerStoryboard.Begin();
                    }
                }
                else if (SpinnerStoryboard.GetCurrentState() != Windows.UI.Xaml.Media.Animation.ClockState.Stopped)
                {
                    SpinnerStoryboard.Stop();
                }
                else
                {
                    // Turn the indicator by an amount proportional to the pull progress.
                    SpinnerTransform.Angle = e.PullProgress * 360;
                }
            }
        }

        public DependencyProperty UseAutoRefreshProperty = DependencyProperty.Register("UseAutoRefresh", typeof(bool), typeof(RecordsFrame), new PropertyMetadata(false));
        public bool UseAutoRefresh
        {
            get { return (bool)GetValue(UseAutoRefreshProperty); }
            set { SetValue(UseAutoRefreshProperty, value); }
        }

        public ObservableCollection<BurialListItem> Items { get; set; } = new ObservableCollection<BurialListItem>();

        private List<BurialModel> mBurialCollection = new List<BurialModel>();
        private IMainRecordsController mController
        {
            get
            {
                return MainPage.Controller;
            }
        }
    }

    public class BurialListItem
    {
        public string CloudId { get; set; }
        public string PicturePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Visibility Updated { get; set; }

    }
}

