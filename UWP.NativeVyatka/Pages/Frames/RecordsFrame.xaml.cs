using NativeVyatka.UWP.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        private Random _rand = new Random();

        public ObservableCollection<ListItemData> Items { get; set; } = new ObservableCollection<ListItemData>();

        public DependencyProperty UseAutoRefreshProperty = DependencyProperty.Register("UseAutoRefresh", typeof(bool), typeof(RecordsFrame), new PropertyMetadata(false));
        public bool UseAutoRefresh
        {
            get { return (bool)GetValue(UseAutoRefreshProperty); }
            set { SetValue(UseAutoRefreshProperty, value); }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await FetchAndInsertItemsAsync(_rand.Next(1, 21));
        }

        private async Task FetchAndInsertItemsAsync(int updateCount)
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

            // Simulate delay while we go fetch new items.
            await Task.Delay(500);

            for (int i = 0; i < updateCount; ++i)
            {
                Items.Insert(0, GetNextItem());
            }

            if (statusBar != null)
            {
                var task = statusBar.ProgressIndicator.HideAsync();
            }
        }

        private ListItemData GetNextItem()
        {
            return new ListItemData()
            {
                Image = "ms-appx:///Assets/Images/nophoto.png",
                Header = RandomSentence(),
                Attribution = RandomSentence(),
                Body = RandomSentence()
            };
        }

        private string RandomSentence()
        {
            return _rand.Next(int.MaxValue).ToString();
        }

        private async void listView_RefreshRequested(object sender, RefreshRequestedEventArgs e)
        {
            using (Deferral deferral = listView.AutoRefresh ? e.GetDeferral() : null)
            {
                await FetchAndInsertItemsAsync(_rand.Next(1, 5));

                if (SpinnerStoryboard.GetCurrentState() != Windows.UI.Xaml.Media.Animation.ClockState.Stopped)
                {
                    SpinnerStoryboard.Stop();
                }
            }
        }

        private void listView_PullProgressChanged(object sender, RefreshProgressEventArgs e)
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
    }
}
