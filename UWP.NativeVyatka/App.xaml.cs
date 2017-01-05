using Microsoft.Practices.Unity;
using NativeVyatka.UWP.Pages;
using NativeVyatka.UWP.Utilities;
using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using NativeVyatkaCore.Utilities;
using Plugin.Media;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NativeVyatka.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            BurialDatabase.InitILobbyPhoneDatabase(ApplicationData.Current.LocalFolder.Path);
            iConsole.Init(new ConsoleRealization());
            CrossMedia.Current.Initialize();
            RegisterTypesIntoDI.InitContainer(Container);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                Window.Current.Content = rootFrame;
            }

            var statusBar = StatusBar.GetForCurrentView();
            if (statusBar != null)
            {
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = (Color)Resources["ColorPrimaryDark"];
                statusBar.ForegroundColor = Colors.White;
            }
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        public static UnityContainer Container { get; } = new UnityContainer();
    }
}
