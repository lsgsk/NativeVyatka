using Abstractions.Interfaces.Plugins;
using UIKit;
using Abstractions.Models;
using Abstractions.Models.AppModels;
using NativeVyatkaIOS.Utilities.TableSources;

namespace NativeVyatkaIOS.Controllers
{
    public partial class MainMenuViewController : UIViewController
    {
        public MainMenuViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var tableItems = new MenuItem[]
                                 {
                                    new MenuItem(NavigationMenuIds.RecordsList,"Register Visitor","icon_visitor_signin"),
                                    new MenuItem(NavigationMenuIds.RecordsMap,"Sign-Out Visitor","icon_signout_visitor")
                                 };
            var ds = new TableSource(tableItems);
            ds.ItemSelected += (item) =>
            {
                switch (item.Id)
                {
                    case NavigationMenuIds.RecordsList:
                    case NavigationMenuIds.RecordsMap:
                        break;
                }
                //SidebarController.CloseMenu();
            };
            tvMenuList.Source = ds;
        }

        public void SetProfile(ProfileModel profile)
        {
            lbProfileName.Text = profile.Name;
            lbProfileEmail.Text = profile.Email;
            //imgProfilePhoto.Image = UIImage.FromFile(profile.PictureUrl);
        }
    }
}