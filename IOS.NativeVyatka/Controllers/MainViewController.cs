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
            mMenuController = AppDelegate.MainStoryboard.InstantiateViewController("MainMenuViewController") as MainMenuViewController;
            mMenuController.CloceMenu += (s, e) => SidebarController.CloseMenu();
            SidebarController = new SidebarController(this, new BurialsListViewController(mController), mMenuController);
            SidebarController.MenuLocation = MenuLocations.Left;
            SidebarController.MenuWidth = 250;
            SidebarController.HasShadowing = true;
            SidebarController.Title = "Главный экран";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.LeftBarButtonItem = EditButtonItem;

            var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
            addButton.AccessibilityLabel = "addButton";
            NavigationItem.RightBarButtonItem = addButton;
            mMenuController.SetProfile(mController.Profile);
        }

        private void AddNewItem(object sender, EventArgs e)
        {
        }

        public SidebarController SidebarController { get; private set; }
        private MainMenuViewController mMenuController;
        public readonly IMainController mController;
    }
}

