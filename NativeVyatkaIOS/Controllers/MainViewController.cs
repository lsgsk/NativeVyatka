using System;
using UIKit;
using SidebarNavigation;
using NativeVyatkaIOS.Controllers;
using Abstractions.Interfaces.Controllers;
using Microsoft.Practices.Unity;

namespace NativeVyatkaIOS
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController()
        {
            mController = App.Container.Resolve<IMainController>();
            SidebarController = new SidebarController(this, new BurialsListViewController(), mMenuController = new MainMenuViewController());
            SidebarController.MenuLocation = MenuLocations.Left;
            SidebarController.MenuWidth = 280;
            SidebarController.HasShadowing = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mMenuController.SetProfile(mController.Profile);
        }

        public SidebarController SidebarController { get; private set; }
        private MainMenuViewController mMenuController;
        public readonly IMainController mController;
    }
}

