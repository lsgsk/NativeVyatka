using System;
using UIKit;
using SidebarNavigation;

namespace NativeVyatkaIOS
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController(IntPtr handle) : base(handle)
        {
            /*var mainframeController = AppDelegate.MainStoryboard.InstantiateViewController("BurialsListViewController");
            var menuController = AppDelegate.MainStoryboard.InstantiateViewController("MainMenuViewController");

            SidebarController = new SidebarController(this, mainframeController, mainframeController);
            SidebarController.MenuLocation = MenuLocations.Left;
            SidebarController.MenuWidth = 300;
            SidebarController.HasShadowing = false;*/
        }    
        public SidebarController SidebarController { get; private set; }
    }
}

